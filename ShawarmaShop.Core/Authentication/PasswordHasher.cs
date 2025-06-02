using System.Security.Cryptography;
using System.Text;

namespace Authentication;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = md5.ComputeHash(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static bool Verify(string password, string storedHash) =>
        Hash(password) == storedHash;
}
