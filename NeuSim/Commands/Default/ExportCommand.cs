namespace NeuSim.Commands.Default
{
    using System;
    using NeuSim.Arguments;
    using NeuSim.Context;

    internal class ExportCommand : CommandBase<ExportSubOptions>
    {
        public ExportCommand(SessionContext sessionContext) : base(sessionContext)
        {
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
            throw new NotImplementedException();
        }
    }
}
