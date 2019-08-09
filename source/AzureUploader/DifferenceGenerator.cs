using AzureUploader.Checksums;
using AzureUploader.DirectoryTrees;
using System;
using System.IO;

namespace AzureUploader
{
    internal class DifferenceGenerator : IDifferenceGenerator
    {
        private readonly ChecksumDataStorage _checksumDataStorage;
        private readonly IFtpDirectoryTreeBuilder _ftpDirectoryTreeBuilder;
        private readonly IFtpManager _ftpManager;
        private readonly ILocalDirectoryTreeBuilder _localDirectoryTreeBuilder;
        private readonly IClassLogger _logger;
        private readonly ITreeComparer _treeComparer;

        public DifferenceGenerator(ChecksumDataStorage checksumDataStorage, IFtpDirectoryTreeBuilder ftpDirectoryTreeBuilder, IFtpManager ftpManager, ILocalDirectoryTreeBuilder localDirectoryTreeBuilder, IClassLogger logger, ITreeComparer treeComparer) =>
            (_checksumDataStorage, _ftpDirectoryTreeBuilder, _ftpManager, _localDirectoryTreeBuilder, _logger, _treeComparer) = (checksumDataStorage, ftpDirectoryTreeBuilder, ftpManager, localDirectoryTreeBuilder, logger, treeComparer);

        public DirectoryTree GenerateDifferenceTree(string localPath, string ftpPath, string checksumFilePath)
        {
            _logger.Log("Analyze directory to upload");
            EnsurePublishDirectoryExists(localPath);
            var localTree = _localDirectoryTreeBuilder.BuildUsingLocalDirectory(localPath);

            _logger.Log("Analyze FTP directory");
            var targetTree = _ftpDirectoryTreeBuilder.BuildUsingFtpDirectory(ftpPath);

            _logger.Log("Read FTP checksums");
            _checksumDataStorage.RestoreFromDump(_ftpManager.ReadText(checksumFilePath));

            _logger.Log("Compare source and target");
            return _treeComparer.Compare(localTree, targetTree);
        }

        private void EnsurePublishDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new InvalidOperationException("Publish directory doesn't exist: " + directory);
            }
        }
    }
}
