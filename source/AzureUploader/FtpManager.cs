using AzureUploader.Checksums;
using AzureUploader.FtpCommands;
using FluentFTP;

namespace AzureUploader
{
    internal class FtpManager : IFtpManager
    {
        private readonly IFtpContentGetter _ftpContentGetter;
        private readonly IFtpDirectoryRemover _ftpDirectoryRemover;
        private readonly IFtpDirectoryUploader _ftpDirectoryUploader;
        private readonly IFtpFileRemover _ftpFileRemover;
        private readonly IFtpTextReader _ftpTextReader;
        private readonly IFtpTextUploader _ftpTextUploader;

        public FtpManager(IChecksumCalculator checksumCalculator, ChecksumDataStorage checksumDataStorage , IFtpClientProvider ftpClientProvider, IClassLogger logger)
        {
            var ftpExecutor = new FtpCommandExecutor(ftpClientProvider, logger);
            _ftpFileRemover = new FtpFileRemover(ftpExecutor, logger);
            _ftpContentGetter = new FtpContentGetter(ftpExecutor);
            _ftpDirectoryRemover = new FtpDirectoryRemover(ftpExecutor, _ftpContentGetter, _ftpFileRemover, logger);
            var ftpFileUploader = new FtpFileUploader(checksumCalculator, ftpExecutor, logger, checksumDataStorage);
            _ftpDirectoryUploader = new FtpDirectoryUploader(ftpExecutor, ftpFileUploader, logger);
            _ftpTextReader = new FtpTextReader(ftpExecutor);
            _ftpTextUploader = new FtpTextUploader(ftpFileUploader);
        }

        public void CleanDirectory(string path) => _ftpDirectoryRemover.CleanDirectory(path);
        public FtpListItem[] GetContent(string path) => _ftpContentGetter.GetContent(path);
        public string ReadText(string path) => _ftpTextReader.ReadText(path);
        public void RemoveDirectory(string path) => _ftpDirectoryRemover.RemoveDirectory(path);
        public void RemoveFile(string path) => _ftpFileRemover.RemoveFile(path);
        public void UploadDirectory(string directoryPath, string targetPath) => _ftpDirectoryUploader.UploadDirectory(directoryPath, targetPath);
        public void UploadText(string text, string targetPath) => _ftpTextUploader.UploadText(text, targetPath);
    }
}
