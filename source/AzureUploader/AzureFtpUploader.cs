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

        private readonly Composer<AzureFtpUploader> _composer;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null) => _composer = new Composer<AzureFtpUploader>(clientFactory, logger);

        public void Deploy(string directory)
        {
            Log("ANALYZE DIRECTORY TO UPLOAD");
            EnsurePublishDirectoryExists(directory);
            var localTree = _composer.LocalDirectoryTreeBuilder.BuildUsingLocalDirectory(directory);
            Log(localTree.ToString());
            Log("ANALYZE FTP DIRECTORY");
            var targetTree = _composer.FtpDirectoryTreeBuilder.BuildUsingFtpDirectory(RootDirectory);
            Log(targetTree.ToString());
            Log("READ FTP CHECKSUMs");
            _composer.ChecksumDataStorage.RestoreFromDump(_composer.FtpManager.ReadText(ChecksumFilePath));
            Log("COMPARE SOURCE AND TARGET");
            var diffTree = _composer.TreeComparer.Compare(localTree, targetTree);
            Log(diffTree.ToString());
            Log("CLEAN FTP");
            _composer.FtpManager.RemoveDirectory(RootDirectory);
            _composer.FtpManager.RemoveFile(ChecksumFilePath);
            Log("PUSH NEW CONTENT");
            _composer.FtpManager.UploadDirectory(directory, RootDirectory);
            Log("UPLOAD CHECKSUMs");
            _composer.FtpManager.UploadText(_composer.ChecksumDataStorage.GetStorageDump(), ChecksumFilePath);
            Log("DEPLOYMENT DONE");
        }

        private void EnsurePublishDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new Exception("Publish directory doesn't exist: " + directory);
            }
        }

        private void Log(string message) => _composer.Logger.Log(message);
    }
}
