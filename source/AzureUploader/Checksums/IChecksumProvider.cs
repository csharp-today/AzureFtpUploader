namespace AzureUploader.Checksums
{
    internal interface IChecksumProvider
    {
        string GetChecksumFor(string path);
    }
}
