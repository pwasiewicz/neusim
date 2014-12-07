namespace NeuSim.Compressing.Helpers
{
    using System.Text;

    internal static class PathHelpers
    {
        internal static string AddSeparatorIfNeeded(this string path)
        {
            const string separator = "\\";

            if (!path.EndsWith(separator))
            {
                return path + separator;
            }

            return path;
        }
        internal static string PathDifference(string path1, string path2)
        {
            var c = 0;  //index up to which the paths are the same
            var d = -1; //index of trailing slash for the portion where the paths are the same

            while (c < path1.Length && c < path2.Length)
            {
                if (char.ToLowerInvariant(path1[c]) != char.ToLowerInvariant(path2[c]))
                {
                    break;
                }

                if (path1[c] == '\\')
                {
                    d = c;
                }

                c++;
            }

            if (c == 0)
            {
                return path2;
            }

            if (c == path1.Length && c == path2.Length)
            {
                return string.Empty;
            }


            var builder = new StringBuilder();

            while (c < path1.Length)
            {
                if (path1[c] == '\\')
                {
                    builder.Append(@"..\");
                }
                c++;
            }

            if (builder.Length == 0 && path2.Length - 1 == d)
            {
                return @".\";
            }

            return builder + path2.Substring(d + 1);
        }
    }
}
