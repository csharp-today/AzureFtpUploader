﻿using System.Collections.Generic;
using System.Linq;

namespace AzureUploader.DirectoryTrees
{
    internal class TreeComparer : ITreeComparer
    {
        public DirectoryTree Compare(DirectoryTree source, DirectoryTree target)
        {
            var diffTree = new DirectoryTree(source.Path);
            diffTree.Target = target;
            CompareDirectory(source, diffTree);
            return diffTree;
        }

        private void CompareDirectory(DirectoryTreeData source, DirectoryTreeData diff)
        {
            var statuses = new List<ItemStatus>();
            foreach (var sourceFile in source?.Files ?? Enumerable.Empty<DirectoryTreeFileData>())
            {
                var diffFile = diff.AddFile(sourceFile.Name);
                diffFile.Source = sourceFile;
                diffFile.Target = diff.Target?.Files?.FirstOrDefault(f => f.Name == sourceFile.Name);
                diffFile.Status = GetFileStatus(diffFile);
                AddUniqueStatus(diffFile.Status);
            }

            foreach (var targetFile in diff.Target?.Files ?? Enumerable.Empty<DirectoryTreeFileData>())
            {
                if (source is null || !source.Files.Any(f => f.Name == targetFile.Name))
                {
                    var diffFile = diff.AddFile(targetFile.Name);
                    diffFile.Target = targetFile;
                    diffFile.Status = GetFileStatus(diffFile);
                    AddUniqueStatus(diffFile.Status);
                }
            }

            foreach (var sourceDirectory in source?.Directories ?? Enumerable.Empty<DirectoryTreeData>())
            {
                var diffDirectory = diff.AddDirectory(sourceDirectory.Name);
                diffDirectory.Target = diff.Target?.Directories?.FirstOrDefault(d => d.Name == sourceDirectory.Name);
                CompareDirectory(sourceDirectory, diffDirectory);
                AddUniqueStatus(diffDirectory.Status);
            }

            foreach (var targetDirectory in diff.Target?.Directories ?? Enumerable.Empty<DirectoryTreeData>())
            {
                if (source is null || !source.Directories.Any(d => d.Name == targetDirectory.Name))
                {
                    var diffDirectory = diff.AddDirectory(targetDirectory.Name);
                    diffDirectory.Target = targetDirectory;
                    CompareDirectory(null, diffDirectory);
                    AddUniqueStatus(diffDirectory.Status);
                }
            }

            diff.Status = statuses.Count == 0 ? statuses[0] : ItemStatus.ItemToUpdate;

            void AddUniqueStatus(ItemStatus status)
            {
                if (!statuses.Contains(status))
                {
                    statuses.Add(status);
                }
            }
        }

        private ItemStatus GetFileStatus(DirectoryTreeFileData fileData)
        {
            if (fileData.Target is null)
            {
                return ItemStatus.ItemToCopy;
            }

            if (fileData.Source is null)
            {
                return ItemStatus.ItemToRemove;
            }

            if (fileData.Source.Size != fileData.Target.Size
                || fileData.Source.Checksum != fileData.Target.Checksum)
            {
                return ItemStatus.ItemToUpdate;
            }

            return ItemStatus.ItemAlreadyPresent;
        }
    }
}
