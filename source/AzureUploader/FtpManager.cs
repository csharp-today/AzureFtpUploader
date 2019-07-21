using AzureUploader.Checksums;
using AzureUploader.FtpCommands;
using FluentFTP;
using Microsoft.Extensions.Logging;
using System;

namespace AzureUploader
{
    internal class FtpManager : IFtpManager
    {
        private readonly IFtpDirectoryRemover _ftpDirectoryRemover;
        private readonly IFtpDirectoryUploader _ftpDirectoryUploader;
        private readonly IFtpFileRemover _ftpFileRemover;
        private readonly IFtpTextReader _ftpTextReader;
        private readonly IFtpTextUploader _ftpTextUploader;

        public ChecksumDataStorage ChecksumDataStorage { get; } = new ChecksumDataStorage();
        public IClassLogger Logger { get; private set; }

        public FtpManager(Func<FtpClient> clientFactory, ILogger logger)
        {
            Logger = new ClassLogger<AzureFtpUploader>(logger);
            var ftpExecutor = new FtpCommandExecutor(new FtpClientProvider(clientFactory), Logger);
            _ftpFileRemover = new FtpFileRemover(ftpExecutor, Logger);
            _ftpDirectoryRemover = new FtpDirectoryRemover(ftpExecutor, _ftpFileRemover, Logger);
            var ftpFileUploader = new FtpFileUploader(new Md5Calculator(), ftpExecutor, Logger, ChecksumDataStorage);
            _ftpDirectoryUploader = new FtpDirectoryUploader(ftpExecutor, ftpFileUploader, Logger);
            _ftpTextReader = new FtpTextReader(ftpExecutor);
            _ftpTextUploader = new FtpTextUploader(ftpFileUploader);
        }

        public string ReadText(string path) => _ftpTextReader.ReadText(path);
        public void RemoveDirectory(string path) => _ftpDirectoryRemover.RemoveDirectory(path);
        public void RemoveFile(string path) => _ftpFileRemover.RemoveFile(path);
        public void UploadDirectory(string directoryPath, string targetPath) => _ftpDirectoryUploader.UploadDirectory(directoryPath, targetPath);
        public void UploadText(string text, string targetPath) => _ftpTextUploader.UploadText(text, targetPath);
    }
}
