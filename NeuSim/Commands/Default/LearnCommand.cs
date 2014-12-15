namespace NeuSim.Commands.Default
{
    using Arguments;
    using Context;
    using Exceptions;
    using Helpers;
    using Newtonsoft.Json;
    using Services;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class LearnCommand : CommandBase<LearnSubOptions>
    {
        private const string StorageFileInfo = "filehashes";

        private const string LearnCaseExt = ".learn";

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
            if (this.WriteHelp(options))
            {
                return;
            }

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
                                                                                                                              (LearnCaseExt)),
                                                                                                                     options
                                                                                                                         .Force));
            }

            if (options.File != null)
            {
                var filePath = this.SessionContext.RelativeToAbsolute(options.File);
                if (!File.Exists(filePath))
                {
                    throw new LearnFileDoesntExist(this.SessionContext, filePath);
                }

                var fileDirectory = Path.GetDirectoryName(filePath);
                pathMirror.VisitSingle<LearnCasesInfoStorage>(this.SessionContext.WorkingPath, fileDirectory,
                                                              (storage, infos) => this
                                                                                      .LearnFromInfoStorage
                                                                                      (storage,
                                                                                       infos
                                                                                           .Where
                                                                                           (info
                                                                                            =>
                                                                                            info
                                                                                                .FullName
                                                                                                .EndsWith
                                                                                                (LearnCaseExt) &&
                                                                                            info.FullName.Equals(
                                                                                                                 filePath)),
                                                                                       options
                                                                                           .Force));

                this.LearnCase(filePath);
            }


            if (options.Paths != null)
            {
                foreach (var path in options.Paths)
                {
                    pathMirror.VisitSingle<LearnCasesInfoStorage>(this.SessionContext.WorkingPath, path,
                                                                  (storage, infos) => this
                                                                                          .LearnFromInfoStorage
                                                                                          (storage,
                                                                                           infos
                                                                                               .Where
                                                                                               (info
                                                                                                =>
                                                                                                info
                                                                                                    .FullName
                                                                                                    .EndsWith
                                                                                                    (LearnCaseExt)),
                                                                                           options
                                                                                               .Force));
                }
            }
        }

        protected override bool ShouldWriteHelp(LearnSubOptions options)
        {
            return options == null || (options.File == null && !options.All && options.Paths == null);
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

            this.SessionContext.NeuronNetwork.Train(inputs, outputs,
                                                    (epoch, input) =>
                                                    this.SessionContext.Output.WriteLine(
                                                        "Current epoch: {0}, case: {1}", epoch, input));        }

        private void LearnCase(string inputFile)
        {
            using (var stream = new FileStream(inputFile, FileMode.Open))
            {
                this.LearnCase(stream);
            }
        }


        private class LearnFileDoesntExist : SimException
        {
            private string file;

            public LearnFileDoesntExist(SessionContext context, string file)
                : base(context)
            {
                this.file = file;
            }

            public override void WriteError()
            {
                this.Context.Output.WriteLine("Cannot read learn case - file {0} does not exist.", this.file);
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
