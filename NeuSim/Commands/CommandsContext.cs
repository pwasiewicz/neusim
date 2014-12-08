namespace NeuSim.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    internal class CommandsContext
    {
        private readonly IEnumerable<ICommand> commandsEvaluator;

        private IDictionary<string, ICommand> commands; 

        public CommandsContext(IEnumerable<ICommand> commandsEvaluator)
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

            this.commands = this.commandsEvaluator.ToDictionary(command => command.Name);
        }
    }
}
