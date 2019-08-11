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

            Log("UPDATE FTP CONTENT");
            _composer.ContentUpdater.UpdateContent(diffTree, directory, RootDirectory, ChecksumFilePath);

            Log("DEPLOYMENT DONE");
        }

        private void Log(string message) => _composer.Logger.Log(message);
    }
}
