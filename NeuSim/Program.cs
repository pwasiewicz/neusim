namespace NeuSim
{
    using Arguments;
    using Autofac;
    using CommandLine;
    using Commands;
    using Context;
    using Exceptions;
    using Services.Implementations;
    using System;
    using System.Reflection;

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
                                        settings.MutuallyExclusive = true;
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
                   .As<ICommand>();

            builder.RegisterType<SessionContext>()
                   .As<SessionContext>()
                   .WithParameter(new NamedParameter("defaultWriter", Console.Out))
                   .InstancePerLifetimeScope();

            builder.RegisterType<EvaluatorService>().AsImplementedInterfaces();
            builder.RegisterType<HashCalculator>().AsImplementedInterfaces();

            builder.RegisterType<CommandsContext>().AsSelf();

            return builder.Build();
        }
    }
}
