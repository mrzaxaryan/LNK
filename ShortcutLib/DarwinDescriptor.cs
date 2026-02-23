namespace ShortcutLib;

/// <summary>
/// Decodes MSI Darwin Descriptor strings from DarwinDataBlock.
/// Darwin descriptors encode a Product Code GUID, Feature ID, and Component Code GUID
/// in a compressed format used by Windows Installer advertised shortcuts.
/// </summary>
public sealed class DarwinDescriptor
{
    /// <summary>The MSI product code GUID.</summary>
    public Guid ProductCode { get; set; }

    /// <summary>The feature identifier string.</summary>
    public string FeatureId { get; set; } = "";

    /// <summary>The MSI component code GUID.</summary>
    public Guid ComponentCode { get; set; }

    /// <summary>
    /// Attempts to decode a Darwin descriptor string.
    /// Returns null if the format is not recognized.
    /// </summary>
    public static DarwinDescriptor? TryDecode(string? descriptor)
    {
        if (string.IsNullOrEmpty(descriptor) || descriptor.Length < 32)
            return null;

        try
        {
            Guid productCode = DecodeCompressedGuid(descriptor.AsSpan(0, 32));
            string remaining = descriptor.Substring(32);

            int featureEnd = remaining.IndexOf('>');
            string featureId;
            string rest;
            if (featureEnd >= 0)
            {
                featureId = remaining.Substring(0, featureEnd);
                rest = remaining.Substring(featureEnd + 1);
            }
            else
            {
                featureId = remaining;
                rest = "";
            }

            Guid componentCode = rest.Length >= 32
                ? DecodeCompressedGuid(rest.AsSpan(0, 32))
                : Guid.Empty;

            return new DarwinDescriptor
            {
                ProductCode = productCode,
                FeatureId = featureId,
                ComponentCode = componentCode
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Encodes a GUID into the MSI compressed format (32 hex characters).
    /// </summary>
    public static string EncodeCompressedGuid(Guid guid)
    {
        string hex = guid.ToString("N").ToUpperInvariant(); // 32 hex chars, no dashes
        char[] packed = new char[32];

        // Reverse groups to match MSI packed format
        ReverseInto(hex, 0, 8, packed, 0);   // DWORD
        ReverseInto(hex, 8, 4, packed, 8);   // WORD
        ReverseInto(hex, 12, 4, packed, 12); // WORD
        for (int i = 0; i < 8; i++)          // 8 individual bytes (pairs of hex chars)
            ReverseInto(hex, 16 + i * 2, 2, packed, 16 + i * 2);

        return new string(packed);
    }

    private static Guid DecodeCompressedGuid(ReadOnlySpan<char> packed)
    {
        if (packed.Length < 32) throw new FormatException("Packed GUID must be at least 32 characters.");

        // MSI compressed GUIDs are the 32 hex chars of a GUID with groups reversed.
        // Take {XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}, strip braces/dashes → 32 hex chars,
        // then reverse chars within each dash-delimited group → 32 packed chars.
        Span<char> hex = stackalloc char[32];
        packed.Slice(0, 32).CopyTo(hex);

        Span<char> result = stackalloc char[32];
        ReverseInto(hex, 0, 8, result, 0);
        ReverseInto(hex, 8, 4, result, 8);
        ReverseInto(hex, 12, 4, result, 12);
        for (int i = 0; i < 8; i++)
            ReverseInto(hex, 16 + i * 2, 2, result, 16 + i * 2);

        string guidHex = new string(result);
        return Guid.ParseExact(guidHex, "N");
    }

    private static void ReverseInto(ReadOnlySpan<char> source, int srcOffset, int length, Span<char> dest, int destOffset)
    {
        for (int i = 0; i < length; i++)
            dest[destOffset + i] = source[srcOffset + length - 1 - i];
    }
}
