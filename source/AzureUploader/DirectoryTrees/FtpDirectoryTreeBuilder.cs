﻿using AzureUploader.Checksums;
using FluentFTP;

namespace AzureUploader.DirectoryTrees
{
    internal class FtpDirectoryTreeBuilder : IFtpDirectoryTreeBuilder
    {
        private readonly IChecksumProvider _checksumProvider;

        public FtpDirectoryTreeBuilder(IChecksumProvider checksumProvider) => _checksumProvider = checksumProvider;

        public DirectoryTree BuildUsingFtpDirectory(IFtpManager ftpManager, string ftpPath)
        {
            var tree = new DirectoryTree(ftpPath, _checksumProvider);
            AddContent(tree, ftpManager, ftpPath);
            return tree;
        }

        private void AddContent(DirectoryTreeData tree, IFtpManager ftpManager, string path)
        {
            foreach (var ftpItem in ftpManager.GetContent(path))
            {
                switch (ftpItem.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        var subTree = tree.AddDirectory(ftpItem.Name);
                        AddContent(subTree, ftpManager, ftpItem.FullName);
                        break;
                    case FtpFileSystemObjectType.File:
                    case FtpFileSystemObjectType.Link:
                        var fileData = tree.AddFile(ftpItem.Name);
                        fileData.Size = ftpItem.Size;
                        break;
                }
            }
        }
    }
}
