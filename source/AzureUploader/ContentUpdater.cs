using AzureUploader.Checksums;
using AzureUploader.DirectoryTrees;
using System.Linq;

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
            UpdateDirectory(differenceTree);

            _logger.Log("Upload checksums");
            _ftpManager.RemoveFile(checksumPath);
            _ftpManager.UploadText(_checksumDataStorage.GetStorageDump(), checksumPath);
        }

        private void UpdateDirectory(DirectoryTreeData directory)
        {
            _logger.Log($"SYNCING {directory.Path}");
            _logger.Log($"Status: {directory.Status}");
            UpdateFiles(directory);

            var theSame = GetByStatus(ItemStatus.ItemAlreadyPresent);
            if (theSame.Length > 0)
            {
                _logger.Log("Directories already up-to-date:");
                foreach (var dir in theSame)
                {
                    Log(dir);
                }
            }

            var toRemove = GetByStatus(ItemStatus.ItemToRemove);
            if (toRemove.Length > 0)
            {
                _logger.Log("Directories for removal:");
                foreach (var dir in toRemove)
                {
                    Log(dir);
                    _ftpManager.RemoveDirectory(dir.Target.Path);
                }
            }

            var toCopy = GetByStatus(ItemStatus.ItemToCopy);
            if (toCopy.Length > 0)
            {
                _logger.Log("Directories to upload:");
                foreach (var dir in toCopy)
                {
                    Log(dir);
                    _ftpManager.CreateDirectory(dir.Target.Path);
                    _ftpManager.UploadDirectory(dir.Path, dir.Target.Path);
                }
            }

            var toUpdate = GetByStatus(ItemStatus.ItemToUpdate);
            if (toUpdate.Length > 0)
            {
                _logger.Log("Directories to update:");
                foreach (var dir in toUpdate)
                {
                    Log(dir);
                    _ftpManager.CreateDirectory(dir.Target.Path);
                    UpdateDirectory(dir);
                }
            }

            DirectoryTreeData[] GetByStatus(ItemStatus status) => directory.Directories.Where(d => d.Status == status).ToArray();
            void Log(DirectoryTreeData dir) => _logger.Log("DIR=> " + dir.Path);
        }

        private void UpdateFiles(DirectoryTreeData directory)
        {
            _logger.Log($"Syncing files in {directory.Path}");

            var theSame = GetByStatus(ItemStatus.ItemAlreadyPresent);
            if (theSame.Length > 0)
            {
                _logger.Log("Files already up-to-date:");
                foreach (var file in theSame)
                {
                    Log(file);
                }
            }

            var toRemove = GetByStatus(ItemStatus.ItemToRemove);
            if (toRemove.Length > 0)
            {
                _logger.Log("Files for removal:");
                foreach (var file in toRemove)
                {
                    Log(file);
                    _ftpManager.RemoveFile(file.Target.FullPath);
                }
            }

            var toCopy = GetByStatus(ItemStatus.ItemToCopy);
            if (toCopy.Length > 0)
            {
                _logger.Log("Files to upload:");
                foreach (var file in toCopy)
                {
                    Log(file);
                    _ftpManager.UploadFile(file.FullPath, file.Target.FullPath);
                }
            }

            var toUpdate = GetByStatus(ItemStatus.ItemToUpdate);
            if (toUpdate.Length > 0)
            {
                _logger.Log("Files to update:");
                foreach (var file in toUpdate)
                {
                    Log(file);
                    _ftpManager.RemoveFile(file.Target.FullPath);
                    _ftpManager.UploadFile(file.FullPath, file.Target.FullPath);
                }
            }

            DirectoryTreeFileData[] GetByStatus(ItemStatus status) => directory.Files.Where(f => f.Status == status).ToArray();
            void Log(DirectoryTreeFileData file) => _logger.Log("====> " + file.Name);
        }
    }
}
