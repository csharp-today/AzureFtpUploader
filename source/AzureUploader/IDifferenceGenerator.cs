using AzureUploader.DirectoryTrees;

namespace AzureUploader
{
    internal interface IDifferenceGenerator
    {
        DirectoryTree GenerateDifferenceTree(string localPath, string ftpPath, string checksumFilePath);
    }
}
