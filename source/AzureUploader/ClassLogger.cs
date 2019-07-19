using Microsoft.Extensions.Logging;

namespace AzureUploader
{
    internal class ClassLogger<T> : IClassLogger
    {
        private readonly string _className = typeof(T).Name;
        private readonly ILogger _logger;

        public ClassLogger(ILogger logger) => _logger = logger;

        public void Log(string message) => _logger?.LogInformation($"{_className}: {message}");
    }
}
