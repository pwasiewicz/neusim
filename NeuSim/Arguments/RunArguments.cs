namespace NeuSim.Arguments
{
    using CommandLine;

    internal class RunArguments
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        [VerbOption("init", HelpText = "Inits new NAI session inside current directory.")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("config", HelpText = "Sets to configuration of session.")]
        public ConfigSubOptions ConfigVerb { get; set; }

        [VerbOption("destroy", HelpText = "Destrys session inside current directory.")]
        public DestroySubOptions DestroyVerb { get; set; }
    }

    internal class ConfigSubOptions
    {
        [Option('a', "ActivationFunction", HelpText = "Sets activtion function for neurons.")]
        public string ActivationFunc { get; set; }
    }

    internal class DestroySubOptions
    {
    }

    internal class InitSubOptions
    {        
    }
}
