using AzureUploader.Checksums;
using AzureUploader.FtpCommands;

namespace AzureUploader
{
    internal interface IFtpManager : IFtpContentGetter, IFtpDirectoryRemover, IFtpExistenceChecker, IFtpFileRemover, IFtpDirectoryUploader, IFtpTextReader, IFtpTextUploader
    {
    }
}
