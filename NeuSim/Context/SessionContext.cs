namespace NeuSim.Context
{
    using System;
    using System.IO;
    using NeuSim.AI;
    using NeuSim.Arguments;
    using NeuSim.Eval;
    using NeuSim.Extensions;

    internal class SessionContext
    {
        private const string NaiConfigDirectory = ".nai";

        private readonly TextWriter defaultWriter;

        private NeuronNetworkContext networkContext;

        private NeuronNetwork neuronNetwork;

        private ConfigSubOptions contextConfigOptions;

        public SessionContext(TextWriter defaultWriter)
        {
            this.defaultWriter = defaultWriter;
            this.WorkingPath = Environment.CurrentDirectory;
        }

        public string WorkingPath { get; private set; }

        public TextWriter Output
        {
            get { return this.defaultWriter; }
        }

        public string ContextDirectory
        {
            get { return Path.Combine(this.WorkingPath, NaiConfigDirectory); }
        }

        public string NeuronNetworkPath
        {
            get { return Path.Combine(this.ContextDirectory, "network"); }
        }

        public string NeuronContextConfigPath
        {
            get { return Path.Combine(this.ContextDirectory, "context"); }
        }

        public bool IsInitialized
        {
            get { return Directory.Exists(this.ContextDirectory) && File.Exists(this.NeuronNetworkPath); }
        }

        public ConfigSubOptions ContextConfig
        {
            get
            {
                if (this.contextConfigOptions != null)
                {
                    return this.contextConfigOptions;
                }

                if (!File.Exists(this.NeuronContextConfigPath))
                {
                    return null;
                }

                this.contextConfigOptions = this.NeuronContextConfigPath.DeserializeFromPath<ConfigSubOptions>();
                return this.contextConfigOptions;
            }
        }

        public NeuronNetworkContext NetworkContext
        {
            get
            {
                if (this.networkContext != null)
                {
                    return null;
                }

                var configOptions = this.ContextConfig;
                if (configOptions == null)
                {
                    this.networkContext = NeuronNetworkContext.BuildDefault();
                }
                else
                {
                    this.networkContext = new NeuronNetworkContext
                                          {
                                              Function = Evaluator.ToDelegate(configOptions.ActivationFunc),
                                              Derivative = x => x * (x -1)
                                          };
                }

                this.networkContext = NeuronNetworkContext.BuildDefault();
                return this.networkContext;
            }
        }

        public NeuronNetwork NeuronNetwork
        {
            get
            {
                if (this.neuronNetwork != null)
                {
                    return this.neuronNetwork;
                }

                using (var file = new FileStream(this.NeuronNetworkPath, FileMode.Open, FileAccess.Read))
                {
                    this.neuronNetwork = NeuronNetwork.Load(file, this.NetworkContext);
                }

                return this.neuronNetwork;
            }
        }
    }
}
