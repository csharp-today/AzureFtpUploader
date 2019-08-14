using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpDirectoryUploader : IFtpDirectoryUploader
    {
        private readonly IClassLogger _logger;
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IFtpFileUploader _ftpFileUploader;

        public FtpDirectoryUploader(IFtpCommandExecutor ftpExecutor, IFtpFileUploader ftpFileUploader, IClassLogger logger) =>
            (_ftpExecutor, _ftpFileUploader, _logger) = (ftpExecutor, ftpFileUploader, logger);

        public void UploadDirectory(string directoryPath, string targetPath)
        {
            var directories = Directory.GetDirectories(directoryPath);
            foreach (var dir in directories)
            {
                var name = Path.GetFileName(dir);
                var ftpPath = $"{targetPath}/{name}";
                _logger.Log("Create: " + ftpPath);
                _ftpExecutor.Execute(c => c.CreateDirectory(ftpPath), c => c.DirectoryExists(ftpPath));
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
