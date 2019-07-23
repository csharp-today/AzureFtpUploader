namespace AzureUploader.DirectoryTrees
{
    internal interface IDirectoryTreeBuilder
    {
        DirectoryTree BuildUsingLocalDirectory(string localPath, string overridePathInTree);
    }
}
