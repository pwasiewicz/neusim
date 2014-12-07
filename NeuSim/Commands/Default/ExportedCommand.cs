namespace NeuSim.Commands.Default
{
    using System.IO;
    using NeuSim.Arguments;
    using NeuSim.Context;

    internal class ExportedCommand : CommandBase<ExportedSubOptions>
    {
        // TODO move to command
        private const string ExportedExt = "zip";

        public ExportedCommand(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "exported"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override void Run(ExportedSubOptions options)
        {
            if (options.List)
            {

                var allExported =
                    (new DirectoryInfo(this.SessionContext.ContextDirectory)).GetFiles(string.Format("*.{0}",
                                                                                                     ExportedExt));

                foreach (var file in allExported)
                {
                    this.SessionContext.Output.WriteLine(Path.GetFileNameWithoutExtension(file.Name));
                }

                return;
            }

            if (options.Delete != null)
            {
                var exportedNetwork = Path.Combine(this.SessionContext.ContextDirectory,
                                                   string.Format("{0}.{1}", options.Delete, ExportedExt));

                if (!File.Exists(exportedNetwork))
                {
                    this.SessionContext.Output.WriteLine("Exported network doesn't exist.");
                }

                File.Delete(exportedNetwork);
            }
        }
    }
}
