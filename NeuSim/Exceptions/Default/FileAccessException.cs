namespace NeuSim.Exceptions.Default
{
    using System;
    using Context;

    internal class FileAccessException : SimException
    {
        private readonly string path;

        public FileAccessException(SessionContext context, Exception innerException, string path)
            : base(context, innerException)
        {
            this.path = path;
        }

        public override void WriteError()
        {
            this.Context.Output.WriteLine("Cannot access path {0}. No sufficient permissions, file in use or even does not exist.",
                                          this.path);
        }
    }
}
