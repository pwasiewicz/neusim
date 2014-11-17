namespace NeuSim.Context
{
    using System;
    using System.IO;
    using NeuSim.AI;
    using NeuSim.Extensions;

    internal class SessionContext
    {
        private const string NaiConfigDirectory = ".nai";

        private readonly TextWriter defaultWriter;

        private NeuronNetworkContext networkContext;

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

        public string NetworkPath
        {
            get { return Path.Combine(this.ContextDirectory, "network"); }
        }

        public string NeuronContextConfigPath
        {
            get { return Path.Combine(this.ContextDirectory, "context"); }
        }

        public bool IsInitialized
        {
            get { return Directory.Exists(this.ContextDirectory) && File.Exists(this.NetworkPath); }
        }

        public NeuronNetworkContext NetworkContext
        {
            get
            {
                if (this.networkContext != null)
                {
                    return null;
                }

                //this.networkContext = !File.Exists(this.NeuronContextConfigPath)
                //                          ? NeuronNetworkContext.BuildDefault()
                //                          : NeuronNetworkContext.BuildDefault();

                this.networkContext = NeuronNetworkContext.BuildDefault();
                return this.networkContext;
            }
        }
    }
}
