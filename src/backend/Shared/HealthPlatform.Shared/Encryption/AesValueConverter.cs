using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HealthPlatform.Shared.Encryption;

/// <summary>
/// EF Core value converter that encrypts string values using AES-256-GCM before storage
/// and decrypts on read. Supports dual-key reading for key rotation.
/// </summary>
public sealed class AesValueConverter : ValueConverter<string?, string?>
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public AesValueConverter(EncryptionKeyProvider keyProvider)
        : base(
            v => Encrypt(v, keyProvider.GetCurrentKey()),
            v => Decrypt(v, keyProvider.GetCurrentKey(), keyProvider.GetPreviousKey()))
    {
    }

    private static string? Encrypt(string? plaintext, byte[] key)
    {
        if (string.IsNullOrEmpty(plaintext))
            return plaintext;

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        // Format: base64(nonce + ciphertext + tag)
        var result = new byte[NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    private static string? Decrypt(string? encrypted, byte[] currentKey, byte[]? previousKey)
    {
        if (string.IsNullOrEmpty(encrypted))
            return encrypted;

        try
        {
            return DecryptWithKey(encrypted, currentKey);
        }
        catch (CryptographicException) when (previousKey is not null)
        {
            // Dual-key reading: try previous key during rotation
            return DecryptWithKey(encrypted, previousKey);
        }
    }

    private static string DecryptWithKey(string encrypted, byte[] key)
    {
        var combined = Convert.FromBase64String(encrypted);
        var nonce = new byte[NonceSize];
        var ciphertextLength = combined.Length - NonceSize - TagSize;
        var ciphertext = new byte[ciphertextLength];
        var tag = new byte[TagSize];

        Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(combined, NonceSize, ciphertext, 0, ciphertextLength);
        Buffer.BlockCopy(combined, NonceSize + ciphertextLength, tag, 0, TagSize);

        var plaintext = new byte[ciphertextLength];
        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}
