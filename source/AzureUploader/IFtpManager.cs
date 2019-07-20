using AzureUploader.Checksums;
using AzureUploader.FtpCommands;

namespace AzureUploader
{
    internal interface IFtpManager : IFtpDirectoryRemover, IFtpDirectoryUploader, IFtpTextUploader
    {
        ChecksumDataStorage ChecksumDataStorage { get; }
        IClassLogger Logger { get; }
    }
}
