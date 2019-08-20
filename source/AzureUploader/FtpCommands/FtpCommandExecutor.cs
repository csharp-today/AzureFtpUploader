using FluentFTP;
using System;
using System.Text;
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
            var lazyLog = new StringBuilder();
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
                        lazyLog.AppendLine("Operation failed: " + error.Message);
                        lazyLog.AppendLine("Running success check...");

                        try
                        {
                            if (wasSuccessfulCheck())
                            {
                                lazyLog.AppendLine("Operation was successful after all.");
                                return;
                            }
                        }
                        catch (Exception checkError)
                        {
                            lazyLog.AppendLine("Ignoring success check - it failed: " + checkError.Message);
                        }
                    }

                    UpdateLoop(error, ref time, ref count, lazyLog);
                }
            }

            throw new NotImplementedException();
        }

        public void Execute(Action<FtpClient> operation, Func<FtpClient, bool> wasSuccessfulCheck) =>
            Execute(operation, () => Execute(c => wasSuccessfulCheck(c)));

        public T Execute<T>(Func<FtpClient, T> operation)
        {
            var lazyLog = new StringBuilder();
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

                    UpdateLoop(error, ref time, ref count, lazyLog);
                }
            }

            throw new NotImplementedException();
        }

        private void UpdateLoop(Exception error, ref int time, ref int count, StringBuilder lazyLog)
        {
            count--;
            lazyLog.AppendLine("Failed - retry count: " + count);
            if (count <= 0)
            {
                // Print lazy-log only in case of failure
                _logger.Log(lazyLog.ToString());
                throw new InvalidOperationException("Can't complete FTP operation", error);
            }

            lazyLog.AppendLine($"Will retry in {time} seconds");
            Thread.Sleep(time * 1000);
            time *= 2;
        }
    }
}
