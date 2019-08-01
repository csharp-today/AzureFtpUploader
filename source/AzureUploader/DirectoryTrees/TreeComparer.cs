using System.Collections.Generic;
using System.Linq;

namespace AzureUploader.DirectoryTrees
{
    internal class TreeComparer : ITreeComparer
    {
        public DirectoryTree Compare(DirectoryTree source, DirectoryTree target)
        {
            var diffTree = new DirectoryTree(source.Path);
            CompareDirectory(source, target, diffTree);
            return diffTree;
        }

        private void CompareDirectory(DirectoryTreeData source, DirectoryTreeData target, DirectoryTreeData diff)
        {
            var statuses = new List<ItemStatus>();
            foreach (var sourceFile in source?.Files ?? Enumerable.Empty<DirectoryTreeFileData>())
            {
                var diffFile = diff.AddFile(sourceFile.Name);
                diffFile.Status = target is null || target.Files.Any(f => f.Name == sourceFile.Name)
                    ? ItemStatus.ItemPresentInSourceAndTarget
                    : ItemStatus.ItemToCopy;
                AddUniqueStatus(diffFile.Status);
            }

            foreach (var targetFile in target?.Files ?? Enumerable.Empty<DirectoryTreeFileData>())
            {
                if (source is null || !source.Files.Any(f => f.Name == targetFile.Name))
                {
                    var diffFile = diff.AddFile(targetFile.Name);
                    diffFile.Status = ItemStatus.ItemToRemove;
                    AddUniqueStatus(diffFile.Status);
                }
            }

            foreach (var sourceDirectory in source?.Directories ?? Enumerable.Empty<DirectoryTreeData>())
            {
                var diffDirectory = diff.AddDirectory(sourceDirectory.Name);
                CompareDirectory(sourceDirectory, target?.Directories?.FirstOrDefault(d => d.Name == sourceDirectory.Name), diffDirectory);
                AddUniqueStatus(diffDirectory.Status);
            }

            foreach (var targetDirectory in target?.Directories ?? Enumerable.Empty<DirectoryTreeData>())
            {
                if (source is null || !source.Directories.Any(d => d.Name == targetDirectory.Name))
                {
                    var diffDirectory = diff.AddDirectory(targetDirectory.Name);
                    CompareDirectory(null, targetDirectory, diffDirectory);
                    AddUniqueStatus(diffDirectory.Status);
                }
            }

            diff.Status = statuses.Count == 0 ? statuses[0] : ItemStatus.ItemPresentInSourceAndTarget;

            void AddUniqueStatus(ItemStatus status)
            {
                if (!statuses.Contains(status))
                {
                    statuses.Add(status);
                }
            }
        }
    }
}
