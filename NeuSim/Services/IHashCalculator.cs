namespace NeuSim.Services
{
    using System.IO;

    public interface IHashCalculator
    {
        string GetHash(Stream stream);
    }
}
