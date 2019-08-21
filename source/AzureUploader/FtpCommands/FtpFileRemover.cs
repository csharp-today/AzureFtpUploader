namespace AzureUploader.FtpCommands
{
    internal class FtpFileRemover : IFtpFileRemover
    {
        private readonly IFtpCommandExecutor _ftpExecutor;

        public FtpFileRemover(IFtpCommandExecutor ftpExecutor) => _ftpExecutor = ftpExecutor;

        public void RemoveFile(string path) => _ftpExecutor.Execute(c => c.DeleteFile(path), c => !c.FileExists(path));
    }
}
