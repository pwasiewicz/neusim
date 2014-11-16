namespace NeuSim.Context
{
    using System;
    using System.IO;

    internal class SessionContext
    {
        private const string NaiConfigDirectory = ".nai";

        private readonly TextWriter defaultWriter;

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

        public bool IsInitialized
        {
            get { return Directory.Exists(this.ContextDirectory); }
        }
    }
}
