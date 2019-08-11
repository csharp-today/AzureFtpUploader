using AzureUploader.DirectoryTrees;

namespace AzureUploader
{
    internal interface IContentUpdater
    {
        void UpdateContent(DirectoryTree differenceTree, string localPath, string ftpPath, string checksumPath);
    }
}
