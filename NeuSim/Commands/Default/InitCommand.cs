namespace NeuSim.Commands.Default
{
    using System.Runtime.Serialization;
    using AI;
    using Arguments;
    using Context;
    using Exceptions;
    using Exceptions.Default;
    using System;
    using System.IO;

    internal class InitCommand : CommandBase<InitSubOptions>
    {
        public InitCommand(SessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "init"; }
        }

        public override bool AllowNotInitialized
        {
            get { return true; }
        }

        public override void Run(InitSubOptions options)
        {
            if (this.SessionContext.IsInitialized)
            {
                throw new SessionAlreadyInitedException(this.SessionContext);
            }

            if (options.Inputs <= 0)
            {
                this.SessionContext.Output.WriteLine("Input length must be at least 1.");
                return;
            }

            if (options.HiddenInputs <= 0)
            {
                this.SessionContext.Output.WriteLine("Hidden neruon no must be at least 1.");
                return;
            }

            try
            {
                var dirInfo = Directory.CreateDirectory(this.SessionContext.ContextDirectory);
                dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            catch (IOException e)
            {
                throw new FileAccessException(this.SessionContext, e, this.SessionContext.ContextDirectory);
            }

            var network = new NeuronNetwork(options.Inputs, options.HiddenInputs, NeuronNetworkContext.BuildDefault());

            try
            {
                using (
                    var stream = new FileStream(this.SessionContext.NeuronNetworkPath, FileMode.CreateNew,
                                                FileAccess.ReadWrite))
                {
                    NeuronNetwork.Save(network, stream);
                }
            }
            catch (SerializationException ex)
            {
                throw new NetworkSaveInternalException(this.SessionContext, ex);
            }
            catch (Exception ex)
            {
                throw new ConfigWriteException(this.SessionContext, ex);
            }
        }

        private class SessionAlreadyInitedException : SimException
        {
            public SessionAlreadyInitedException(SessionContext context) : base(context) { }

            public override void WriteError()
            {
                this.Context.Output.WriteAsync("There is session initialzied inside provided directory.");
            }
        }

        private class ConfigWriteException : SimException
        {
            public ConfigWriteException(SessionContext context, Exception inner) : base(context, inner) { }
            public override void WriteError()
            {
                this.Context.Output.WriteLine(
                                              "Cannot write initial network state. No sufficient permssion or interna error.");
            }
        }
    }
}
