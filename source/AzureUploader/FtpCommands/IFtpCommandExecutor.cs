using FluentFTP;
using System;

namespace AzureUploader.FtpCommands
{
    internal interface IFtpCommandExecutor
    {
        void Execute(Action<FtpClient> operation);
        T Execute<T>(Func<FtpClient, T> operation);
    }
}
