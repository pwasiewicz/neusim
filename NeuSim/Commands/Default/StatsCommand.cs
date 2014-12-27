namespace NeuSim.Commands.Default
{
    using System;
    using System.IO;
    using NeuSim.Arguments;
    using NeuSim.Context;
    using NeuSim.Extensions;

    internal class StatsCommand : CommandBase<StatsSubOptions>
    {
        public StatsCommand(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "stats"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override void Run(StatsSubOptions options)
        {
            var filePath = this.SessionContext.RelativeToAbsolute(options.File);
            if (!File.Exists(filePath))
            {
                this.SessionContext.Output.WriteLine("Specified file doesn't exist.");
                return;
            }

            var learnCases = filePath.DeserializeFromPath<LearnCase[]>();

            var properOutputs = 0;
            var invalidOutputs = 0;

            foreach (var learnCase in learnCases)
            {
                var output = this.SessionContext.NeuronNetwork.Process(learnCase.Input);
                if (Math.Abs(output - learnCase.Output) < float.Epsilon)
                {
                    properOutputs += 1;
                }
                else
                {
                    invalidOutputs += 1;
                }
            }

            this.SessionContext.Output.WriteLine("Valid outputs: {0}", properOutputs);
            this.SessionContext.Output.WriteLine("Invalid outputs: {0}", invalidOutputs);
        }
    }
}
