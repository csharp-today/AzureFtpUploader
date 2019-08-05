using System;
using System.IO;
using System.Security.Cryptography;

namespace AzureUploader.Checksums
{
    internal class Md5Calculator : IChecksumCalculator
    {
        public string CalculateChecksum(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath.Replace('/', '\\')))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
