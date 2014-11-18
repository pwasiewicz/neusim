namespace NeuSim.Commands.Default
{
    using NeuSim.Arguments;
    using NeuSim.Context;

    internal class SimulateCommand : CommandBase<SimulateSubOptions>
    {
        public SimulateCommand(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "simualte"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override bool Run(SimulateSubOptions options)
        {
            return true;
        }
    }
}
