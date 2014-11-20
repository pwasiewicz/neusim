namespace NeuSim.Commands
{
    public interface ICommand
    {
        string Name { get; }

        void Run(object options);
    }
}
