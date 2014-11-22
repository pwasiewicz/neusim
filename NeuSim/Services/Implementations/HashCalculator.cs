namespace NeuSim.Services.Implementations
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    internal class HashCalculator : IHashCalculator
    {
        public string GetHash(Stream stream)
        {
            var sha = SHA256.Create();

            var hash = sha.ComputeHash(stream);
            return Convert.ToBase64String(hash);
        }
    }
}
