namespace NeuSim.Commands.Default
{
    using System.IO;
    using System.Linq;
    using NeuSim.Arguments;
    using NeuSim.Context;
    using NeuSim.Helpers;

    internal class ExportedCommand : CommandBase<ExportedSubOptions>
    {
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
                                                                                                     ExportingContext.ExporedExtensions));

                if (!allExported.Any())
                {
                    this.SessionContext.Output.WriteLine("No exported networks.");
                    return;
                }

                foreach (var file in allExported)
                {
                    this.SessionContext.Output.WriteLine(Path.GetFileNameWithoutExtension(file.Name));
                }

                return;
            }

            if (options.Delete != null)
            {
                var exportedNetwork = Path.Combine(this.SessionContext.ContextDirectory,
                                                   string.Format("{0}.{1}", options.Delete,
                                                                 ExportingContext.ExporedExtensions));

                if (!File.Exists(exportedNetwork))
                {
                    this.SessionContext.Output.WriteLine("Exported network doesn't exist.");
                }

                File.Delete(exportedNetwork);
            }
        }
    }
}
