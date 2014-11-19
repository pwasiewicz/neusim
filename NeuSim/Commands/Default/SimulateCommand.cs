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
            if (options.Files != null)
            {
                return this.RunFiles(options);
            }

            return this.RunSingleInput(options);

            //var inputs = new List<Tuple<double[], string>>();

            //if (options.Files != null && options.Files.Any())
            //{
            //    inputs.AddRange(
            //        options.Files.Select(
            //            file =>
            //            new Tuple<double[], string>(
            //                this.SessionContext.RelativeToAbsolute(file).DeserializeFromPath<double[]>(), file)));
            //}

            //if (options.Input != null)
            //{
            //    inputs.Add(new Tuple<double[], string>(options.Input.ToArray(), null));
            //}

            //var resultsToAggregate = new List<double>();

            //foreach (var input in inputs)
            //{
            //    var inputArr = input.Item1;
            //    var file = input.Item2;

            //    var result = this.SessionContext.NeuronNetwork.Process(inputArr);
            //    resultsToAggregate.Add(result);

            //    if (!options.AgreggateResult)
            //    {
            //        var resultTransformed = this.SessionContext.TransformResult(result);

            //        if (file == null)
            //        {
            //            this.PrintOutput(inputArr, resultTransformed);
            //        }
            //    }
            //}

            //if (options.AgreggateResult)
            //{
            //    var aggregated = this.SessionContext.AggregateResults(resultsToAggregate.ToArray());
            //}

            //return true;
        }

        private bool RunSingleInput(SimulateSubOptions options)
        {
            var input = options.Input;
            if (input == null)
            {
                throw new InvalidOperationException();
            }

            if (!input.Any())
            {
                this.PrintOutput(input, "0.0");
            }

            var network = this.SessionContext.NeuronNetwork;
            var result = network.Process(input);

            if (options.IgnoreTransform)
            {
                this.PrintOutput(input, result);
                return true;
            }

            if (options.AgreggateResult)
            {
                var aggregated = this.SessionContext.AggregateResults(new[] {result});
                this.PrintOutput(input, aggregated);
                return true;
            }

            var transformed = this.SessionContext.TransformResult(result);
            this.PrintOutput(input, transformed);

            return true;
        }

        private bool RunFiles(SimulateSubOptions options)
        {
            var files = options.Files;

            var results = new List<double>();

            foreach (var file in files)
            {
                var filePath = this.SessionContext.RelativeToAbsolute(file);
                if (!File.Exists(file))
                {
                    throw new InvalidOperationException();
                }

                var input = filePath.DeserializeFromPath<double[]>();
                var output = this.SessionContext.NeuronNetwork.Process(input);

                if (options.IgnoreTransform)
                {
                    this.WriteToFile(input, output, file);
                }

                if (options.AgreggateResult)
                {
                    results.Add(output);
                }
                else
                {
                    var transformed = this.SessionContext.TransformResult(output);
                    this.WriteToFile(input, transformed, file);
                }
            }

            if (!options.IgnoreTransform && options.AgreggateResult)
            {
                var aggregated = this.SessionContext.AggregateResults(results.ToArray());
                this.PrintOutput(new double[0], aggregated);
            }

            return true;

        }

        private void WriteToFile(IEnumerable<double> input, string output, string inputFile)
        {
            var fullFile = this.SessionContext.RelativeToAbsolute(inputFile);
            fullFile = Path.GetFileNameWithoutExtension(fullFile) + ".out";

            if (File.Exists(fullFile))
            {
                this.SessionContext.Output.WriteLine("Cannot write output to file {0} - it already exist.", fullFile);
                return;
            }

            File.WriteAllText(fullFile, output);
        }

        private void WriteToFile(IEnumerable<double> input, double output, string inputFile)
        {
            this.WriteToFile(input, output.ToString("R"), inputFile);
        }

        private void PrintOutput(IEnumerable<double> inputCase, double output)
        {
            this.PrintOutput(inputCase, output.ToString("R"));
        }

        private void PrintOutput(IEnumerable<double> inputCase, string output)
        {
            this.SessionContext.Output.WriteLine("\t{0}: {1}", string.Join(" ", inputCase), output);
        }
    }
}
