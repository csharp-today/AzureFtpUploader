namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTreeFileData
    {
        public string Name { get; }
        public DirectoryTreeData Parent { get; }
        public long Size { get; set; }
        public DirectoryTreeFileData Source { get; set; }
        public ItemStatus Status { get; set; } = ItemStatus.ItemPresent;
        public DirectoryTreeFileData Target { get; set; }

        public DirectoryTreeFileData(DirectoryTreeData parent, string name) =>
            (Name, Parent) = (name, parent);
    }
}
