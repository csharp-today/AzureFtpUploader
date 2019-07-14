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
        private const string RootDirectory = "/site/wwwroot";
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IFtpDirectoryUploader _ftpUploader;
        private readonly IFtpFileRemover _ftpRemover;
        private readonly IClassLogger _logger;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null)
        {
            _logger = new ClassLogger<AzureFtpUploader>(logger);
            _ftpExecutor = new FtpCommandExecutor(new FtpClientProvider(clientFactory), _logger);
            _ftpRemover = new FtpFileRemover(_ftpExecutor, _logger);
            _ftpUploader = new FtpDirectoryUploader(_ftpExecutor, new FtpFileUploader(new Md5Calculator(), _ftpExecutor, _logger), _logger);
        }

        public void Deploy(string directory)
        {
            EnsurePublishDirectoryExists(directory);
            _logger.Log("CLEAN FTP");
            Clean();
            _logger.Log("PUSH NEW CONTENT");
            _ftpUploader.UploadDirectory(directory, RootDirectory);
            _logger.Log("DEPLOYMENT DONE");
        }

        private void Clean(string path = RootDirectory)
        {
            var items = _ftpExecutor.Execute(c => c.GetListing(path));
            foreach (var item in items)
            {
                _logger.Log($"Remove {item.Type.ToString().ToLower()}: {item.FullName}");
                switch (item.Type)
                {
                    case FtpFileSystemObjectType.File:
                    case FtpFileSystemObjectType.Link:
                        _ftpRemover.RemoveFile(item.FullName);
                        break;
                    case FtpFileSystemObjectType.Directory:
                        Clean(item.FullName);
                        _ftpExecutor.Execute(c => c.DeleteDirectory(item.FullName));
                        break;
                }
            }
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
