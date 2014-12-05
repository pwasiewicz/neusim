namespace NeuSim.Compressing
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Ionic.Zip;
    using NeuSim.Common.Services;

    public class CompressionService : ICompressionService
    {
        public void CompressFolder(string path, string outputFile, params string[] excludedExtensions)
        {
            using (var zipFile = new ZipFile(outputFile))
            {
                var startDir = new DirectoryInfo(path);

                this.AddFiles(zipFile, startDir, startDir, excludedExtensions);

                zipFile.Save();
            }
        }

        private void AddFiles(ZipFile zipFile, DirectoryInfo current, DirectoryInfo root,
                              string[] excludedExtensions)
        {
            var rootUri = new Uri(root.FullName);
            foreach (
                var file in
                    current.GetFiles().Where(file => excludedExtensions.All(ext => !file.Extension.EndsWith(ext)))
                )
            {
                Debug.Assert(file.Directory != null, "file.Directory != null");

                var dirName = file.Directory.FullName;
                var relativeDir = rootUri.MakeRelativeUri(new Uri(dirName));
                // TODO remove .nai from begging
                zipFile.AddFile(file.FullName, relativeDir.OriginalString);
            }

            foreach (var directoryInfo in current.GetDirectories())
            {
                this.AddFiles(zipFile, directoryInfo, root, excludedExtensions);
            }
        }
    }
}
