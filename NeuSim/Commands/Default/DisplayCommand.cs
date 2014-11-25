namespace NeuSim.Commands.Default
{
    using Arguments;
    using Context;

    internal class DisplayCommand : CommandBase<DisplaySubOptions>
    {
        public DisplayCommand(SessionContext sessionContext) : base(sessionContext) { }

        public override string Name
        {
            get { return "display"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override void Run(DisplaySubOptions options)
        {
        }
    }
}
