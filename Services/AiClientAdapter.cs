using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HireZ.Services
{
    /// <summary>
    /// Adapter that tries to locate an existing IAiService implementation in DI at runtime
    /// and invoke a method (GenerateAsync or similar) via reflection. If unsuccessful, returns null.
    /// This allows the code to compile without depending on the concrete IAiService signature.
    /// </summary>
    public class AiClientAdapter : IAiClient
    {
        private readonly IServiceProvider _provider;

        public AiClientAdapter(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<string?> GenerateAsync(string prompt)
        {
            // Try to find an IAiService interface type in loaded assemblies
            var aiInterface = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                })
                .FirstOrDefault(t => t.IsInterface && (t.Name == "IAiService" || t.Name == "IGenieService" || t.Name == "IAiProvider"));

            if (aiInterface == null)
            {
                // No IAiService interface found
                return null;
            }

            // Resolve the implementation from DI
            var aiService = _provider.GetService(aiInterface);
            if (aiService == null) return null;

            // Try common method names
            var candidateMethodNames = new[] { "GenerateAsync", "Generate", "CallAsync", "CompleteAsync", "CreateAsync", "RunAsync" };

            MethodInfo? method = null;
            foreach (var name in candidateMethodNames)
            {
                method = aiService.GetType().GetMethod(name, new[] { typeof(string) });
                if (method != null) break;
            }

            if (method == null)
            {
                // Try methods with additional parameters: (string prompt, object options) => skip for now
                method = aiService.GetType().GetMethods().FirstOrDefault(m =>
                    m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string));
            }

            if (method == null) return null;

            // Invoke method and await Task<string> if returned
            try
            {
                var result = method.Invoke(aiService, new object[] { prompt });
                if (result == null) return null;

                // If result is Task or Task<T>, await it
                if (result is Task task)
                {
                    await task.ConfigureAwait(false);

                    // Try to get Result property for Task<T>
                    var resultProp = task.GetType().GetProperty("Result");
                    if (resultProp != null)
                    {
                        var res = resultProp.GetValue(task);
                        return res?.ToString();
                    }

                    return null;
                }

                // If method returned string directly
                return result.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
