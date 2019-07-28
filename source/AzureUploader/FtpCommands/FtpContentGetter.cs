using FluentFTP;

namespace AzureUploader.FtpCommands
{
    internal class FtpContentGetter : IFtpContentGetter
    {
        private readonly IFtpCommandExecutor _ftpExecutor;

        public FtpContentGetter(IFtpCommandExecutor ftpExecutor) => _ftpExecutor = ftpExecutor;

        public FtpListItem[] GetContent(string path) => _ftpExecutor.Execute(c => c.GetListing(path));
    }
}
