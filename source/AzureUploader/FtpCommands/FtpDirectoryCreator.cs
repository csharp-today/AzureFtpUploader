namespace AzureUploader.FtpCommands
{
    internal class FtpDirectoryCreator : IFtpDirectoryCreator
    {
        private readonly IFtpCommandExecutor _ftpExecutor;

        public FtpDirectoryCreator(IFtpCommandExecutor ftpExecutor) => _ftpExecutor = ftpExecutor;

        public void CreateDirectory(string path) => _ftpExecutor.Execute(c => c.CreateDirectory(path), c => c.DirectoryExists(path));
    }
}
