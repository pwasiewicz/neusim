namespace NeuSim.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class CommandsContext
    {
        private readonly Lazy<IEnumerable<ICommand>> commandsEvaluator;

        private IDictionary<string, ICommand> commands; 

        public CommandsContext(Lazy<IEnumerable<ICommand>> commandsEvaluator)
        {
            this.commandsEvaluator = commandsEvaluator;
        }


        public void RunCommand(string command, object options)
        {
            this.ResolveCommands();

            this.commands[command].Run(options);
        }

        private void ResolveCommands()
        {
            if (commands != null)
            {
                return;
            }

            this.commands = this.commandsEvaluator.Value.ToDictionary(command => command.Name);
        }
    }
}
