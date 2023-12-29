using System.Security.Cryptography;

namespace PhotoLibraryBackend.MediaReader;

public static class Md5Helper
{
    public static string GenerateHash(this string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
