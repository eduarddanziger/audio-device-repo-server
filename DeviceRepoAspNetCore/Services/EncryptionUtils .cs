using System.Security.Cryptography;
using System.Text;

namespace DeviceRepoAspNetCore.Services;


public static class EncryptionUtils
{
    // Encrypt plain text data using AES-256 with a passphrase-based key
    public static string Encrypt(string plainText, string passphrase)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(passphrase))
            throw new ArgumentNullException(nameof(passphrase));

        // Convert plaintext and passphrase to bytes
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var keyBytes = GetAesKeyFromPassphrase(passphrase);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        // Prepend IV to the ciphertext
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(plainBytes, 0, plainBytes.Length);
        }

        // Return Base64-encoded IV + ciphertext
        return Convert.ToBase64String(ms.ToArray());
    }

    // Decrypt ciphertext using passphrase
    public static string Decrypt(string cipherText, string passphrase)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(passphrase))
            throw new ArgumentNullException(nameof(passphrase));

        var fullCipher = Convert.FromBase64String(cipherText);
        var keyBytes = GetAesKeyFromPassphrase(passphrase);

        using var aes = Aes.Create();
        aes.Key = keyBytes;

        // Extract IV from the first aes.BlockSize/8 bytes
        var iv = new byte[aes.BlockSize / 8];
        Array.Copy(fullCipher, iv, iv.Length);
        aes.IV = iv;

        // The rest is the actual ciphertext
        var actualCipher = new byte[fullCipher.Length - iv.Length];
        Array.Copy(fullCipher, iv.Length, actualCipher, 0, actualCipher.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(actualCipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);

        return reader.ReadToEnd();
    }

    // Helper to derive a 256-bit key from the passphrase
    private static byte[] GetAesKeyFromPassphrase(string passphrase)
    {
        // Using a fixed salt for simplicity. In production, use a unique salt per secret.
        var salt = Encoding.UTF8.GetBytes("FixedSaltValue");
        const int iterations = 10000; // PBKDF2 iterations

        using var rfc2898 = new Rfc2898DeriveBytes(passphrase, salt, iterations, HashAlgorithmName.SHA256);
        return rfc2898.GetBytes(32); // 32 bytes for AES-256
    }
}
