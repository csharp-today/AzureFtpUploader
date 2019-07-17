using AzureUploader.Checksums;
using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpFileUploader : IFtpFileUploader
    {
        private readonly IChecksumCalculator _checksumCalculator;
        private readonly ChecksumDataStorage _checksumDataStorage;
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IClassLogger _logger;

        public FtpFileUploader(IChecksumCalculator checksumCalculator, IFtpCommandExecutor ftpCommandExecutor, IClassLogger logger, ChecksumDataStorage checksumDataStorage) =>
            (_checksumCalculator, _checksumDataStorage, _ftpExecutor, _logger) = (checksumCalculator, checksumDataStorage, ftpCommandExecutor, logger);

        public void UploadFile(string filePath, string targetPath)
        {
            _logger.Log("Upload: " + targetPath);
            _logger.Log($"Size={new FileInfo(filePath).Length}");

            var checksum = _checksumCalculator.CalculateChecksum(filePath);
            _checksumDataStorage.Store(targetPath, checksum);
            _logger.Log($"MD5={checksum}");

            _ftpExecutor.Execute(c => c.UploadFile(filePath, targetPath));
        }
    }
}
