using AzureUploader.Checksums;
using AzureUploader.FtpCommands;
using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AzureUploader
{
    public class AzureFtpUploader
    {
        private const string ChecksumDirectory = "/site";
        private const string ChecksumFilePath = ChecksumDirectory + "/checksum.txt";
        private const string RootDirectory = ChecksumDirectory + "/wwwroot";
        private readonly ChecksumDataStorage _checksumDataStorage = new ChecksumDataStorage();
        private readonly IFtpDirectoryRemover _ftpRemover;
        private readonly IFtpDirectoryUploader _ftpDirectoryUploader;
        private readonly IFtpTextUploader _ftpTextUploader;
        private readonly IClassLogger _logger;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null)
        {
            _logger = new ClassLogger<AzureFtpUploader>(logger);
            var ftpExecutor = new FtpCommandExecutor(new FtpClientProvider(clientFactory), _logger);
            _ftpRemover = new FtpDirectoryRemover(ftpExecutor, new FtpFileRemover(ftpExecutor, _logger), _logger);
            var ftpFileUploader = new FtpFileUploader(new Md5Calculator(), ftpExecutor, _logger, _checksumDataStorage);
            _ftpDirectoryUploader = new FtpDirectoryUploader(ftpExecutor, ftpFileUploader, _logger);
            _ftpTextUploader = new FtpTextUploader(ftpFileUploader);
        }

        public void Deploy(string directory)
        {
            EnsurePublishDirectoryExists(directory);
            _logger.Log("CLEAN FTP");
            _ftpRemover.RemoveDirectory(RootDirectory);
            _logger.Log("PUSH NEW CONTENT");
            _ftpDirectoryUploader.UploadDirectory(directory, RootDirectory);
            _logger.Log("UPLOAD CHECKSUMs");
            _ftpTextUploader.UploadText(_checksumDataStorage.GetStorageDump(), ChecksumFilePath);
            _logger.Log("DEPLOYMENT DONE");
        }

        private void EnsurePublishDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new Exception("Publish directory doesn't exist: " + directory);
            }
        }
    }
}
