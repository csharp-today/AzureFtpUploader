namespace AzureUploader.FtpCommands
{
    internal class FtpFileRemover : IFtpFileRemover
    {
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IClassLogger _logger;

        public FtpFileRemover(IFtpCommandExecutor ftpExecutor, IClassLogger logger) =>
            (_ftpExecutor, _logger) = (ftpExecutor, logger);

        public void RemoveFile(string path)
        {
            _logger.Log($"Size={_ftpExecutor.Execute(c => c.GetFileSize(path))}");
            _ftpExecutor.Execute(c => c.DeleteFile(path));
        }
    }
}
