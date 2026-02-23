using System.Text;

namespace ShortcutLib;

/// <summary>
/// Computes the WinX menu hash required for Power User Menu (Win+X) shortcuts.
/// Without this hash, shortcuts placed in the WinX menu folder are silently ignored.
/// The hash is stored as a PropertyStore property under
/// {FB8D2D7B-90D1-4E34-BF60-6EAC09922BBF} PID 2 (VT_UI4).
/// </summary>
public static class WinXHasher
{
    private const string Salt = "Do not prehash links. This should only be done by the user.";

    /// <summary>
    /// Computes the WinX hash for the given target path and arguments.
    /// The hash matches the algorithm used by shlwapi!HashData.
    /// </summary>
    public static uint ComputeHash(string targetPath, string? arguments = null)
    {
        ArgumentNullException.ThrowIfNull(targetPath);
        string input = targetPath + (arguments ?? "");
        string data = input.ToLowerInvariant() + Salt;
        byte[] bytes = Encoding.Unicode.GetBytes(data);
        return HashData(bytes);
    }

    /// <summary>
    /// Builds a PropertyStoreData byte array containing the WinX hash.
    /// Optionally merges with additional properties from a <see cref="PropertyStoreBuilder"/>.
    /// </summary>
    public static byte[] BuildPropertyStore(string targetPath, string? arguments = null)
    {
        uint hash = ComputeHash(targetPath, arguments);
        var builder = new PropertyStoreBuilder { WinXHash = hash };
        return builder.Build();
    }

    /// <summary>
    /// Implementation of the shlwapi!HashData algorithm.
    /// </summary>
    private static uint HashData(byte[] data)
    {
        // shlwapi!HashData with 4-byte output uses this algorithm:
        // Initialize hash bytes, then iterate over input data
        byte[] hash = [0x81, 0x3A, 0xDE, 0x67];
        for (int i = 0; i < data.Length; i++)
        {
            hash[i % 4] ^= data[i];
            hash[i % 4] = (byte)((hash[i % 4] << 3) | (hash[i % 4] >> 5));
            hash[i % 4] ^= data[i];
            hash[i % 4] = RotateLeft(hash[i % 4], data[i]);
        }
        return BitConverter.ToUInt32(hash, 0);
    }

    private static byte RotateLeft(byte value, byte count)
    {
        int shift = count & 7;
        return (byte)((value << shift) | (value >> (8 - shift)));
    }
}
