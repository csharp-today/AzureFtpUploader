namespace AzureUploader.DirectoryTrees
{
    internal class DirectoryTreeFileData
    {
        public string Name { get; }
        public DirectoryTreeData Parent { get; }

        public DirectoryTreeFileData(DirectoryTreeData parent, string name) =>
            (Name, Parent) = (name, parent);
    }
}
