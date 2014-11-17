namespace NeuSim.Commands.Default
{
    using NeuSim.AI;
    using NeuSim.Arguments;
    using NeuSim.Context;
    using System.IO;

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

            var dirInfo = Directory.CreateDirectory(this.SessionContext.ContextDirectory);
            dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            var network = new NeuronNetwork(command.Inputs, command.HiddenInputs, NeuronNetworkContext.BuildDefault());
            using (
                var stream = new FileStream(this.SessionContext.NetworkPath, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                NeuronNetwork.Save(network, stream);
            }

            return true;
        }
    }
}
