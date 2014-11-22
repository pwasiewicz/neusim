namespace NeuSim.Arguments
{
    using System.IO;
    using CommandLine;

    internal class RunArguments
    {
        /// <summary>
        /// Gets or sets the Context.
        /// </summary>
        [VerbOption("init", HelpText = "Inits new simulator session inside current directory.")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("config", HelpText = "Sets to configuration of session.")]
        public ConfigSubOptions ConfigVerb { get; set; }

        [VerbOption("destroy", HelpText = "Destroys session inside current directory.")]
        public DestroySubOptions DestroyVerb { get; set; }

        [VerbOption("simulate", HelpText = "Simulates the specified data via network.")]
        public SimulateSubOptions SimulateVerb { get; set; }

        [VerbOption("learn", HelpText = "Learn the network of with specified data.")]
        public LearnSubOptions LearnVerb { get; set; }
    }

    internal class ConfigSubOptions
    {
        [Option('a', "activation", HelpText = "Sets activation function for neurons.")]
        public string ActivationFunc { get; set; }

        [Option('d', "derviative", HelpText = "Sets derivative of activation function for neurons.")]
        public string DerivativeActivationFunc { get; set; }

        [Option('p', "parser", HelpText = "Sets the script that will be applied to result network.")]
        public string ResultParserFile { get; set; }

        [Option('e', "epoch", HelpText = "Sets the number of epoch used in learn properties.")]
        public int? LearnEpoch { get; set; }

        public bool IsDefined(TextWriter errorWriter)
        {
            if (string.IsNullOrWhiteSpace(this.ActivationFunc))
            {
                errorWriter.WriteLine("Config: activation is not specified.");
                return false;
            }

            if (this.LearnEpoch == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(this.DerivativeActivationFunc))
            {
                return true;
            }

            errorWriter.WriteLine("Config: derivative of activation function is not specified.");
            return false;
        }

        public static ConfigSubOptions Default()
        {
            return new ConfigSubOptions
                   {
                       ActivationFunc = "1.0 / (1.0 + Exp(-x))",
                       DerivativeActivationFunc = "x * (x - 1)",
                       LearnEpoch = 10000,
                       ResultParserFile = null
                   };
        }
    }

    internal class LearnSubOptions
    {
        [OptionArray('p', "path", HelpText = "Learns from files inside specified path.")]
        public string[] Paths { get; set; }
    }

    internal class DestroySubOptions
    {
    }

    internal class InitSubOptions
    {
        [Option('i', "input", HelpText = "Number of input lnegth of network.", Required = true)]
        public int Inputs { get; set; }

        [Option('h', "hidden", HelpText = "Number of hiden neurons in hidden layer.", Required = true)]
        public int HiddenInputs { get; set; }
    }

    public class SimulateSubOptions
    {
        [OptionArray('f', "files", HelpText = "Input file for input data.", MutuallyExclusiveSet = "input")]
        public string[] Files { get; set; }

        [OptionArray('i', "input", HelpText = "Input data for inputs space separated.", MutuallyExclusiveSet = "input")]
        public double[] Input { get; set; }

        [Option("aggregate", HelpText = "Applies custom aggregate function to results.", DefaultValue = false)]
        public bool AgreggateResult { get; set; }

        [Option("skiptransform", HelpText = "Skips transform if needed.", DefaultValue = false)]
        public bool IgnoreTransform { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
    }
}
