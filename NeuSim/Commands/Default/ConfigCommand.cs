namespace NeuSim.Commands.Default
{
    using NeuSim.Arguments;
    using NeuSim.Context;

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
            return true;
        }
    }
}
