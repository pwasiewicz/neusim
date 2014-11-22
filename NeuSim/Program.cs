namespace NeuSim
{
    using System.Linq;
    using Arguments;
    using Autofac;
    using CommandLine;
    using Commands;
    using Context;
    using Exceptions;
    using System;
    using System.Reflection;
    using Services.Implementations;

    public static class Program
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
                BeginNeuSim(scope.Resolve<CommandsContext>(), invokedVerb, invokerVerbOptions);
            }
        }

        private static void BeginNeuSim(CommandsContext ctx, string invokedVerb, object invokerVerbOptions)
        {
            try
            {
                ctx.RunCommand(invokedVerb, invokerVerbOptions);
            }
            catch (SimException ex)
            {
                ex.WriteError();
            }
        }

        private static IContainer BuildIoC()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(type => typeof(ICommand).IsAssignableFrom(type))
                   .As<ICommand>().InstancePerLifetimeScope();

            builder.RegisterType<SessionContext>()
                   .As<SessionContext>()
                   .WithParameter(new NamedParameter("defaultWriter", Console.Out))
                   .SingleInstance();

            builder.RegisterType<EvaluatorService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<HashCalculator>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<CommandsContext>().AsSelf().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
