using AzureUploader.Checksums;
using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpUploader : IFtpUploader
    {
        private readonly IChecksumCalculator _checksumCalculator;
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IClassLogger _logger;

        public FtpUploader(IChecksumCalculator checksumCalculator, IFtpCommandExecutor ftpCommandExecutor, IClassLogger logger) =>
            (_checksumCalculator, _ftpExecutor, _logger) = (checksumCalculator, ftpCommandExecutor, logger);

        public void UploadDirectory(string directoryPath, string targetPath)
        {
            var directories = Directory.GetDirectories(directoryPath);
            foreach (var dir in directories)
            {
                var name = Path.GetFileName(dir);
                var ftpPath = $"{targetPath}/{name}";
                _logger.Log("Create: " + ftpPath);
                _ftpExecutor.Execute(c => c.CreateDirectory(ftpPath));
                UploadDirectory(dir, ftpPath);
            }

            var files = Directory.GetFiles(directoryPath);
            foreach (var file in files)
            {
                UploadFile(file, $"{targetPath}/{Path.GetFileName(file)}");
            }
        }

        public void UploadFile(string filePath, string targetPath)
        {
            _logger.Log("Upload: " + targetPath);
            _logger.Log($"Size={new FileInfo(filePath).Length}");
            _logger.Log($"MD5={_checksumCalculator.CalculateChecksum(filePath)}");
            _ftpExecutor.Execute(c => c.UploadFile(filePath, targetPath));
        }
    }
}
