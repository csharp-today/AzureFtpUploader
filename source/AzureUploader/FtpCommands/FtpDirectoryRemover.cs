using FluentFTP;

namespace AzureUploader.FtpCommands
{
    internal class FtpDirectoryRemover : IFtpDirectoryRemover
    {
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IFtpFileRemover _ftpFileRemover;
        private readonly IClassLogger _logger;

        public FtpDirectoryRemover(IFtpCommandExecutor ftpExecutor, IFtpFileRemover ftpFileRemover, IClassLogger logger) =>
            (_ftpExecutor, _ftpFileRemover, _logger) = (ftpExecutor, ftpFileRemover, logger);

        public void RemoveDirectory(string path)
        {
            var items = _ftpExecutor.Execute(c => c.GetListing(path));
            foreach (var item in items)
            {
                _logger.Log($"Remove {item.Type.ToString().ToLower()}: {item.FullName}");
                switch (item.Type)
                {
                    case FtpFileSystemObjectType.File:
                    case FtpFileSystemObjectType.Link:
                        _ftpFileRemover.RemoveFile(item.FullName);
                        break;
                    case FtpFileSystemObjectType.Directory:
                        RemoveDirectory(item.FullName);
                        _ftpExecutor.Execute(c => c.DeleteDirectory(item.FullName));
                        break;
                }
            }
        }
    }
}
