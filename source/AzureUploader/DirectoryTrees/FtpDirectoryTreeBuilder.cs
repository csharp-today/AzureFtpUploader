using AzureUploader.Checksums;
using AzureUploader.FtpCommands;
using FluentFTP;

namespace AzureUploader.DirectoryTrees
{
    internal class FtpDirectoryTreeBuilder : IFtpDirectoryTreeBuilder
    {
        private readonly IChecksumProvider _checksumProvider;
        private readonly IFtpContentGetter _ftpContentGetter;

        public FtpDirectoryTreeBuilder(IChecksumProvider checksumProvider, IFtpContentGetter ftpContentGetter) =>
            (_checksumProvider, _ftpContentGetter) = (checksumProvider, ftpContentGetter);

        public DirectoryTree BuildUsingFtpDirectory(string ftpPath)
        {
            var tree = new DirectoryTree(ftpPath, _checksumProvider);
            AddContent(tree, ftpPath);
            return tree;
        }

        private void AddContent(DirectoryTreeData tree, string path)
        {
            foreach (var ftpItem in _ftpContentGetter.GetContent(path))
            {
                switch (ftpItem.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        var subTree = tree.AddDirectory(ftpItem.Name);
                        AddContent(subTree, ftpItem.FullName);
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
