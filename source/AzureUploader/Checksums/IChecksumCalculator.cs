namespace AzureUploader.Checksums
{
    internal interface IChecksumCalculator
    {
        string CalculateChecksum(string filePath);
    }
}
