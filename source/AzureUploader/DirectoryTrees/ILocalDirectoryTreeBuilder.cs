namespace AzureUploader.DirectoryTrees
{
    internal interface ILocalDirectoryTreeBuilder
    {
        DirectoryTree BuildUsingLocalDirectory(string localPath);
    }
}
