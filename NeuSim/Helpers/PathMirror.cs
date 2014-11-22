namespace NeuSim.Helpers
{
    using NeuSim.Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class PathMirror
    {
        private readonly string storageRoot;

        private readonly string pathInfoFileName;

        private readonly string[] excludedPaths;

        public PathMirror(string storageRoot, string pathInfoFileName, params string[] excludedPaths)
        {
            this.storageRoot = storageRoot;
            this.excludedPaths = excludedPaths;
            this.pathInfoFileName = pathInfoFileName;
        }

        public void VisitSingle<T>(string root, string subDirectory, Action<T, IEnumerable<FileInfo>> fileAction) where T : new()
        {
            if (!IsSubDir(root, subDirectory))
            {
                throw new ArgumentException("SubDirectory is not subdirectory of root.");
            }

            this.VisitAll(new DirectoryInfo(root), new DirectoryInfo(subDirectory), fileAction,
                                   continueRecursive: false);
        }

        public void VisitAll<T>(string path, Action<T, IEnumerable<FileInfo>> fileAction) where T : new()
        {
            this.EnsureMirrorStoragePath();

            var startDirectory = new DirectoryInfo(path);
            if (!startDirectory.Exists)
            {
                throw new InvalidOperationException("Start path doesn't exist.");
            }

            var rootDirInfo = new DirectoryInfo(path);

            this.VisitAll(rootDirInfo, rootDirInfo, fileAction, continueRecursive: true);
        }

        private static bool IsSubDir(string rootPath, string childPath)
        {
            return Path.GetFullPath(childPath).StartsWith(Path.GetFullPath(rootPath));
        }

        private void VisitAll<T>(FileSystemInfo rootPath, DirectoryInfo currentDirectory,
                                          Action<T, IEnumerable<FileInfo>> fileAction, bool continueRecursive) where T : new()
        {
            if (excludedPaths.Any(excludedPath => IsSubDir(excludedPath, currentDirectory.FullName)))
            {
                // omit exlucded path
                return;
            }

            var storageFileInfoPath = this.GetStorageInfoFile(rootPath, currentDirectory);
            var storageFIleInfo = this.RetreiveStorageFileInfoOrDefault<T>(storageFileInfoPath);

            var allFiles = currentDirectory.GetFiles();

            fileAction(storageFIleInfo, allFiles);
            this.StoreFileInfo(storageFileInfoPath, storageFIleInfo);

            if (!continueRecursive)
            {
                return;
            }

            foreach (var subDir in currentDirectory.GetDirectories())
            {
                this.VisitAll(rootPath, subDir, fileAction, true);
            }
        }

        private void StoreFileInfo<T>(string fileName, T fileStorageInfo) where T : new()
        {
            fileName.SerializeToPath(fileStorageInfo);
        }

        private T RetreiveStorageFileInfoOrDefault<T>(string fileName) where T : new()
        {
            return !File.Exists(fileName) ? new T() : fileName.DeserializeFromPath<T>();
        }

        private string GetStorageInfoFile(FileSystemInfo rootPath, FileSystemInfo currentDirectory)
        {
            return Path.Combine(this.BuildMirrorPath(rootPath, currentDirectory), this.pathInfoFileName);
        }

        private string BuildMirrorPath(FileSystemInfo rootPath, FileSystemInfo currentDirectory)
        {
            if (!currentDirectory.FullName.StartsWith(rootPath.FullName))
            {
                throw new ArgumentException("Current directory is not sub-directory of root.");
            }

            var rootUri = new Uri(rootPath.FullName);
            var relative = rootUri.MakeRelativeUri(new Uri(currentDirectory.FullName));

            return Path.Combine(this.storageRoot, relative.OriginalString);
        }

        private void EnsureMirrorStoragePath()
        {
            var directoryInfo = new DirectoryInfo(this.storageRoot);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(this.storageRoot);
            }
        }
    }
}
