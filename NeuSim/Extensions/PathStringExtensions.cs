namespace NeuSim.Extensions
{
    using System.IO;
    using Newtonsoft.Json;

    public static class PathStringExtensions
    {
        public static T DeserializeFromPath<T>(this string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
