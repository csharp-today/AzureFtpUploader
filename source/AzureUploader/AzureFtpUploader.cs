using AzureUploader.Checksums;
using AzureUploader.DirectoryTrees;
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

        private readonly IFtpDirectoryTreeBuilder _ftpDirectoryTreeBuilder;
        private readonly ILocalDirectoryTreeBuilder _localDirectoryTreeBuilder;
        private readonly IFtpManager _ftpManager;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null)
        {
            _ftpDirectoryTreeBuilder = new FtpDirectoryTreeBuilder();
            _localDirectoryTreeBuilder = new LocalDirectoryTreeBuilder();
            _ftpManager = new FtpManager(clientFactory, logger);
        }

        public void Deploy(string directory)
        {
            Log("ANALYZE DIRECTORY TO UPLOAD");
            EnsurePublishDirectoryExists(directory);
            var localTree = _localDirectoryTreeBuilder.BuildUsingLocalDirectory(directory);
            Log(localTree.ToString());
            Log("ANALYZE TARGET DIRECTORY");
            var targetTree = _ftpDirectoryTreeBuilder.BuildUsingFtpDirectory(_ftpManager, RootDirectory);
            Log(targetTree.ToString());
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
