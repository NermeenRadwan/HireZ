// Program.cs
using System.Text;
using HireZ.Data;
using HireZ.Services;
using HireZ.Services.Background;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Basic services ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---------- Swagger with JWT support ----------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HireZ API", Version = "v1" });

    // Add JWT auth to swagger
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {token}'",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// ---------- DbContext (SQL Server) ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("DefaultConnection is not configured in appsettings.json (or environment).");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------- Application services ----------
builder.Services.AddScoped<IUserService, UserService>();

// ---------- JWT configuration ----------
var jwtSection = builder.Configuration.GetSection("Jwt");
// allow overriding secret with environment variable "JWT__Key" or "HIREZ_JWT_KEY"
var keyFromConfig = jwtSection.GetValue<string>("Key");
var key = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("HIREZ_JWT_KEY"))
    ? Environment.GetEnvironmentVariable("HIREZ_JWT_KEY")!
    : (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("JWT__Key")) ? Environment.GetEnvironmentVariable("JWT__Key")! : keyFromConfig);

if (string.IsNullOrWhiteSpace(key))
{
    throw new Exception("JWT key is not configured. Set Jwt:Key in appsettings.json or the HIREZ_JWT_KEY environment variable.");
}

var issuer = jwtSection.GetValue<string>("Issuer") ?? "HireZApi";
var audience = jwtSection.GetValue<string>("Audience") ?? "HireZClients";
var expiryMinutes = jwtSection.GetValue<int?>("ExpiryMinutes") ?? 1440;

var keyBytes = Encoding.UTF8.GetBytes(key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // In production set RequireHttpsMetadata = true
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ---------- Apply migrations and seed DB (development friendly) ----------
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying pending migrations (if any)...");
        db.Database.Migrate();

        logger.LogInformation("Seeding database...");
        DbSeeder.Seed(db);
        logger.LogInformation("Database ready.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        // Re-throw to fail fast in CI / dev if desired:
        throw;
    }
}

builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
builder.Services.AddSingleton<ITextExtractionService, PdfTextExtractionService>();
builder.Services.AddSingleton<ResumeAnalysisQueue>();
builder.Services.AddHostedService<ResumeAnalysisWorker>();
builder.Services.AddScoped<IAiService, OpenAiServiceStub>();
builder.Services.AddScoped<IResumeService, ResumeService>();

// ---------- Middleware pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HireZ API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
