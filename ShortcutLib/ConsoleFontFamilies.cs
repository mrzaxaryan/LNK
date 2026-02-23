namespace ShortcutLib;

/// <summary>
/// Font family and pitch constants for <see cref="ConsoleData.FontFamily"/>.
/// Combine a family value with a pitch flag using bitwise OR.
/// </summary>
public static class ConsoleFontFamilies
{
    // Font families (bits 4-7)

    /// <summary>Don't care or unknown font family.</summary>
    public const uint DontCare = 0x0000;

    /// <summary>Roman font family (variable-width, serif).</summary>
    public const uint Roman = 0x0010;

    /// <summary>Swiss font family (variable-width, sans-serif).</summary>
    public const uint Swiss = 0x0020;

    /// <summary>Modern font family (fixed-width, serif or sans-serif).</summary>
    public const uint Modern = 0x0030;

    /// <summary>Script font family (handwriting-like).</summary>
    public const uint Script = 0x0040;

    /// <summary>Decorative font family (novelty).</summary>
    public const uint Decorative = 0x0050;

    // Pitch flags (bits 0-3)

    /// <summary>Fixed-pitch (monospace) font.</summary>
    public const uint FixedPitch = 0x01;

    /// <summary>Vector (scalable) font.</summary>
    public const uint Vector = 0x02;

    /// <summary>TrueType font.</summary>
    public const uint TrueType = 0x04;

    /// <summary>Device font.</summary>
    public const uint Device = 0x08;
}
