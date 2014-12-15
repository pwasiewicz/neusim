namespace NeuSim
{
    using Arguments;
    using CommandLine;
    using Commands;
    using Context;
    using Exceptions;
    using MiniAutFac;
    using MiniAutFac.Interfaces;
    using NeuSim.Compressing;
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
                                        settings.HelpWriter = null;
                                        settings.IgnoreUnknownArguments = false;
                                        settings.MutuallyExclusive = true;
                                    });

            parser.ParseArgumentsStrict(args, options, (verCommand, verbOptions) =>
                                                       {
                                                           invokedVerb = verCommand;
                                                           invokerVerbOptions = verbOptions;
                                                       }, () => invokerVerbOptions = null);

            if (invokedVerb == null)
            {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            using (var scope = BuildIoC())
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

        private static ILifetimeScope BuildIoC()
        {
            var builder = new ContainerBuilder();

            builder.Register(type => typeof(ICommand).IsAssignableFrom(type), Assembly.GetExecutingAssembly())
                   .As<ICommand>()
                   .PerLifetimeScope();

            builder.Register<SessionContext>()
                   .As<SessionContext>().WithNamedParameter("defaultWriter", Console.Out)
                   .PerLifetimeScope();

            builder.Register<EvaluatorService>().AsImplementedInterfaces();
            builder.Register<HashCalculator>().AsImplementedInterfaces();
            builder.Register<CompressionService>().AsImplementedInterfaces();

            builder.Register<CommandsContext>();

            return builder.Build();
        }
    }
}
