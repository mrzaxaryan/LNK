namespace ShortcutLib;

/// <summary>
/// Represents a single property from a deserialized MS-PROPSTORE.
/// Used by <see cref="PropertyStoreReader"/> for forensic analysis and display.
/// </summary>
public sealed class PropertyStoreEntry
{
    /// <summary>The format ID (GUID) of the property storage section.</summary>
    public Guid FormatId { get; set; }

    /// <summary>Numeric property ID (for integer-keyed properties). Null for string-named.</summary>
    public uint? PropertyId { get; set; }

    /// <summary>Property name (for string-named properties under D5CDD505-...). Null for PID-based.</summary>
    public string? Name { get; set; }

    /// <summary>The VT type code (e.g. 0x001F for VT_LPWSTR, 0x000B for VT_BOOL).</summary>
    public ushort VtType { get; set; }

    /// <summary>
    /// The deserialized property value. Type depends on VtType:
    /// VT_LPWSTR → string, VT_BOOL → bool, VT_UI4 → uint, VT_I4 → int,
    /// VT_CLSID → Guid, VT_FILETIME → DateTime, VT_BLOB → byte[],
    /// unknown → byte[].
    /// </summary>
    public object? Value { get; set; }
}
