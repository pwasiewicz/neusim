namespace NeuSim.Arguments
{
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
        [Option('a', "ActivationFunction", HelpText = "Sets activation function for neurons.")]
        public string ActivationFunc { get; set; }

        [Option('d', "DerviativeActivationFunction", HelpText = "Sets derivative of activation function for neurons.")]
        public string DerivativeActivationFunc { get; set; }
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
        [Option('f', "file", HelpText = "Input file for input data.")]
        public string File { get; set; }
    }
}
