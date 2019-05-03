using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;

namespace AzureUploader
{
    public class AzureFtpUploader
    {
        private const string RootDirectory = "/site/wwwroot";
        private readonly Func<FtpClient> _clientFactory;

        private FtpClient _client;
        private ILogger _logger;

        private FtpClient Client => _client ?? (_client = Connect());

        public AzureFtpUploader(Func<FtpClient> clientFactory, ILogger logger = null) => (_clientFactory, _logger) = (clientFactory, logger);

        public void Deploy(string directory)
        {
            EnsurePublishDirectoryExists(directory);
            Log("CLEAN FTP");
            Clean();
            Log("PUSH NEW CONTENT");
            Push(directory);
            Log("DEPLOYMENT DONE");
        }

        private void Clean(string path = RootDirectory)
        {
            var items = FtpCall(c => c.GetListing(path));
            foreach (var item in items)
            {
                Log($"Remove {item.Type.ToString().ToLower()}: {item.FullName}");
                switch (item.Type)
                {
                    case FtpFileSystemObjectType.File:
                    case FtpFileSystemObjectType.Link:
                        FtpCall(c => c.DeleteFile(item.FullName));
                        break;
                    case FtpFileSystemObjectType.Directory:
                        Clean(item.FullName);
                        FtpCall(c => c.DeleteDirectory(item.FullName));
                        break;
                }
            }
        }

        private FtpClient Connect()
        {
            var client = _clientFactory();
            client.EncryptionMode = FtpEncryptionMode.Explicit;
            client.ReadTimeout = 60_000;
            client.RetryAttempts = 3;
            client.Connect();
            return client;
        }

        private void EnsurePublishDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new Exception("Publish directory doesn't exist: " + directory);
            }
        }

        private void FtpCall(Action<FtpClient> operation) => FtpCall(c => { operation(c); return 0; });

        private T FtpCall<T>(Func<FtpClient, T> operation)
        {
            int time = 3;
            int count = 6;
            while (count > 0)
            {
                try
                {
                    return operation(Client);
                }
                catch (Exception)
                {
                    // Usually Azure FTP needs to stage a new connection
                    _client.Dispose();
                    _client = null;

                    count--;
                    Log("Failed - retry count: " + count);
                    if (count <= 0)
                    {
                        throw;
                    }
                    Log($"Will retry in {time} seconds");
                    Thread.Sleep(time * 1000);
                    time *= 2;
                }
            }

            throw new NotImplementedException();
        }

        private void Log(string message) => _logger?.LogInformation($"{nameof(AzureFtpUploader)}: {message}");

        private void Push(string directory) => PushDirectory(directory, RootDirectory);

        private void PushDirectory(string source, string target)
        {
            var directories = Directory.GetDirectories(source);
            foreach (var dir in directories)
            {
                var name = Path.GetFileName(dir);
                var targetPath = $"{target}/{name}";
                Log("Create: " + targetPath);
                FtpCall(c => c.CreateDirectory(targetPath));
                PushDirectory(dir, targetPath);
            }

            var files = Directory.GetFiles(source);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var targetPath = $"{target}/{name}";
                Log("Upload: " + targetPath);
                FtpCall(c => c.UploadFile(file, targetPath));
            }
        }
    }
}
