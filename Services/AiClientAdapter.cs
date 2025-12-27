using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HireZ.Services
{
    public class AiClientAdapter : IAiClient
    {
        private readonly IServiceProvider _provider;

        public AiClientAdapter(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<string?> GenerateAsync(string prompt)
        {
            var aiInterface = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                })
                .FirstOrDefault(t => t.IsInterface && (t.Name == "IAiService"));

            if (aiInterface == null) return null;

            var aiService = _provider.GetService(aiInterface);
            if (aiService == null) return null;

            // Try to find a method that accepts a single string and returns Task<string> or string
            var method = aiService.GetType().GetMethods()
                .FirstOrDefault(m =>
                {
                    var ps = m.GetParameters();
                    return ps.Length == 1 && ps[0].ParameterType == typeof(string);
                });

            if (method == null) return null;

            try
            {
                var result = method.Invoke(aiService, new object[] { prompt });
                if (result == null) return null;

                if (result is Task task)
                {
                    await task.ConfigureAwait(false);
                    var resProp = task.GetType().GetProperty("Result");
                    if (resProp != null)
                    {
                        var val = resProp.GetValue(task);
                        return val?.ToString();
                    }
                    return null;
                }

                return result.ToString();
            }
            catch
            {
                return null;
            }
        }

        public Task<string?> GenerateAsync(object prompt)
        {
            throw new NotImplementedException();
        }
    }
}
