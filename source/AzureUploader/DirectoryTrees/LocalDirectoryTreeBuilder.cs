using AzureUploader.Checksums;
using System.IO;

namespace AzureUploader.DirectoryTrees
{
    internal class LocalDirectoryTreeBuilder : ILocalDirectoryTreeBuilder
    {
        private readonly IChecksumProvider _checksumProvider;

        public LocalDirectoryTreeBuilder(IChecksumProvider checksumProvider) => _checksumProvider = checksumProvider;

        public DirectoryTree BuildUsingLocalDirectory(string localPath)
        {
            var tree = new DirectoryTree(localPath, _checksumProvider);
            AddContent(localPath, tree);
            return tree;
        }

        private void AddContent(string path, DirectoryTreeData tree)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                var subTree = tree.AddDirectory(Path.GetFileName(directory));
                AddContent(directory, subTree);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                var fileData = tree.AddFile(Path.GetFileName(file));
                fileData.Size = new FileInfo(file).Length;
            }
        }
    }
}
