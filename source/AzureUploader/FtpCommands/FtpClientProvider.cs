using FluentFTP;
using System;

namespace AzureUploader.FtpCommands
{
    internal class FtpClientProvider : IFtpClientProvider
    {
        private readonly Func<FtpClient> _clientFactory;
        private readonly object _lock = new object();

        private FtpClient _client;

        public FtpClientProvider(Func<FtpClient> clientFactory) => _clientFactory = clientFactory;

        public void CloseActiveClient()
        {
            FtpClient client;
            lock (_lock)
            {
                client = _client;
                _client = null;
            }

            client?.Dispose();
        }

        public FtpClient GetClient()
        {
            FtpClient client;
            lock (_lock)
            {
                if (_client is null)
                {
                    _client = CreateClient();
                }

                return _client;
            }
        }

        private FtpClient CreateClient()
        {
            var client = _clientFactory();
            client.EncryptionMode = FtpEncryptionMode.Explicit;
            client.ReadTimeout = 60_000;
            client.RetryAttempts = 3;
            client.Connect();
            return client;
        }
    }
}
