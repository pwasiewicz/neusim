namespace NeuSim.Commands.Default
{
    using System.Diagnostics;
    using AI.Specification;
    using Arguments;
    using Context;
    using Exceptions;
    using Exceptions.Default;
    using Extensions;
    using System;
    using System.IO;

    internal class ConfigCommand : CommandBase<ConfigSubOptions>
    {
        public ConfigCommand(SessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "config"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override void Run(ConfigSubOptions options)
        {
            var currentOptions = this.SessionContext.ContextConfig ?? new ConfigSubOptions();

            if (options.ActivationFunc != null)
            {
                currentOptions.ActivationFunc = options.ActivationFunc;
            }

            if (options.DerivativeActivationFunc != null)
            {
                currentOptions.DerivativeActivationFunc = options.DerivativeActivationFunc;
            }

            if (options.Tolerance != null)
            {
                currentOptions.Tolerance = options.Tolerance;
            }

            if (options.ResultParserFile != null)
            {
                if (!File.Exists(this.SessionContext.RelativeToAbsolute(options.ResultParserFile)))
                {
                    throw new PerserFileDoesntExistException(this.SessionContext, options.ResultParserFile);
                }

                currentOptions.ResultParserFile = options.ResultParserFile;
            }

            if (options.LearnEpoch.HasValue)
            {
                currentOptions.LearnEpoch = options.LearnEpoch;
            }

            if (options.Weight != null)
            {
                this.SetWeight(options);
            }

            try
            {
                this.SessionContext.NeuronContextConfigPath.SerializeToPath(currentOptions);
            }
            catch (Exception ex)
            {
                throw new FileAccessException(this.SessionContext, ex, this.SessionContext.NeuronContextConfigPath);
            }
        }

        private void SetWeight(ConfigSubOptions options)
        {
            if (!this.EnsureSpecifedNeuronWithInputSelected(options))
            {
                return;
            }

            Debug.Assert(options.Layer != null, "options.Layer != null");
            Debug.Assert(options.Neuron != null, "options.Neuron != null");
            Debug.Assert(options.InputOfNeuron != null, "options.InputOfNeuron != null");
            Debug.Assert(options.Weight != null, "options.Weight != null");

            this.SessionContext.NeuronNetwork.Specification().SetWeight((int)options.Layer, (int)options.Neuron, (int)options.InputOfNeuron, options.Weight.Value);
        }

        private bool EnsureSpecifedNeuronWithInputSelected(ConfigSubOptions options)
        {
            var specificationEvalutor =
                new Lazy<INeuronNetworkSpecification>(() => this.SessionContext.NeuronNetwork.Specification());

            if (!options.Layer.HasValue)
            {
                this.SessionContext.Output.WriteLine("You must specify layer of neuron. Use \"layer\" argument.");
                return false;
            }

            var layersNo = specificationEvalutor.Value.LayerNumber;
            if (options.Layer.IsOutOfRange(1, layersNo + 1))
            {
                this.SessionContext.Output.WriteLine("Layer number is out of range. Possibilities: {0} - {1}", 1,
                                                     layersNo);
                return false;
            }

            if (!options.Neuron.HasValue)
            {
                this.SessionContext.Output.WriteLine("You must specify neuron in layer. Use \"neuron\" option.");
                return false;
            }

            var neuronInLayers = specificationEvalutor.Value.NeuronInLayers(options.Layer.Value);
            if (options.Neuron.IsOutOfRange(1, neuronInLayers + 1))
            {
                this.SessionContext.Output.WriteLine("Neuron no is out of range. Possibilities: {0} - {1}", 1,
                                                     neuronInLayers);
                return false;
            }

            if (!options.InputOfNeuron.HasValue)
            {
                this.SessionContext.Output.WriteLine("You must specify input neruon of specified neuron. Use \"input\" option.");
                return false;
            }

            var inputNoInNeuron = specificationEvalutor.Value.InputsInNeuron(options.Layer.Value, options.Neuron.Value);
            if (!options.InputOfNeuron.IsOutOfRange(1, inputNoInNeuron + 1))
            {
                return true;
            }

            this.SessionContext.Output.WriteLine("Input is out of range inside nuron. Possibilities: {0} - {1}", 1,
                                                 inputNoInNeuron);
            return false;
        }

        private class PerserFileDoesntExistException : SimException
        {
            private readonly string relativePath;

            public PerserFileDoesntExistException(SessionContext context, string relativePath)
                : base(context)
            {
                this.relativePath = relativePath;
            }

            public override void WriteError()
            {
                this.Context.Output.WriteLine("File {0} with parser functions does not exist.", this.relativePath);
            }
        }
    }
}
