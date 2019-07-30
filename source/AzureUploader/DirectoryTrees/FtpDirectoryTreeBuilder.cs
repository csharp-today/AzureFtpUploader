using FluentFTP;

namespace AzureUploader.DirectoryTrees
{
    internal class FtpDirectoryTreeBuilder : IFtpDirectoryTreeBuilder
    {
        public DirectoryTree BuildUsingFtpDirectory(IFtpManager ftpManager, string ftpPath)
        {
            var tree = new DirectoryTree(ftpPath);
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
                        tree.AddFile(ftpItem.Name);
                        break;
                }
            }
        }
    }
}
