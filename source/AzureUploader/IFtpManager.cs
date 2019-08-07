using AzureUploader.Checksums;
using AzureUploader.FtpCommands;

namespace AzureUploader
{
    internal interface IFtpManager : IFtpContentGetter, IFtpDirectoryRemover, IFtpFileRemover, IFtpDirectoryUploader, IFtpTextReader, IFtpTextUploader
    {
    }
}
