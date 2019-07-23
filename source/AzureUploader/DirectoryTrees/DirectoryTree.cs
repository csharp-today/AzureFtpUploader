﻿using System;

namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTree : DirectoryTreeData
    {
        public DirectoryTree(string path) : base(null, path) { }

        public override string ToString() => $"Directory tree:{Environment.NewLine}{Path}{Environment.NewLine}{base.ToString()}";
    }
}
