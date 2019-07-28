using FluentFTP;

namespace AzureUploader.FtpCommands
{
    internal class FtpDirectoryRemover : IFtpDirectoryRemover
    {
        private readonly IFtpCommandExecutor _ftpExecutor;
        private readonly IFtpContentGetter _ftpContentGetter;
        private readonly IFtpFileRemover _ftpFileRemover;
        private readonly IClassLogger _logger;

        public FtpDirectoryRemover(IFtpCommandExecutor ftpExecutor, IFtpContentGetter ftpContentGetter, IFtpFileRemover ftpFileRemover, IClassLogger logger) =>
            (_ftpExecutor, _ftpContentGetter, _ftpFileRemover, _logger) = (ftpExecutor, ftpContentGetter, ftpFileRemover, logger);

        public void RemoveDirectory(string path)
        {
            foreach (var item in _ftpContentGetter.GetContent(path))
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
