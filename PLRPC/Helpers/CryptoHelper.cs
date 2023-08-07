using System.Security.Cryptography;
using System.Text;

namespace LBPUnion.PLRPC.Helpers;

public static class CryptoHelper
{
    public static string Sha1Hash(string input)
    {
        byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new(hash.Length * 2);

        foreach (byte b in hash)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}