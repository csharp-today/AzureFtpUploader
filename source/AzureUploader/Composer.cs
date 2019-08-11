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

        private readonly Lazy<IContentUpdater> _contentUpdater;
        private readonly Lazy<IDifferenceGenerator> _differenceGenerator;
        private readonly Lazy<IFtpDirectoryTreeBuilder> _ftpDirectoryTreeBuilder;
        private readonly Lazy<IFtpManager> _ftpManager;
        private readonly Lazy<ILocalDirectoryTreeBuilder> _localDirectoryTreeBuilder;

        public IContentUpdater ContentUpdater => _contentUpdater.Value;
        public IDifferenceGenerator DifferenceGenerator => _differenceGenerator.Value;
        public IClassLogger Logger { get; }

        private IChecksumCalculator ChecksumCalculator { get; } = new Md5Calculator();
        private ChecksumDataStorage ChecksumDataStorage { get; } = new ChecksumDataStorage();
        private IFtpDirectoryTreeBuilder FtpDirectoryTreeBuilder => _ftpDirectoryTreeBuilder.Value;
        private IFtpManager FtpManager => _ftpManager.Value;
        private ILocalDirectoryTreeBuilder LocalDirectoryTreeBuilder => _localDirectoryTreeBuilder.Value;
        private ITreeComparer TreeComparer { get; } = new TreeComparer();

        public Composer(Func<FtpClient> clientFactory, ILogger logger)
        {
            _ftpClientProvider = new FtpClientProvider(clientFactory);
            Logger = new ClassLogger<T>(logger);

            _contentUpdater = new Lazy<IContentUpdater>(() =>
                new ContentUpdater(ChecksumDataStorage, FtpManager, Logger));
            _differenceGenerator = new Lazy<IDifferenceGenerator>(() =>
                new DifferenceGenerator(ChecksumDataStorage, FtpDirectoryTreeBuilder, FtpManager, LocalDirectoryTreeBuilder, Logger, TreeComparer));
            _ftpDirectoryTreeBuilder = new Lazy<IFtpDirectoryTreeBuilder>(() =>
                new FtpDirectoryTreeBuilder(new StorageChecksumProvider(ChecksumDataStorage), FtpManager));
            _ftpManager = new Lazy<IFtpManager>(() =>
                new FtpManager(ChecksumCalculator, ChecksumDataStorage, _ftpClientProvider, Logger));
            _localDirectoryTreeBuilder = new Lazy<ILocalDirectoryTreeBuilder>(() =>
                new LocalDirectoryTreeBuilder(new LocalChecksumProvider(ChecksumCalculator)));
        }
    }
}
