namespace NeuSim.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NeuSim.Context;

    internal class CommandsContext
    {
        private readonly SessionContext sessionContext;
        private IDictionary<string, ICommand> commands;

        public CommandsContext(SessionContext sessionContext)
        {
            this.sessionContext = sessionContext;
        }

        public void ResolveCommands()
        {
            var types =
                Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof (ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            this.commands = new Dictionary<string, ICommand>();

            foreach (var type in types)
            {
                var instance = (ICommand)Activator.CreateInstance(type, this.sessionContext);
                this.commands.Add(instance.Name, instance);
            }
        }

        public void RunCommand(string command, object options)
        {
            this.commands[command].Run(options);
        }
    }
}
