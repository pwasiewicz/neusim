namespace NeuSim.Context
{
    using System.Runtime.Serialization;
    using AI;
    using Arguments;
    using Eval;
    using Exceptions.Default;
    using Extensions;
    using NeuSim.Exceptions;
    using Services;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal class SessionContext : IDisposable
    {
        private const string NaiConfigDirectory = ".nai";

        private readonly TextWriter defaultWriter;

        private NeuronNetworkContext networkContext;

        private NeuronNetwork neuronNetwork;

        private ConfigSubOptions contextConfigOptions;

        private string resultParser;

        private bool resultParsesLoaded;

        private readonly IEvaluatorService evaluatorService;

        public SessionContext(TextWriter defaultWriter, IEvaluatorService evaluatorService)
        {
            this.defaultWriter = defaultWriter;
            this.evaluatorService = evaluatorService;
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
                Debug.Assert(configOptions.Tolerance != null, "configOptions.Tolerance != null");
                Debug.Assert(configOptions.LearnStep != null, "configOptions.LearnStep != null");

                this.networkContext = new NeuronNetworkContext
                                      {
                                          Function = Evaluator.ToDelegate(configOptions.ActivationFunc),
                                          Derivative =
                                              Evaluator.ToDelegate(configOptions.DerivativeActivationFunc),
                                          LearnEpoch = configOptions.LearnEpoch.Value,
                                          LearnStep = configOptions.LearnStep.Value,
                                          ErrorTolerance = configOptions.Tolerance.Value
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

            internal set { this.neuronNetwork = value; }
        }

        public string RelativeToAbsolute(string relative)
        {
            return Path.Combine(this.WorkingPath, relative);
        }

        public string AggregateResults(double[] results)
        {
            this.EnsureParsers();
            if (string.IsNullOrWhiteSpace(this.resultParser))
            {
                return results.Average().ToString("R");
            }

            try
            {
                return this.evaluatorService.CallFunction(this.resultParser, "aggregate", results);
            }
            catch (Exception ex)
            {
                throw new ExternalScriptException(this, ex);
            }
        }

        public string TransformResult(double result)
        {
            this.EnsureParsers();
            if (string.IsNullOrWhiteSpace(this.resultParser))
            {
                return result.ToString("R");
            }

            try
            {
                return this.evaluatorService.CallFunction(this.resultParser, "transform", result);
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

        public void Dispose()
        {
            try
            {
                if (this.neuronNetwork == null)
                {
                    return;
                }

                using (var networkFile = new FileStream(this.NeuronNetworkPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    NeuronNetwork.Save(this.NeuronNetwork, networkFile);
                }
            }
            catch (SerializationException ex)
            {
                throw new NetworkSaveInternalException(this, ex);
            }
            catch (Exception ex)
            {
                throw new NetworkWriteException(this, ex);
            }
        }

        private class NetworkWriteException : SimException
        {
            public NetworkWriteException(SessionContext context, Exception inner) : base(context, inner) { }
            public override void WriteError()
            {
                this.Context.Output.WriteLine(
                    "Cannot write network state. No sufficient permssion or internal error occured.");
            }
        }
    }
}
