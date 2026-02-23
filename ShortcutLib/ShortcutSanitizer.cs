namespace ShortcutLib;

/// <summary>
/// Utility for stripping privacy-sensitive metadata from shortcut options.
/// LNK files may contain forensic artifacts such as the originating machine name,
/// MAC address, volume serial number, and user account information.
/// </summary>
public static class ShortcutSanitizer
{
    /// <summary>
    /// Removes privacy-sensitive data from the shortcut options in-place.
    /// Strips TrackerData (machine name, MAC address in UUIDs),
    /// PropertyStoreData (may contain file owner, computer name),
    /// and OverlayData (unstructured post-terminal data).
    /// All other properties (Target, LinkInfo, Console, etc.) are preserved.
    /// </summary>
    public static void Sanitize(ShortcutOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Tracker = null;
        options.PropertyStoreData = null;
        options.OverlayData = null;
    }

    /// <summary>
    /// Opens an existing .lnk file, strips privacy-sensitive data,
    /// and returns a sanitized byte array.
    /// </summary>
    public static byte[] SanitizeBytes(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var options = Shortcut.Open(data);
        Sanitize(options);
        return Shortcut.Create(options);
    }
}
