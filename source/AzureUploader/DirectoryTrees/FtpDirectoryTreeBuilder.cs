namespace AzureUploader.DirectoryTrees
{
    internal class FtpDirectoryTreeBuilder : IFtpDirectoryTreeBuilder
    {
        public DirectoryTree BuildUsingFtpDirectory(string ftpPath)
        {
            var tree = new DirectoryTree(ftpPath);
            return tree;
        }
    }
}
