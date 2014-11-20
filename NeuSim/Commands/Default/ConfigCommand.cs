namespace NeuSim.Commands.Default
{
    using System.IO;
    using Arguments;
    using Context;
    using Exceptions;
    using Exceptions.Default;
    using Extensions;
    using System;

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

            try
            {
                this.SessionContext.NeuronContextConfigPath.SerializeToPath(currentOptions);
            }
            catch (Exception ex)
            {
                throw new FileAccessException(this.SessionContext, ex, this.SessionContext.NeuronContextConfigPath);
            }
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
