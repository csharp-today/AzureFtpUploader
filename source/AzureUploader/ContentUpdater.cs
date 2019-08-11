using AzureUploader.Checksums;
using AzureUploader.DirectoryTrees;

namespace AzureUploader
{
    internal class ContentUpdater : IContentUpdater
    {
        private readonly ChecksumDataStorage _checksumDataStorage;
        private readonly IFtpManager _ftpManager;
        private readonly IClassLogger _logger;

        public ContentUpdater(ChecksumDataStorage checksumDataStorage, IFtpManager ftpManager, IClassLogger logger) =>
            (_checksumDataStorage, _ftpManager, _logger) = (checksumDataStorage, ftpManager, logger);

        public void UpdateContent(DirectoryTree differenceTree, string localPath, string ftpPath, string checksumPath)
        {
            _logger.Log("Clean FTP");
            _ftpManager.CleanDirectory(ftpPath);
            _ftpManager.RemoveFile(checksumPath);
            _checksumDataStorage.Clear();

            _logger.Log("Push new content");
            _ftpManager.UploadDirectory(localPath, ftpPath);

            _logger.Log("Upload checksums");
            _ftpManager.UploadText(_checksumDataStorage.GetStorageDump(), checksumPath);
        }
    }
}
