using FluentFTP;
using System;
using System.Threading;

namespace AzureUploader.FtpCommands
{
    internal class FtpCommandExecutor : IFtpCommandExecutor
    {
        private readonly IFtpClientProvider _ftpClientProvider;
        private readonly IClassLogger _logger;

        public FtpCommandExecutor(IFtpClientProvider ftpClientProvider, IClassLogger logger) =>
            (_ftpClientProvider, _logger) = (ftpClientProvider, logger);

        public void Execute(Action<FtpClient> operation) => Execute(c => { operation(c); return 0; });

        public T Execute<T>(Func<FtpClient, T> operation)
        {
            int time = 3;
            int count = 6;
            while (count > 0)
            {
                try
                {
                    return operation(_ftpClientProvider.GetClient());
                }
                catch (Exception)
                {
                    // Usually Azure FTP needs to stage a new connection
                    _ftpClientProvider.CloseActiveClient();

                    count--;
                    _logger.Log("Failed - retry count: " + count);
                    if (count <= 0)
                    {
                        throw;
                    }
                    _logger.Log($"Will retry in {time} seconds");
                    Thread.Sleep(time * 1000);
                    time *= 2;
                }
            }

            throw new NotImplementedException();
        }
    }
}
