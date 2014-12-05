namespace NeuSim.Commands.Default
{
    using System.IO;
    using NeuSim.Arguments;
    using NeuSim.Common.Services;
    using NeuSim.Context;

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
            this.compressionService.CompressFolder(this.SessionContext.ContextDirectory,
                                                   Path.Combine(this.SessionContext.ContextDirectory,
                                                                string.Format("{0}.zip", options.Name)), "zip");

        }
    }
}
