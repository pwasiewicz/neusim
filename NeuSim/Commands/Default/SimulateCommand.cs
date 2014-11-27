namespace NeuSim.Commands.Default
{
    using Arguments;
    using Context;
    using Exceptions;
    using Exceptions.Default;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class SimulateCommand : CommandBase<SimulateSubOptions>
    {
        public SimulateCommand(SessionContext sessionContext)
            : base(sessionContext)
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

        public override void Run(SimulateSubOptions options)
        {
            if (options.Files != null)
            {
                this.RunFiles(options);
                return;
            }

            this.RunSingleInput(options);
        }

        private void RunSingleInput(SimulateSubOptions options)
        {
            var input = options.Input;
            if (input == null)
            {
                throw new InvalidOperationException();
            }

            if (!input.Any())
            {
                this.SessionContext.Output.WriteLine("There is no input values.");
                return;
            }

            var network = this.SessionContext.NeuronNetwork;
            if (input.Length != network.InputNo)
            {
                throw new InvalidInputException(this.SessionContext, input.Length);
            }

            double result;

            try
            {
                result = network.Process(input);
            }
            catch (Exception ex)
            {
                throw new InternalProcessException(this.SessionContext, ex);
            }

            if (options.IgnoreTransform)
            {
                this.PrintOutput(input, result);
                return;
            }

            if (options.AgreggateResult)
            {

                var aggregated = this.SessionContext.AggregateResults(new[] { result });
                this.PrintOutput(input, aggregated);
                return;
            }

            var transformed = this.SessionContext.TransformResult(result);
            this.PrintOutput(input, transformed);
        }

        private void RunFiles(SimulateSubOptions options)
        {
            var files = options.Files;

            var results = new List<double>();

            foreach (var file in files)
            {
                var filePath = this.SessionContext.RelativeToAbsolute(file);
                if (!File.Exists(file))
                {
                    throw new FileAccessException(this.SessionContext, null, file);
                }

                var input = filePath.DeserializeFromPath<double[]>();
                if (input.Length != this.SessionContext.NeuronNetwork.InputNo)
                {
                    throw new InvalidInputInFileException(this.SessionContext, file, input.Length);
                }

                double output;

                try
                {
                    output = this.SessionContext.NeuronNetwork.Process(input);
                }
                catch (Exception ex)
                {
                    throw new InternalProcessException(this.SessionContext, ex);
                }

                if (options.IgnoreTransform)
                {
                    this.WriteToFile(output, file);
                }

                if (options.AgreggateResult)
                {
                    results.Add(output);
                }
                else
                {
                    var transformed = this.SessionContext.TransformResult(output);
                    this.WriteToFile(transformed, file);
                }
            }

            if (options.IgnoreTransform || !options.AgreggateResult)
            {
                return;
            }

            var aggregated = this.SessionContext.AggregateResults(results.ToArray());
            this.PrintOutput(new double[0], aggregated);
        }

        private void WriteToFile(string output, string inputFile)
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

        private void WriteToFile(double output, string inputFile)
        {
            this.WriteToFile(output.ToString("R"), inputFile);
        }

        private void PrintOutput(IEnumerable<double> inputCase, double output)
        {
            this.PrintOutput(inputCase, output.ToString("R"));
        }

        private void PrintOutput(IEnumerable<double> inputCase, string output)
        {
            this.SessionContext.Output.WriteLine("\t{0}: {1}", string.Join(" ", inputCase), output);
        }

        private class InvalidInputInFileException : SimException
        {
            private readonly string file;

            private readonly int inputLength;

            public InvalidInputInFileException(SessionContext context, string file, int inputLength)
                : base(context)
            {
                this.file = file;
                this.inputLength = inputLength;
            }


            public override void WriteError()
            {
                this.Context.Output.WriteLine("The file {0} contains invalid input lnegth. Actual: {1}. Desired: {2}",
                                              this.file, this.inputLength, this.Context.NeuronNetwork.InputNo);
            }
        }

        private class InvalidInputException : SimException
        {
            private readonly int inputNo;

            public InvalidInputException(SessionContext context, int inputNo)
                : base(context)
            {
                this.inputNo = inputNo;
            }

            public override void WriteError()
            {
                this.Context.Output.WriteLine("The number of input doesn't match network. Acutal: {0}. Desired: {1}",
                                              this.inputNo, this.Context.NeuronNetwork.InputNo);
            }
        }

        private class InternalProcessException : SimException
        {
            public InternalProcessException(SessionContext context, Exception inner) : base(context, inner) { }
            public override void WriteError()
            {
                this.Context.Output.WriteLine("An internal error occured while simulating input data. Message: {0}",
                                              this.InnerException.Message);
            }
        }
    }
}
