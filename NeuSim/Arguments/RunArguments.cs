namespace NeuSim.Arguments
{
    using CommandLine;
    using CommandLine.Text;
    using Newtonsoft.Json;
    using System.IO;

    internal class RunArguments
    {
        [VerbOption("init", HelpText = "Inits new simulator session inside working directory.")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("config", HelpText = "Allows to specify configuration of current session.")]
        public ConfigSubOptions ConfigVerb { get; set; }

        [VerbOption("destroy", HelpText = "Destroys session inside working directory if available.")]
        public DestroySubOptions DestroyVerb { get; set; }

        [VerbOption("simulate", HelpText = "Simulates the specified data with neuron network")]
        public SimulateSubOptions SimulateVerb { get; set; }

        [VerbOption("learn", HelpText = "Learn the network of with specified data.")]
        public LearnSubOptions LearnVerb { get; set; }

        [VerbOption("display", HelpText = "Displays the neuron network")]
        public DisplaySubOptions DisplayVerb { get; set; }

        [VerbOption("export", HelpText = "Exports the whole network with learnt files.")]
        public ExportSubOptions ExportVerb { get; set; }

        [VerbOption("import", HelpText = "Imports the previously exported network.")]
        public ImportSubOptions ImportVerb { get; set; }

        [VerbOption("exported", HelpText = "Manages exported networks.")]
        public ExportedSubOptions ExportedVerb { get; set; }

        [VerbOption("stats", HelpText = "Generates states for learn case.")]
        public StatsSubOptions StatesVerb { get; set; }
    }

    public class StatsSubOptions : DefaultHelpable
    {
        [Option('f', "file", Required = true, HelpText = "File name to generate stats.")]
        public string File { get; set; }
    }

    internal class ExportedSubOptions : DefaultHelpable
    {
        [Option('l', "list", DefaultValue = false, HelpText = "Lists all exported networks.", MutuallyExclusiveSet = "ExportedOptions")]
        public bool List { get; set; }

        [Option('d', "delete", HelpText = "Lists all exported networks.", MutuallyExclusiveSet = "ExportedOptions")]
        public string Delete { get; set; }
    }

    internal class ImportSubOptions : DefaultHelpable
    {
        [Option('n', "name", HelpText = "Name of network to import.", Required = true)]
        public string Name { get; set; }
    }

    internal class ExportSubOptions : DefaultHelpable
    {
        [Option('n', "name", HelpText = "Name of exported network.", Required = true)]
        public string Name { get; set; }
        
        [Option('o', "override", HelpText = "Override exsiting exported network.")]
        public bool? Override { get; set; }
    }

    internal class ConfigSubOptions : DefaultHelpable
    {
        [Option('a', "activation",
            HelpText =
                "Sets the activation function for neurons. Supports base function like Exp, Sin or Cos. The variable is x literal. Sample: -a 1/(x+Exp(-x))"
            )]
        public string ActivationFunc { get; set; }

        [Option('d', "derviative", HelpText = "Sets the derivative of activation function for neurons. The variable is x literal. Sample: -d x*(1-x)")]
        public string DerivativeActivationFunc { get; set; }

        [Option('p', "parser",
            HelpText =
                "Allows to the script that will be applied to result network. The input is file name with parser function specified. Sample: -p result-parser.js"
            )]
        public string ResultParserFile { get; set; }

        [Option('e', "epoch", HelpText = "Sets the number of epoch used in learn properties. Sample: -e 10000")]
        public int? LearnEpoch { get; set; }

        [Option('t', "tolarance", HelpText = "Sets the tolerance of error on output value. Sample: -t 0.005")]
        public double? Tolerance { get; set; }

        [Option('s', "step", HelpText = "Sets the learn step - how sensitive should network be while learning. Less = more sensitive. Sample: -s 0.5")]
        public double? LearnStep { get; set; }

        [OptionArray('w', "weight",
            HelpText =
                "Sets manually weight of specified input in neuron inside layer. Sample (first layer, first neuron and first input): -l 1 -n 1 -i 2 -w 1.02"
            )]
        [JsonIgnore]
        public int? Weight { get; set; }

        [OptionArray('b', "bias", HelpText = "Sets manually bias of specified input in neuron inside layer. Sample: -l 1 -n 1 -i 2 -b 0.02")]
        [JsonIgnore]
        public int? Bias { get; set; }

        [Option('l', "layer", HelpText = "Sets the context layer for setting weight or bias.")]
        [JsonIgnore]
        public int? Layer { get; set; }

        [Option('n', "neuron", HelpText = "Sets the context layer for setting weight or bias.")]
        [JsonIgnore]
        public int? Neuron { get; set; }

        [Option('i', "input", HelpText = "Sets the context for input of selected neuron in \"neuron\" option.")]
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

            if (this.LearnStep == null)
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
                Tolerance = 0.001,
                LearnStep = 0.5
            };
        }
    }

    internal class LearnSubOptions : DefaultHelpable
    {
        [OptionArray('p', "path",
            HelpText =
                "Learns network from files inside specified path. Files must end with end with learn extension. Sample: -p LearnCases\\XOR",
            MutuallyExclusiveSet = "all")]
        public string[] Paths { get; set; }

        [Option('f', "file", HelpText = "Learns the specified learn case from file. Sample -f LearnCase.learn", MutuallyExclusiveSet = "all")]
        public string File { get; set; }

        [Option("all", HelpText = "Learns all non-learnt cases from all subdirectories.", MutuallyExclusiveSet = "all")]
        public bool All { get; set; }

        [Option("force", HelpText = "Flag that forces to learn cases even it has already been learnt.")]
        public bool Force { get; set; }
    }

    internal class DestroySubOptions
    {
    }

    internal class InitSubOptions : DefaultHelpable
    {
        [Option('i', "input", HelpText = "Number of input neuron of network. Sample: -i 2", Required = true)]
        public int Inputs { get; set; }

        [Option('h', "hidden", HelpText = "Number of neurons in hidden layer. Sample: -i 2", Required = true)]
        public int HiddenInputs { get; set; }
    }

    public class SimulateSubOptions : DefaultHelpable
    {
        [OptionArray('f', "files", HelpText = "Simulates data from specified files. Sample: -f File1 File2.", MutuallyExclusiveSet = "input")]
        public string[] Files { get; set; }

        [OptionArray('i', "input", HelpText = "Simulates the data given in standard input. Sample: -i 0.2 0.4.", MutuallyExclusiveSet = "input")]
        public double[] Input { get; set; }

        [Option("aggregate", HelpText = "Applies custom aggregate function to results. Transform file must be specified.", DefaultValue = false)]
        public bool AgreggateResult { get; set; }

        [Option("skiptransform", HelpText = "Skips transform if available.", DefaultValue = false)]
        public bool IgnoreTransform { get; set; }
    }

    public class DisplaySubOptions
    {

    }

    public abstract class DefaultHelpable : IHelpable
    {
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
