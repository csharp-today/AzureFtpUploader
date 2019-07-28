using FluentFTP;

namespace AzureUploader.FtpCommands
{
    internal interface IFtpContentGetter
    {
        FtpListItem[] GetContent(string path);
    }
}
