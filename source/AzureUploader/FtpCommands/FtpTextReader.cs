using System.IO;

namespace AzureUploader.FtpCommands
{
    internal class FtpTextReader : IFtpTextReader
    {
        private readonly IFtpCommandExecutor _ftpExecutor;

        public FtpTextReader(IFtpCommandExecutor ftpExecutor) => _ftpExecutor = ftpExecutor;

        public string ReadText(string path)
        {
            var tempFile = Path.GetTempFileName();
            _ftpExecutor.Execute(c => c.DownloadFile(tempFile, path));
            var text = File.ReadAllText(tempFile);
            File.Delete(tempFile);
            return text;
        }
    }
}
