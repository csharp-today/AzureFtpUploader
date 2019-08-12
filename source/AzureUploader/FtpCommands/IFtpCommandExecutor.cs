using FluentFTP;
using System;

namespace AzureUploader.FtpCommands
{
    internal interface IFtpCommandExecutor
    {
        void Execute(Action<FtpClient> operation, Func<bool> wasSuccessfulCheck = null);
        void Execute(Action<FtpClient> operation, Func<FtpClient, bool> wasSuccessfulCheck);
        T Execute<T>(Func<FtpClient, T> operation);
    }
}
