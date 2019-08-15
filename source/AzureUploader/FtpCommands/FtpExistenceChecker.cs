namespace AzureUploader.FtpCommands
{
    internal class FtpExistenceChecker : IFtpExistenceChecker
    {
        private readonly IFtpCommandExecutor _ftpExecutor;

        public FtpExistenceChecker(IFtpCommandExecutor ftpExecutor) => _ftpExecutor = ftpExecutor;

        public bool FileExist(string path) => _ftpExecutor.Execute(c => c.FileExists(path));
    }
}
