namespace NeuSim.Commands.Default
{
    using NeuSim.Arguments;
    using NeuSim.Context;
    using NeuSim.Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class SimulateCommand : CommandBase<SimulateSubOptions>
    {
        public SimulateCommand(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "simulate"; }
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
                inputs.Add(new Tuple<double[], string>(options.Input.ToArray(), null));
            }

            foreach (var input in inputs)
            {
                var inputArr = input.Item1;
                var file = input.Item2;
                
                var result = this.SessionContext.NeuronNetwork.Process(inputArr);
                var resultTransformed = this.SessionContext.TransformResult(result);
                if (file == null)
                {
                    this.PrintOutput(inputArr, resultTransformed);
                }
            }

            return true;
        }

        private void WriteToFile(IEnumerable<double> input, double output, string inputFile)
        {
            var fullFile = this.SessionContext.RelativeToAbsolute(inputFile);
            fullFile = Path.GetFileNameWithoutExtension(fullFile) + ".out";

            if (File.Exists(fullFile))
            {
                this.SessionContext.Output.WriteLine("Cannot write output to file {0} - it already exist.", fullFile);
                return;
            }

            File.WriteAllText(fullFile, output.ToString("R"));
        }

        private void PrintOutput(IEnumerable<double> input, string output)
        {
            this.SessionContext.Output.WriteLine("\t{0}: {1}", string.Join(" ", input), output);
        }
    }
}
