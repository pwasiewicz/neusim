namespace NeuSim.Commands.Default
{
    using System.IO;
    using NeuSim.Arguments;
    using NeuSim.Common.Services;
    using NeuSim.Context;
    using NeuSim.Helpers;

    internal class ImportCommand : CommandBase<ImportSubOptions>
    {
        private readonly ICompressionService compressionService;

        public ImportCommand(SessionContext sessionContext, ICompressionService compressionService) : base(sessionContext)
        {
            this.compressionService = compressionService;
        }

        public override string Name
        {
            get { return "import"; }
        }

        public override bool AllowNotInitialized
        {
            get { return true; }
        }

        public override void Run(ImportSubOptions options)
        {
            if (!this.IsExported(options))
            {
                this.SessionContext.Output.WriteLine("The given name is invalid - no such network was exported.");
                return;
            }

            var fullPath = Path.Combine(this.SessionContext.ContextDirectory,
                                        string.Format("{0}.{1}", options.Name, ExportingContext.ExporedExtensions));
            this.compressionService.UncompressFilder(fullPath, this.SessionContext.ContextDirectory);
        }

        public bool IsExported(ImportSubOptions options)
        {
            var fullPath = Path.Combine(this.SessionContext.ContextDirectory,
                                        string.Format("{0}.{1}", options.Name, ExportingContext.ExporedExtensions));

            return !File.Exists(fullPath);
        }
    }
}
