using Microsoft.Extensions.Logging;

namespace AzureUploader
{
    internal class ClassLogger<T>
    {
        private readonly ILogger _logger;

        public ClassLogger(ILogger logger) => _logger = logger;

        public void Log(string message) => _logger?.LogInformation($"{nameof(T)}: {message}");
    }
}
