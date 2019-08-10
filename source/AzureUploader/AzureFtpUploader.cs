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
            Log("GENERATE DIFFERENCE - LOCAL vs FTP");
            var diffTree = _composer.DifferenceGenerator.GenerateDifferenceTree(directory, RootDirectory, ChecksumFilePath);
            Log(diffTree.ToString());

            Log("CLEAN FTP");
            _composer.FtpManager.CleanDirectory(RootDirectory);
            _composer.FtpManager.RemoveFile(ChecksumFilePath);
            Log("PUSH NEW CONTENT");
            _composer.FtpManager.UploadDirectory(directory, RootDirectory);
            Log("UPLOAD CHECKSUMs");
            _composer.FtpManager.UploadText(_composer.ChecksumDataStorage.GetStorageDump(), ChecksumFilePath);
            Log("DEPLOYMENT DONE");
        }

        private void Log(string message) => _composer.Logger.Log(message);
    }
}
