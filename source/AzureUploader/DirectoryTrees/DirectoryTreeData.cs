using System;
using System.Collections.Generic;
using System.Text;

namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTreeData
    {
        private readonly List<DirectoryTreeData> _directories = new List<DirectoryTreeData>();

        public IEnumerable<DirectoryTreeData> Directories => _directories;
        public string Level { get; }
        public string Name { get; }
        public DirectoryTreeData Parent { get; }
        public string Path => string.Concat(Parent?.Path, Parent?.Path is null ? "" : "/", Name);

        public DirectoryTreeData(DirectoryTreeData parent, string name) =>
            (Parent, Name, Level) = (parent, name, " " + parent?.Level);

        public DirectoryTreeData AddDirectory(string name)
        {
            var dir = new DirectoryTreeData(this, name);
            _directories.Add(dir);
            return dir;
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"{Level}{Name}/");
            foreach(var directory in Directories)
            {
                sb.Append(Environment.NewLine);
                sb.Append(directory.ToString());
            }

            return sb.ToString();
        }
    }
}
