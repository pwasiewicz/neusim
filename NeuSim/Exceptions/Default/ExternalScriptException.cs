namespace NeuSim.Exceptions.Default
{
    using Context;
    using System;

    internal class ExternalScriptException : SimException
    {
        public ExternalScriptException(SessionContext context) : base(context) { }
        public ExternalScriptException(SessionContext context, Exception inner) : base(context, inner) { }
        public override void WriteError()
        {
            this.Context.Output.WriteLine(
                                          "There is an error while executing parser script. Please check the syntax of script. Details: {0}",
                                          this.InnerException != null
                                              ? this.InnerException.Message
                                              : "(details missing)");
        }
    }
}
