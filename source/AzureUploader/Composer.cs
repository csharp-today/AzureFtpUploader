using AzureUploader.Checksums;
using AzureUploader.DirectoryTrees;
using AzureUploader.FtpCommands;
using FluentFTP;
using Microsoft.Extensions.Logging;
using System;

namespace AzureUploader
{
    internal class Composer<T>
    {
        private readonly IFtpClientProvider _ftpClientProvider;

        private readonly Lazy<IFtpDirectoryTreeBuilder> _ftpDirectoryTreeBuilder;
        private readonly Lazy<IFtpManager> _ftpManager;
        private readonly Lazy<ILocalDirectoryTreeBuilder> _localDirectoryTreeBuilder;

        public IChecksumCalculator ChecksumCalculator { get; } = new Md5Calculator();
        public ChecksumDataStorage ChecksumDataStorage { get; } = new ChecksumDataStorage();
        public IFtpDirectoryTreeBuilder FtpDirectoryTreeBuilder => _ftpDirectoryTreeBuilder.Value;
        public IFtpManager FtpManager => _ftpManager.Value;
        public ILocalDirectoryTreeBuilder LocalDirectoryTreeBuilder => _localDirectoryTreeBuilder.Value;
        public IClassLogger Logger { get; }
        public ITreeComparer TreeComparer { get; } = new TreeComparer();

        public Composer(Func<FtpClient> clientFactory, ILogger logger)
        {
            _ftpClientProvider = new FtpClientProvider(clientFactory);
            Logger = new ClassLogger<T>(logger);

            _ftpDirectoryTreeBuilder = new Lazy<IFtpDirectoryTreeBuilder>(() =>
                new FtpDirectoryTreeBuilder(new StorageChecksumProvider(ChecksumDataStorage), FtpManager));
            _ftpManager = new Lazy<IFtpManager>(() =>
                new FtpManager(ChecksumCalculator, ChecksumDataStorage, _ftpClientProvider, Logger));
            _localDirectoryTreeBuilder = new Lazy<ILocalDirectoryTreeBuilder>(() =>
                new LocalDirectoryTreeBuilder(new LocalChecksumProvider(ChecksumCalculator)));
        }
    }
}
