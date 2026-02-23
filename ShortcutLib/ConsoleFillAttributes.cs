namespace ShortcutLib;

/// <summary>
/// Console text color flag constants for <see cref="ConsoleData.FillAttributes"/>
/// and <see cref="ConsoleData.PopupFillAttributes"/>.
/// Combine foreground and background flags with bitwise OR.
/// </summary>
public static class ConsoleFillAttributes
{
    /// <summary>Text color contains blue.</summary>
    public const ushort ForegroundBlue = 0x0001;

    /// <summary>Text color contains green.</summary>
    public const ushort ForegroundGreen = 0x0002;

    /// <summary>Text color contains red.</summary>
    public const ushort ForegroundRed = 0x0004;

    /// <summary>Text color is intensified (bright).</summary>
    public const ushort ForegroundIntensity = 0x0008;

    /// <summary>Background color contains blue.</summary>
    public const ushort BackgroundBlue = 0x0010;

    /// <summary>Background color contains green.</summary>
    public const ushort BackgroundGreen = 0x0020;

    /// <summary>Background color contains red.</summary>
    public const ushort BackgroundRed = 0x0040;

    /// <summary>Background color is intensified (bright).</summary>
    public const ushort BackgroundIntensity = 0x0080;
}
