namespace AzureUploader.Checksums
{
    internal class EmptyChecksumProvider : IChecksumProvider
    {
        public string GetChecksumFor(string path) => "";
    }
}
