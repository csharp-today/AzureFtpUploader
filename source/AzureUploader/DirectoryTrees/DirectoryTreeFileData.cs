namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTreeFileData
    {
        public string Name { get; }
        public DirectoryTreeData Parent { get; }
        public ItemStatus Status { get; set; } = ItemStatus.ItemPresent;

        public DirectoryTreeFileData(DirectoryTreeData parent, string name) =>
            (Name, Parent) = (name, parent);
    }
}
