using System.Security.Cryptography;

namespace HealthPlatform.Shared.Encryption;

/// <summary>
/// Provides encryption keys from environment variables.
/// Supports dual-key reading for rotation: ENCRYPTION_KEY (current) + ENCRYPTION_KEY_PREVIOUS (optional).
/// </summary>
public sealed class EncryptionKeyProvider
{
    private readonly byte[] _currentKey;
    private readonly byte[]? _previousKey;

    public EncryptionKeyProvider()
    {
        var currentKeyBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
            ?? throw new InvalidOperationException(
                "ENCRYPTION_KEY environment variable is required for PII encryption.");

        _currentKey = Convert.FromBase64String(currentKeyBase64);

        if (_currentKey.Length != 32)
            throw new InvalidOperationException("ENCRYPTION_KEY must be exactly 32 bytes (256-bit) encoded as base64.");

        var previousKeyBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_KEY_PREVIOUS");
        if (!string.IsNullOrEmpty(previousKeyBase64))
        {
            _previousKey = Convert.FromBase64String(previousKeyBase64);
        }
    }

    public byte[] GetCurrentKey() => _currentKey;
    public byte[]? GetPreviousKey() => _previousKey;

    /// <summary>Generates a new 256-bit key suitable for AES-256-GCM. For key provisioning only.</summary>
    public static string GenerateNewKey()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }
}
