using AzureUploader.Checksums;
using AzureUploader.FtpCommands;

namespace AzureUploader
{
    internal interface IFtpManager :
        IFtpContentGetter,
        IFtpDirectoryCreator,
        IFtpDirectoryRemover,
        IFtpDirectoryUploader,
        IFtpExistenceChecker,
        IFtpFileRemover,
        IFtpFileUploader,
        IFtpTextReader,
        IFtpTextUploader
    {
    }
}
