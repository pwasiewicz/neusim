namespace NeuSim.Commands.Default
{
    using NeuSim.Arguments;
    using NeuSim.Context;

    internal class LearnCommand : CommandBase<LearnSubOptions>
    {
        public LearnCommand(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "learn"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override void Run(LearnSubOptions options)
        {
        }
    }

    public class LearnCase
    {
        public double Output { get; set; }

        public double[] Input { get; set; }
    }
}
