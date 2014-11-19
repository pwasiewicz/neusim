namespace NeuSim.Arguments
{
    using System.Collections.Generic;
    using System.IO;
    using CommandLine;

    internal class RunArguments
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        [VerbOption("init", HelpText = "Inits new simulator session inside current directory.")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("config", HelpText = "Sets to configuration of session.")]
        public ConfigSubOptions ConfigVerb { get; set; }

        [VerbOption("destroy", HelpText = "Destroys session inside current directory.")]
        public DestroySubOptions DestroyVerb { get; set; }

        [VerbOption("simulate", HelpText = "Simulates the specified data via network.")]
        public SimulateSubOptions SimulateVerb { get; set; }
    }

    internal class ConfigSubOptions
    {
        [Option('a', "activation", HelpText = "Sets activation function for neurons.")]
        public string ActivationFunc { get; set; }

        [Option('d', "derviative", HelpText = "Sets derivative of activation function for neurons.")]
        public string DerivativeActivationFunc { get; set; }

        [Option('p', "parser", HelpText = "Sets the script that will be applied to result network.")]
        public string ResultParserFile { get; set; }

        public bool IsDefined(TextWriter errorWriter)
        {
            if (string.IsNullOrWhiteSpace(this.ActivationFunc))
            {
                errorWriter.WriteLine("Config: activation is not specified.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.DerivativeActivationFunc))
            {
                errorWriter.WriteLine("Config: derivative of activation function is not specified.");
                return false;
            }

            return true;
        }
    }

    internal class DestroySubOptions
    {
    }

    internal class InitSubOptions
    {
        [Option('i', "inputs", HelpText = "Number of inputs of network.", Required = true)]
        public int Inputs { get; set; }

        [Option('h', "hidden", HelpText = "Number of hiden neurons in hidden layer.", Required = true)]
        public int HiddenInputs { get; set; }
    }

    public class SimulateSubOptions
    {
        [Option('f', "files", HelpText = "Input file for input data.", MutuallyExclusiveSet = "input")]
        public string[] Files { get; set; }

        [OptionArray('i', "input", HelpText = "Input data for inputs space separated.", MutuallyExclusiveSet = "input")]
        public double[] Input { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
    }
}
