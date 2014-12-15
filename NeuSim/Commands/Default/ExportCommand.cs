namespace NeuSim.Commands.Default
{
    using System.IO;
    using NeuSim.Arguments;
    using NeuSim.Common.Services;
    using NeuSim.Context;
    using NeuSim.Helpers;

    internal class ExportCommand : CommandBase<ExportSubOptions>
    {
        private readonly ICompressionService compressionService;

        public ExportCommand(SessionContext sessionContext, ICompressionService compressionService)
            : base(sessionContext)
        {
            this.compressionService = compressionService;
        }

        public override string Name
        {
            get { return "export"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override void Run(ExportSubOptions options)
        {
            if (this.ExportedExists(options))
            {
                this.SessionContext.Output.WriteLine("Exported newtork with that name already exist.");
                return;
            }

            this.compressionService.CompressFolder(this.SessionContext.ContextDirectory,
                                                   Path.Combine(this.SessionContext.ContextDirectory,
                                                                string.Format("{0}.{1}", options.Name,
                                                                              ExportingContext.ExporedExtensions)),
                                                   ExportingContext.ExporedExtensions);
        }

        private bool ExportedExists(ExportSubOptions options)
        {
            var fileName = Path.Combine(this.SessionContext.ContextDirectory,
                                        string.Format("{0}.{1}", options.Name, ExportingContext.ExporedExtensions));
            if (!File.Exists(fileName))
            {
                return true;
            }

            if (!options.Override.HasValue || !options.Override.Value)
            {
                return false;
            }

            File.Delete(fileName);
            return true;
        }
    }
}
