using AzureUploader.Checksums;
using AzureUploader.FtpCommands;
using FluentFTP;

namespace AzureUploader
{
    internal class FtpManager : IFtpManager
    {
        private readonly IFtpContentGetter _ftpContentGetter;
        private readonly IFtpDirectoryCreator _ftpDirectoryCreator;
        private readonly IFtpDirectoryRemover _ftpDirectoryRemover;
        private readonly IFtpDirectoryUploader _ftpDirectoryUploader;
        private readonly IFtpExistenceChecker _ftpExistenceChecker;
        private readonly IFtpFileRemover _ftpFileRemover;
        private readonly IFtpFileUploader _ftpFileUploader;
        private readonly IFtpTextReader _ftpTextReader;
        private readonly IFtpTextUploader _ftpTextUploader;

        public FtpManager(IChecksumCalculator checksumCalculator, ChecksumDataStorage checksumDataStorage , IFtpClientProvider ftpClientProvider, IClassLogger logger)
        {
            var ftpExecutor = new FtpCommandExecutor(ftpClientProvider, logger);
            _ftpContentGetter = new FtpContentGetter(ftpExecutor);
            _ftpDirectoryCreator = new FtpDirectoryCreator(ftpExecutor);
            _ftpDirectoryRemover = new FtpDirectoryRemover(ftpExecutor, _ftpContentGetter, _ftpFileRemover, logger);
            var ftpFileUploader = new FtpFileUploader(checksumCalculator, ftpExecutor, logger, checksumDataStorage);
            _ftpDirectoryUploader = new FtpDirectoryUploader(_ftpDirectoryCreator, ftpFileUploader, logger);
            _ftpExistenceChecker = new FtpExistenceChecker(ftpExecutor);
            _ftpFileRemover = new FtpFileRemover(ftpExecutor, logger);
            _ftpFileUploader = new FtpFileUploader(checksumCalculator, ftpExecutor, logger, checksumDataStorage);
            _ftpTextReader = new FtpTextReader(ftpExecutor);
            _ftpTextUploader = new FtpTextUploader(ftpFileUploader);
        }

        public void CleanDirectory(string path) => _ftpDirectoryRemover.CleanDirectory(path);
        public void CreateDirectory(string path) => _ftpDirectoryCreator.CreateDirectory(path);
        public bool FileExist(string path) => _ftpExistenceChecker.FileExist(path);
        public FtpListItem[] GetContent(string path) => _ftpContentGetter.GetContent(path);
        public string ReadText(string path) => _ftpTextReader.ReadText(path);
        public void RemoveDirectory(string path) => _ftpDirectoryRemover.RemoveDirectory(path);
        public void RemoveFile(string path) => _ftpFileRemover.RemoveFile(path);
        public void UploadDirectory(string directoryPath, string targetPath) => _ftpDirectoryUploader.UploadDirectory(directoryPath, targetPath);
        public void UploadFile(string filePath, string targetPath) => _ftpFileUploader.UploadFile(filePath, targetPath);
        public void UploadText(string text, string targetPath) => _ftpTextUploader.UploadText(text, targetPath);
    }
}
