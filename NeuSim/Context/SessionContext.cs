namespace NeuSim.Context
{
    using AI;
    using Arguments;
    using Eval;
    using Exceptions.Default;
    using Extensions;
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class SessionContext
    {
        private const string NaiConfigDirectory = ".nai";

        private readonly TextWriter defaultWriter;

        private NeuronNetworkContext networkContext;

        private NeuronNetwork neuronNetwork;

        private ConfigSubOptions contextConfigOptions;

        private string resultParser;

        private bool resultParsesLoaded;

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
            get { return Path.Combine(this.ContextDirectory, "Context"); }
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
                    throw new InvalidOperationException("Missing config file");
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
                if (!configOptions.IsDefined(this.Output))
                {
                    this.Output.WriteLine("Missing some settings, using default context.");
                    configOptions = ConfigSubOptions.Default();

                }

                Debug.Assert(configOptions.LearnEpoch != null, "configOptions.LearnEpoch != null");

                this.networkContext = new NeuronNetworkContext
                                      {
                                          Function = Evaluator.ToDelegate(configOptions.ActivationFunc),
                                          Derivative =
                                              Evaluator.ToDelegate(configOptions.DerivativeActivationFunc),
                                          LearnEpoch = configOptions.LearnEpoch.Value
                                      };

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

        public string RelativeToAbsolute(string relative)
        {
            return Path.Combine(this.WorkingPath, relative);
        }

        public string AggregateResults(double[] results)
        {
            this.EnsureParsers();

            try
            {
                return Evaluator.JsAggregate(results, this.resultParser);
            }
            catch (Exception ex)
            {
                throw new ExternalScriptException(this, ex);
            }
        }

        public string TransformResult(double result)
        {
            this.EnsureParsers();

            try
            {
                return Evaluator.JsEval(result, this.resultParser);
            }
            catch (Exception ex)
            {
                throw new ExternalScriptException(this, ex);
            }
        }

        private void EnsureParsers()
        {
            if (this.resultParsesLoaded)
            {
                return;
            }

            var config = this.ContextConfig;

            if (!string.IsNullOrWhiteSpace(config.ResultParserFile))
            {
                var fullPath = config.ResultParserFile;

                try
                {
                    fullPath = this.RelativeToAbsolute(config.ResultParserFile);

                    if (File.Exists(fullPath))
                    {
                        this.resultParser = File.ReadAllText(fullPath);
                    }
                }
                catch (IOException ex)
                {
                    throw new FileAccessException(this, ex, fullPath);
                }
            }

            this.resultParsesLoaded = true;
        }
    }
}
