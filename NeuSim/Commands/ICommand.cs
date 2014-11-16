namespace NeuSim.Commands
{
    public interface ICommand
    {
        string Name { get; }

        bool Run(object options);
    }
}
