﻿using FluentFTP;

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
        
        public void CleanDirectory(string path)
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
                        CallFtpRemoveDirectory(item.FullName);
                        break;
                }
            }
        }

        public void RemoveDirectory(string path)
        {
            CleanDirectory(path);
            CallFtpRemoveDirectory(path);
        }

        private void CallFtpRemoveDirectory(string path) =>
            _ftpExecutor.Execute(c => c.DeleteDirectory(path), c => !c.DirectoryExists(path));
    }
}
