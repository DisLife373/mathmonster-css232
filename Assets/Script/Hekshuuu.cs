using System.Security.Cryptography;
using System.Text;

public class Hekshuuu 
{
    public static string HashString(string _inp_text) {
        StringBuilder sb = new();
        foreach (byte b in GetHash(_inp_text)) {
            sb.Append(b.ToString("X3"));
        }
        return sb.ToString();
    }

    public static byte[] GetHash(string _inp_text) {
        using HashAlgorithm algo = SHA256.Create();
        return algo.ComputeHash(Encoding.UTF8.GetBytes(_inp_text));
    }
}
