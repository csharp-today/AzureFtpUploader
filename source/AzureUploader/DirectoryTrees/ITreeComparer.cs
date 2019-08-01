namespace AzureUploader.DirectoryTrees
{
    internal interface ITreeComparer
    {
        DirectoryTree Compare(DirectoryTree source, DirectoryTree target);
    }
}
