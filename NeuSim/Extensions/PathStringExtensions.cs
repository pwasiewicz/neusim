namespace NeuSim.Extensions
{
    using System.IO;
    using Newtonsoft.Json;

    public static class PathStringExtensions
    {
        public static void SerializeToPath<T>(this string path, T value)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(value));
        }

        public static T DeserializeFromPath<T>(this string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
