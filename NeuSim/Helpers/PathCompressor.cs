namespace NeuSim.Helpers
{
    using System.Collections.Generic;
    using System.IO.Compression;

    internal class PathCompressor
    {
        private readonly string path;

        private readonly string name;

        private readonly string savePath;

        private readonly HashSet<string> ignoredExtensions;

        public PathCompressor(string path, string name, string savePath)
        {
            this.path = path;
            this.name = name;
            this.savePath = savePath;

            this.ignoredExtensions = new HashSet<string>();
        }

        public void IgnoreExts(params string[] exts)
        {
            foreach (var ext in exts)
            {
                this.ignoredExtensions.Add(ext);
            }
        }
    }
}
