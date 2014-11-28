namespace NeuSim.Commands
{
    using System;
    using Arguments;
    using NeuSim.Context;

    internal abstract class CommandBase<TOptions> : ICommand where TOptions : class, new()
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
            if (this.AllowNotInitialized || this.SessionContext.IsInitialized)
            {
                this.Run((TOptions)options);
                return;
            }

            this.SessionContext.Output.WriteLine(
                "Session is not initialized. Use init command to create new session.");
        }

        public abstract void Run(TOptions options);

        protected virtual bool ShouldWriteHelp(TOptions options)
        {
            return options == null;
        }

        protected virtual bool WriteHelp(TOptions options)
        {
            if (!this.ShouldWriteHelp(options))
            {
                return false;
            }

            var emptyOptions = new TOptions();
            var helpable = emptyOptions as IHelpable;
            if (helpable == null)
            {
                return false;
            }

            var helpText = helpable.GetUsage();
            this.SessionContext.Output.WriteLine(helpText);
            return true;
        }
    }
}
