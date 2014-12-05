namespace NeuSim.Common.Services
{
    public interface ICompressionService
    {
        void CompressFolder(string path, string outputFile, params string[] excludedExtensions);
    }
}
