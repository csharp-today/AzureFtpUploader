using System.IO;

namespace AzureUploader.DirectoryTrees
{
    internal class LocalDirectoryTreeBuilder : ILocalDirectoryTreeBuilder
    {
        public DirectoryTree BuildUsingLocalDirectory(string localPath, string overrideRootInTree = null)
        {
            var tree = new DirectoryTree(overrideRootInTree ?? localPath);
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
                tree.AddFile(Path.GetFileName(file));
            }
        }
    }
}
