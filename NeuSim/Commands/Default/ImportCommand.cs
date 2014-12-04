namespace NeuSim.Commands.Default
{
    using System;
    using NeuSim.Arguments;
    using NeuSim.Context;

    internal class ImportCommand : CommandBase<ImportSubOptions>
    {
        public ImportCommand(SessionContext sessionContext) : base(sessionContext)
        {
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
            throw new NotImplementedException();
        }
    }
}
