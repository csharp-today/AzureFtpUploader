using AzureUploader.Checksums;
using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpFileUploader : IFtpFileUploader
    {
        private readonly IChecksumCalculator _checksumCalculator;
        private readonly ChecksumDataStorage _checksumDataStorage;
        private readonly IFtpCommandExecutor _ftpExecutor;

        public FtpFileUploader(IChecksumCalculator checksumCalculator, IFtpCommandExecutor ftpCommandExecutor, ChecksumDataStorage checksumDataStorage) =>
            (_checksumCalculator, _checksumDataStorage, _ftpExecutor) = (checksumCalculator, checksumDataStorage, ftpCommandExecutor);

        public void UploadFile(string filePath, string targetPath)
        {
            var checksum = _checksumCalculator.CalculateChecksum(filePath);
            _checksumDataStorage.Store(targetPath, checksum);

            _ftpExecutor.Execute(c => c.UploadFile(filePath, targetPath));
        }
    }
}
