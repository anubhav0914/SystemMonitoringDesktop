using System.IO;
using System.Security.Cryptography;

namespace SystemMonitoring
{
    public static class FileHashHelper
    {
        public static string ComputeSHA256OfFile(string path)
        {
            using var stream = File.OpenRead(path);
            using SHA256 sha = SHA256.Create();
            var hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
