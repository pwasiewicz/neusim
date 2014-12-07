namespace NeuSim.Compressing
{
    using Ionic.Zip;
    using NeuSim.Common.Services;
    using NeuSim.Compressing.Helpers;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

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

        public void UncompressFilder(string package, string outputPath)
        {
            using (var zip = new ZipFile(outputPath))
            {
                zip.ExtractAll(package, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        private void AddFiles(ZipFile zipFile, DirectoryInfo current, DirectoryInfo root,
                              string[] excludedExtensions)
        {
            var rootPath = root.FullName.AddSeparatorIfNeeded();
            foreach (
                var file in
                    current.GetFiles().Where(file => excludedExtensions.All(ext => !file.Extension.EndsWith(ext)))
                )
            {
                Debug.Assert(file.Directory != null, "file.Directory != null");

                var dirName = file.Directory.FullName.AddSeparatorIfNeeded();
                var relativeDir = PathHelpers.PathDifference(rootPath, dirName);

                zipFile.AddFile(file.FullName, relativeDir);
            }

            foreach (var directoryInfo in current.GetDirectories())
            {
                this.AddFiles(zipFile, directoryInfo, root, excludedExtensions);
            }
        }
    }
}
