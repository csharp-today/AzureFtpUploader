using AzureUploader.Checksums;
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
        
        private readonly IFtpManager _ftpManager;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null) => _ftpManager = new FtpManager(clientFactory, logger);

        public void Deploy(string directory)
        {
            EnsurePublishDirectoryExists(directory);
            Log("READ FTP CHECKSUMs");
            var checksums = new ChecksumDataStorage();
            checksums.RestoreFromDump(_ftpManager.ReadText(ChecksumFilePath));
            Log("CLEAN FTP");
            _ftpManager.RemoveDirectory(RootDirectory);
            _ftpManager.RemoveFile(ChecksumFilePath);
            Log("PUSH NEW CONTENT");
            _ftpManager.UploadDirectory(directory, RootDirectory);
            Log("UPLOAD CHECKSUMs");
            _ftpManager.UploadText(_ftpManager.ChecksumDataStorage.GetStorageDump(), ChecksumFilePath);
            Log("DEPLOYMENT DONE");
        }

        private void EnsurePublishDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new Exception("Publish directory doesn't exist: " + directory);
            }
        }

        private void Log(string message) => _ftpManager.Logger.Log(message);
    }
}
