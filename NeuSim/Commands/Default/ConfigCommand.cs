namespace NeuSim.Commands.Default
{
    using NeuSim.Arguments;
    using NeuSim.Context;
    using NeuSim.Extensions;

    internal class ConfigCommand : CommandBase<ConfigSubOptions>
    {
        public ConfigCommand(SessionContext sessionContext) : base(sessionContext)
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

        public override bool Run(ConfigSubOptions options)
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

            this.SessionContext.NeuronContextConfigPath.SerializeToPath(currentOptions);

            return true;
        }
    }
}
