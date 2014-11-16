namespace NeuSim.Commands.Default
{
    using System.IO;
    using NeuSim.Arguments;
    using NeuSim.Context;

    internal class InitCommand : CommandBase<InitSubOptions>
    {
        public InitCommand(SessionContext sessionContext) : base(sessionContext)
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

        public override bool Run(InitSubOptions command)
        {
            if (this.SessionContext.IsInitialized)
            {
                this.SessionContext.Output.WriteLine("There is session initialized inside provided directory.");
                return false;
            }

            var dirInfo = new DirectoryInfo(this.SessionContext.ContextDirectory)
                          {
                              Attributes =
                                  FileAttributes.Directory |
                                  FileAttributes.Hidden
                          };

            dirInfo.Create();

            return true;
        }
    }
}
