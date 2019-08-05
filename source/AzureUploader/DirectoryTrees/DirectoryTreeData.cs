using AzureUploader.Checksums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTreeData
    {
        private readonly List<DirectoryTreeData> _directories = new List<DirectoryTreeData>();
        private readonly List<DirectoryTreeFileData> _files = new List<DirectoryTreeFileData>();

        internal IChecksumProvider ChecksumProvider { get; }

        public IEnumerable<DirectoryTreeData> Directories => _directories;
        public IEnumerable<DirectoryTreeFileData> Files => _files;
        public string Level { get; }
        public string Name { get; }
        public DirectoryTreeData Parent { get; }
        public string Path => string.Concat(Parent?.Path, Parent?.Path is null ? "" : "/", Name);
        public ItemStatus Status { get; set; } = ItemStatus.ItemPresent;

        public DirectoryTreeData(DirectoryTreeData parent, string name, IChecksumProvider checksumProvider) =>
            (Parent, Name, Level, ChecksumProvider) = (parent, name, " " + parent?.Level, checksumProvider);

        public DirectoryTreeData AddDirectory(string name)
        {
            var dir = new DirectoryTreeData(this, name, ChecksumProvider);
            _directories.Add(dir);
            return dir;
        }

        public DirectoryTreeFileData AddFile(string name)
        {
            var file = new DirectoryTreeFileData(this, name);
            _files.Add(file);
            return file;
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"{Level}{Name}/");

            foreach(var file in Files)
            {
                sb.Append(Environment.NewLine);
                sb.Append(Level);
                sb.Append(" ");
                sb.Append(file.Name);
                if (file.Status != ItemStatus.ItemPresent)
                {
                    sb.Append(" (");
                    sb.Append(file.Status);
                    sb.Append(")");
                }
            }

            foreach (var directory in Directories)
            {
                sb.Append(Environment.NewLine);
                sb.Append(directory.ToString());
            }

            return sb.ToString();
        }
    }
}
