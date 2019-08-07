namespace AzureUploader.DirectoryTrees
{
    internal interface IFtpDirectoryTreeBuilder
    {
        DirectoryTree BuildUsingFtpDirectory(string ftpPath);
    }
}
