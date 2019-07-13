using AzureUploader.Checksums;
using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpFileUploader : IFtpFileUploader
    {
        private readonly IChecksumCalculator _checksumCalculator;
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IClassLogger _logger;

        public FtpFileUploader(IChecksumCalculator checksumCalculator, IFtpCommandExecutor ftpCommandExecutor, IClassLogger logger) =>
            (_checksumCalculator, _ftpExecutor, _logger) = (checksumCalculator, ftpCommandExecutor, logger);

        public void UploadFile(string filePath, string targetPath)
        {
            _logger.Log("Upload: " + targetPath);
            _logger.Log($"Size={new FileInfo(filePath).Length}");
            _logger.Log($"MD5={_checksumCalculator.CalculateChecksum(filePath)}");
            _ftpExecutor.Execute(c => c.UploadFile(filePath, targetPath));
        }
    }
}
