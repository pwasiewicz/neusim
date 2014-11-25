namespace NeuSim.Arguments
{
    using CommandLine;
    using Newtonsoft.Json;
    using System.IO;

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

        [VerbOption("display", HelpText = "Displays the neuron network")]
        public DisplaySubOptions DisplayVerb { get; set; }
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

        [Option('t', "tolarance", HelpText = "Sets the tolerance of error on output value")]
        public double? Tolerance { get; set; }

        [OptionArray('w', "weight", HelpText = "Sets manually weights of inputs.")]
        [JsonIgnore]
        public int? Weight { get; set; }

        [OptionArray('b', "bias", HelpText = "Sets manually baises of inputs.")]
        [JsonIgnore]
        public int? Bias { get; set; }

        [Option('l', "layer", HelpText = "Sets the context layer for setting weight or bias.")]
        [JsonIgnore]
        public int? Layer { get; set; }

        [Option('n', "neuron", HelpText = "Sets the context layer for setting weight or bias.")]
        [JsonIgnore]
        public int? Neuron { get; set; }

        [Option('t', "input", HelpText = "Sets the context for input of selected neuroon in \"neuron\" option.")]
        [JsonIgnore]
        public int? InputOfNeuron { get; set; }

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

            if (this.Tolerance == null)
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
                       DerivativeActivationFunc = "x * (1 - x)",
                       LearnEpoch = 10000,
                       ResultParserFile = null,
                       Tolerance = 0.001
                   };
        }
    }

    internal class LearnSubOptions
    {
        [OptionArray('p', "path", HelpText = "Learns from files inside specified path.", MutuallyExclusiveSet = "all")]
        public string[] Paths { get; set; }

        [Option('f', "file", HelpText = "Learns the specified learn case.", MutuallyExclusiveSet = "all")]
        public string File { get; set; }

        [Option("all", HelpText = "Learns all non-learn cases.", MutuallyExclusiveSet = "all")]
        public bool All { get; set; }

        [Option("force", HelpText = "Forces to learn cases even it has already been learnt.")]
        public bool Force { get; set; }
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
    }

    public class DisplaySubOptions
    {

    }
}
