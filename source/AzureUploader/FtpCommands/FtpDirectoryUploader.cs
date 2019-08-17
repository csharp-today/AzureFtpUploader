using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpDirectoryUploader : IFtpDirectoryUploader
    {
        private readonly IClassLogger _logger;
        private readonly IFtpDirectoryCreator _ftpDirectoryCreator;
        private readonly IFtpFileUploader _ftpFileUploader;

        public FtpDirectoryUploader(IFtpDirectoryCreator ftpDirectoryCreator, IFtpFileUploader ftpFileUploader, IClassLogger logger) =>
            (_ftpDirectoryCreator, _ftpFileUploader, _logger) = (ftpDirectoryCreator, ftpFileUploader, logger);

        public void UploadDirectory(string directoryPath, string targetPath)
        {
            var directories = Directory.GetDirectories(directoryPath);
            foreach (var dir in directories)
            {
                var name = Path.GetFileName(dir);
                var ftpPath = $"{targetPath}/{name}";
                _logger.Log("Create: " + ftpPath);
                _ftpDirectoryCreator.CreateDirectory(ftpPath);
                UploadDirectory(dir, ftpPath);
            }

            var files = Directory.GetFiles(directoryPath);
            foreach (var file in files)
            {
                _ftpFileUploader.UploadFile(file, $"{targetPath}/{Path.GetFileName(file)}");
            }
        }
    }
}
