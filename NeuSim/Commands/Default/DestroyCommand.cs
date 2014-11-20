namespace NeuSim.Commands.Default
{
    using Arguments;
    using Context;
    using Exceptions.Default;
    using System.IO;

    internal class DestroyCommand : CommandBase<DestroySubOptions>
    {
        public DestroyCommand(SessionContext sessionContext)
            : base(sessionContext)
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

        public override void Run(DestroySubOptions options)
        {
            try
            {
                var dirInfo = new DirectoryInfo(this.SessionContext.ContextDirectory);
                dirInfo.Delete(recursive: true);
            }
            catch (IOException ex)
            {
                throw new FileAccessException(this.SessionContext, ex, this.SessionContext.ContextDirectory);
            }
        }
    }
}
