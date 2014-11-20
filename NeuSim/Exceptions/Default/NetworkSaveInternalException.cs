namespace NeuSim.Exceptions.Default
{
    using System;
    using Context;

    internal class NetworkSaveInternalException : SimException
    {
        public NetworkSaveInternalException(SessionContext context) : base(context) { }
        public NetworkSaveInternalException(SessionContext context, Exception inner) : base(context, inner) { }

        public override void WriteError()
        {
            this.Context.Output.WriteLine("Internal error while serializing network.");
        }
    }
}
