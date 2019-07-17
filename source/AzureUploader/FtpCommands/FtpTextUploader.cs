using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpTextUploader : IFtpTextUploader
    {
        private readonly IFtpFileUploader _fileUploader;

        public FtpTextUploader(IFtpFileUploader fileUploader) => _fileUploader = fileUploader;

        public void UploadText(string text, string targetPath)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, text);
            _fileUploader.UploadFile(tempFile, targetPath);
            File.Delete(tempFile);
        }
    }
}
