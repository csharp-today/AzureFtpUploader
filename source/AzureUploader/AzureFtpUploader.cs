﻿using AzureUploader.Checksums;
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
        private readonly IFtpUploader _ftpUploader;
        private readonly IClassLogger _logger;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null)
        {
            _logger = new ClassLogger<AzureFtpUploader>(logger);
            _ftpExecutor = new FtpCommandExecutor(new FtpClientProvider(clientFactory), _logger);
            _ftpUploader = new FtpUploader(new Md5Calculator(), _ftpExecutor, _logger);
        }

        public void Deploy(string directory)
        {
            EnsurePublishDirectoryExists(directory);
            _logger.Log("CLEAN FTP");
            Clean();
            _logger.Log("PUSH NEW CONTENT");
            Push(directory);
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
                        _logger.Log($"Size={_ftpExecutor.Execute(c => c.GetFileSize(item.FullName))}");
                        _ftpExecutor.Execute(c => c.DeleteFile(item.FullName));
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

        private void Push(string directory) => PushDirectory(directory, RootDirectory);

        private void PushDirectory(string source, string target)
        {
            var directories = Directory.GetDirectories(source);
            foreach (var dir in directories)
            {
                var name = Path.GetFileName(dir);
                var targetPath = $"{target}/{name}";
                _logger.Log("Create: " + targetPath);
                _ftpExecutor.Execute(c => c.CreateDirectory(targetPath));
                PushDirectory(dir, targetPath);
            }

            var files = Directory.GetFiles(source);
            foreach (var file in files)
            {
                _ftpUploader.UploadFile(file, $"{target}/{Path.GetFileName(file)}");
            }
        }
    }
}
