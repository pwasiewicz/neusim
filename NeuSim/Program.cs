namespace NeuSim
{
    using CommandLine;
    using NeuSim.Arguments;
    using NeuSim.Commands;
    using NeuSim.Context;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new RunArguments();

            string invokedVerb = null;
            object invokerVerbOptions = null;

            if (!Parser.Default.ParseArguments(args, options, (verCommand, verbOptions) =>
            {
                invokedVerb = verCommand;
                invokerVerbOptions = verbOptions;
            }))
            {
                // TODO print help;
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            var session = new SessionContext(Console.Out);
            var commandsContext = new CommandsContext(session);

            commandsContext.ResolveCommands();
            commandsContext.RunCommand(invokedVerb, invokerVerbOptions);
        }
    }
}
