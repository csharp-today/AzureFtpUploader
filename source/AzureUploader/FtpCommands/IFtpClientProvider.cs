using FluentFTP;

namespace AzureUploader.FtpCommands
{
    internal interface IFtpClientProvider
    {
        void CloseActiveClient();
        FtpClient GetClient();
    }
}
