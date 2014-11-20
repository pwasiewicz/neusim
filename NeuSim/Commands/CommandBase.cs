namespace NeuSim.Commands
{
    using System;
    using NeuSim.Context;

    internal abstract class CommandBase<TOptions> : ICommand
    {
        protected readonly SessionContext SessionContext;

        protected CommandBase(SessionContext sessionContext)
        {
            this.SessionContext = sessionContext;
        }

        public abstract string Name { get; }

        public abstract bool AllowNotInitialized { get; }

        public virtual void Run(object options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (this.AllowNotInitialized || this.SessionContext.IsInitialized)
            {
                this.Run((TOptions)options);
                return;
            }

            this.SessionContext.Output.WriteLine(
                "Session is not initialized. Use init command to create new session.");
        }

        public abstract void Run(TOptions options);
    }
}
