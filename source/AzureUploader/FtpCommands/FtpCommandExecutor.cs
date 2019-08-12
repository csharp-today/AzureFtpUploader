using FluentFTP;
using System;
using System.Threading;

namespace AzureUploader.FtpCommands
{
    internal class FtpCommandExecutor : IFtpCommandExecutor
    {
        private const int StartTime = 3;
        private const int StartCount = 6;

        private readonly IFtpClientProvider _ftpClientProvider;
        private readonly IClassLogger _logger;

        public FtpCommandExecutor(IFtpClientProvider ftpClientProvider, IClassLogger logger) =>
            (_ftpClientProvider, _logger) = (ftpClientProvider, logger);

        public void Execute(Action<FtpClient> operation, Func<bool> wasSuccessfulCheck = null)
        {
            int time = StartTime;
            int count = StartCount;
            while (count > 0)
            {
                try
                {
                    operation(_ftpClientProvider.GetClient());
                }
                catch (Exception error)
                {
                    // Usually Azure FTP needs to stage a new connection
                    _ftpClientProvider.CloseActiveClient();

                    if (wasSuccessfulCheck != null)
                    {
                        _logger.Log("Operation failed: " + error.Message);
                        _logger.Log("Running success check...");

                        try
                        {
                            if (wasSuccessfulCheck())
                            {
                                _logger.Log("Operation was successful after all.");
                                return;
                            }
                        }
                        catch (Exception checkError)
                        {
                            _logger.Log("Ignoring success check - it failed: " + checkError.Message);
                        }
                    }

                    UpdateLoop(error, ref time, ref count);
                }
            }

            throw new NotImplementedException();
        }

        public void Execute(Action<FtpClient> operation, Func<FtpClient, bool> wasSuccessfulCheck) =>
            Execute(operation, () => Execute(c => wasSuccessfulCheck(c)));

        public T Execute<T>(Func<FtpClient, T> operation)
        {
            int time = StartTime;
            int count = StartCount;
            while (count > 0)
            {
                try
                {
                    return operation(_ftpClientProvider.GetClient());
                }
                catch (Exception error)
                {
                    // Usually Azure FTP needs to stage a new connection
                    _ftpClientProvider.CloseActiveClient();

                    UpdateLoop(error, ref time, ref count);
                }
            }

            throw new NotImplementedException();
        }

        private void UpdateLoop(Exception error, ref int time, ref int count)
        {
            count--;
            _logger.Log("Failed - retry count: " + count);
            if (count <= 0)
            {
                throw new InvalidOperationException("Can't complete FTP operation", error);
            }

            _logger.Log($"Will retry in {time} seconds");
            Thread.Sleep(time * 1000);
            time *= 2;
        }
    }
}
