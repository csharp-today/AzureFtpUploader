namespace AzureUploader.DirectoryTrees
{
    internal interface IFtpDirectoryTreeBuilder
    {
        DirectoryTree BuildUsingFtpDirectory(IFtpManager ftpManager, string ftpPath);
    }
}
