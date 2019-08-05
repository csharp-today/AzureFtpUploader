namespace AzureUploader.Checksums
{
    internal class StorageChecksumProvider : IChecksumProvider
    {
        private readonly ChecksumDataStorage _storage;

        public StorageChecksumProvider(ChecksumDataStorage storage) => _storage = storage;

        public string GetChecksumFor(string path) => _storage.GetChecksum(path);
    }
}
