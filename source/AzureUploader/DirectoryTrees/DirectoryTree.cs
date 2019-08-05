using AzureUploader.Checksums;
using System;

namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTree : DirectoryTreeData
    {
        public DirectoryTree(string path, IChecksumProvider checksumProvider = null) : base(null, path, checksumProvider) { }

        public override string ToString() => $"Directory tree: {Path}{Environment.NewLine}{base.ToString()}";
    }
}
