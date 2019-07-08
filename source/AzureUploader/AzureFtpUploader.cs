﻿using AzureUploader.FtpCommands;
using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AzureUploader
{
    public class AzureFtpUploader
    {
        private const string RootDirectory = "/site/wwwroot";
        private readonly BaseFtpCommand _baseFtpCommand;
        private readonly IClassLogger _logger;

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null)
        {
            _logger = new ClassLogger<AzureFtpUploader>(logger);
            _baseFtpCommand = new BaseFtpCommand(new FtpClientProvider(clientFactory), _logger);
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
            var items = _baseFtpCommand.FtpCall(c => c.GetListing(path));
            foreach (var item in items)
            {
                _logger.Log($"Remove {item.Type.ToString().ToLower()}: {item.FullName}");
                switch (item.Type)
                {
                    case FtpFileSystemObjectType.File:
                    case FtpFileSystemObjectType.Link:
                        _logger.Log($"Size={_baseFtpCommand.FtpCall(c => c.GetFileSize(item.FullName))}");
                        _baseFtpCommand.FtpCall(c => c.DeleteFile(item.FullName));
                        break;
                    case FtpFileSystemObjectType.Directory:
                        Clean(item.FullName);
                        _baseFtpCommand.FtpCall(c => c.DeleteDirectory(item.FullName));
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
                _baseFtpCommand.FtpCall(c => c.CreateDirectory(targetPath));
                PushDirectory(dir, targetPath);
            }

            var files = Directory.GetFiles(source);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var targetPath = $"{target}/{name}";
                _logger.Log("Upload: " + targetPath);
                _logger.Log($"Size={new FileInfo(file).Length}");
                _logger.Log($"MD5={GetFileMd5(file)}");
                _baseFtpCommand.FtpCall(c => c.UploadFile(file, targetPath));
            }
        }

        private string GetFileMd5(string path)
        {
            using(var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
