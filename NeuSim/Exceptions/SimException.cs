namespace NeuSim.Exceptions
{
    using System;
    using Context;

    internal abstract class SimException : Exception
    {
        /// <summary>
        /// The Context
        /// </summary>
        protected readonly SessionContext Context;

        protected SimException(SessionContext context)
            : this(context, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimException" /> class.
        /// </summary>
        /// <param name="context">The Context.</param>
        /// <param name="inner">The inner.</param>
        protected SimException(SessionContext context, Exception inner)
            : base("Simulator exception occured. See output to check more details", inner)
        {
            this.Context = context;
        }

        public abstract void WriteError();
    }
}
