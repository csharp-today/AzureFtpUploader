namespace AzureUploader.Checksums
{
    internal class LocalChecksumProvider : IChecksumProvider
    {
        private readonly IChecksumCalculator _checksumCalculator;

        public LocalChecksumProvider(IChecksumCalculator checksumCalculator) => _checksumCalculator = checksumCalculator;

        public string GetChecksumFor(string path) => _checksumCalculator.CalculateChecksum(path);
    }
}
