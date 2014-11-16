namespace NeuSim.Commands.Default
{
    using NeuSim.Arguments;
    using NeuSim.Context;
    using System.IO;

    internal class DestroyCommand : CommandBase<DestroySubOptions>
    {
        public DestroyCommand(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override string Name
        {
            get { return "destroy"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public override bool Run(DestroySubOptions options)
        {
            var dirInfo = new DirectoryInfo(this.SessionContext.ContextDirectory);
            dirInfo.Delete(recursive: true);

            return true;
        }
    }
}
