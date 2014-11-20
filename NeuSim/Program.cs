namespace NeuSim
{
    using Arguments;
    using Autofac;
    using CommandLine;
    using Commands;
    using Context;
    using Exceptions;
    using System;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new RunArguments();

            string invokedVerb = null;
            object invokerVerbOptions = null;

            var parser = new Parser(settings =>
                                    {
                                        settings.HelpWriter = Console.Out;
                                        settings.IgnoreUnknownArguments = false;
                                    });

            if (!parser.ParseArgumentsStrict(args, options, (verCommand, verbOptions) =>
            {
                invokedVerb = verCommand;
                invokerVerbOptions = verbOptions;
            }))
            {

                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            using (var scope = BuildIoC().BeginLifetimeScope())
            {
                try
                {
                    var commandsContext = scope.Resolve<CommandsContext>();
                    commandsContext.RunCommand(invokedVerb, invokerVerbOptions);
                }
                catch (SimException ex)
                {
                    ex.WriteError();
                }
            }
        }

        private static IContainer BuildIoC()
        {
            var session = new SessionContext(Console.Out);


            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(type => typeof(ICommand).IsAssignableFrom(type))
                   .As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterInstance(session).As<SessionContext>().SingleInstance();
            builder.RegisterType<CommandsContext>().AsSelf().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
