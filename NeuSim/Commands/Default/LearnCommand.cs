namespace NeuSim.Commands.Default
{
    using Arguments;
    using Context;
    using Helpers;
    using Newtonsoft.Json;
    using Services;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class LearnCommand : CommandBase<LearnSubOptions>
    {
        private const string StorageFileInfo = "filehashes";

        private readonly IHashCalculator hashCalculator;

        public LearnCommand(SessionContext sessionContext, IHashCalculator hashCalculator)
            : base(sessionContext)
        {
            this.hashCalculator = hashCalculator;
        }

        public override string Name
        {
            get { return "learn"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public string MirrorPath
        {
            get { return Path.Combine(this.SessionContext.ContextDirectory, "storage"); }
        }

        public override void Run(LearnSubOptions options)
        {
            var pathMirror = new PathMirror(this.MirrorPath, StorageFileInfo, this.SessionContext.ContextDirectory);
            if (options.All)
            {
                pathMirror.VisitAll<LearnCasesInfoStorage>(this.SessionContext.WorkingPath, (storage, infos) => this
                                                                                                                    .LearnFromInfoStorage
                                                                                                                    (storage,
                                                                                                                     infos
                                                                                                                         .Where
                                                                                                                         (info
                                                                                                                          =>
                                                                                                                          info
                                                                                                                              .FullName
                                                                                                                              .EndsWith
                                                                                                                              (".learn")),
                                                                                                                     options
                                                                                                                         .Force));
            }

        }

        private void LearnFromInfoStorage(LearnCasesInfoStorage storageFilesInfo, IEnumerable<FileInfo> filesInfo,
                                          bool force)
        {
            if (storageFilesInfo.Files == null)
            {
                storageFilesInfo.Files = new List<LearnCaseInfoStorage>();
            }

            foreach (var fileInfo in filesInfo)
            {
                var fileStorageInfo =
                    storageFilesInfo.Files.FirstOrDefault(storageFileInfo => storageFileInfo.Name == fileInfo.Name);

                using (
                    var currentFileInfoStream = new FileStream(fileInfo.FullName, FileMode.Open))
                {
                    string currentFileInfohHash = null;
                    if (fileStorageInfo != null && !force)
                    {
                        currentFileInfohHash = this.hashCalculator.GetHash(currentFileInfoStream);
                        currentFileInfoStream.Position = 0;

                        if (currentFileInfohHash == fileStorageInfo.Hash)
                        {
                            // case already learnt
                            continue;
                        }
                    }

                    if (currentFileInfohHash == null)
                    {
                        currentFileInfohHash = this.hashCalculator.GetHash(currentFileInfoStream);
                        currentFileInfoStream.Position = 0;
                    }

                    if (fileStorageInfo != null)
                    {
                        fileStorageInfo.Hash = currentFileInfohHash;
                    }
                    else
                    {
                        storageFilesInfo.Files.Add(new LearnCaseInfoStorage
                                                   {
                                                       Hash = currentFileInfohHash,
                                                       Name = fileInfo.Name
                                                   });
                    }

                    this.LearnCase(currentFileInfoStream);
                }
            }
        }

        private void LearnCase(Stream stream)
        {
            LearnCase[] learnCases;
            using (var txtReader = new StreamReader(stream))
            {
                learnCases = JsonConvert.DeserializeObject<LearnCase[]>(txtReader.ReadToEnd());
            }


            var inputs = learnCases.Select(learnCase => learnCase.Input).ToArray();
            var outputs = learnCases.Select(learnCase => learnCase.Output).ToArray();

            this.SessionContext.NeuronNetwork.Train(inputs, outputs);
        }

        private void LearnCase(string inputFile)
        {
            using (var stream = new FileStream(inputFile, FileMode.Open))
            {
                this.LearnCase(stream);
            }
        }
    }

    public class LearnCasesInfoStorage
    {

        public List<LearnCaseInfoStorage> Files { get; set; }
    }

    public class LearnCaseInfoStorage
    {
        public string Name { get; set; }

        public string Hash { get; set; }
    }

    public class LearnCase
    {
        public double Output { get; set; }

        public double[] Input { get; set; }
    }
}
