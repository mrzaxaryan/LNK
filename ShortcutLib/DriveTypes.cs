namespace ShortcutLib;

/// <summary>
/// Drive type constants for <see cref="LocalPathInfo.DriveType"/> per [MS-SHLLINK] 2.3.1.
/// </summary>
public static class DriveTypes
{
    /// <summary>The drive type cannot be determined.</summary>
    public const uint Unknown = 0;

    /// <summary>The root path is invalid (no volume is mounted).</summary>
    public const uint NoRootDir = 1;

    /// <summary>Removable media (floppy disk, USB flash drive).</summary>
    public const uint Removable = 2;

    /// <summary>Fixed media (hard disk, SSD).</summary>
    public const uint Fixed = 3;

    /// <summary>Remote (network) drive.</summary>
    public const uint Remote = 4;

    /// <summary>CD-ROM drive.</summary>
    public const uint CDRom = 5;

    /// <summary>RAM disk.</summary>
    public const uint RamDisk = 6;
}
