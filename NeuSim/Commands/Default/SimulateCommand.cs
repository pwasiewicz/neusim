namespace NeuSim.Commands.Default
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NeuSim.Arguments;
    using NeuSim.Context;
    using NeuSim.Extensions;

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
            var inputs = new List<Tuple<double[], string>>();

            if (options.Files != null && options.Files.Any())
            {
                inputs.AddRange(
                    options.Files.Select(
                        file =>
                        new Tuple<double[], string>(
                            this.SessionContext.RelativeToAbsolute(file).DeserializeFromPath<double[]>(), file)));
            }

            if (options.Input != null)
            {
                inputs.Add(new Tuple<double[], string>(options.Input, null));
            }

            return true;
        }
    }
}
