namespace ShortcutLib;

/// <summary>
/// Common Windows Application Compatibility shim layer names for
/// <see cref="ShortcutOptions.ShimLayerName"/>.
/// </summary>
public static class ShimLayerNames
{
    /// <summary>Windows XP SP3 compatibility mode.</summary>
    public const string WinXPSP3 = "WinXPSP3";

    /// <summary>Windows Vista SP2 compatibility mode.</summary>
    public const string WinVistaSP2 = "WinVistaSP2";

    /// <summary>Windows 7 RTM compatibility mode.</summary>
    public const string Win7RTM = "Win7RTM";

    /// <summary>Windows 8 RTM compatibility mode.</summary>
    public const string Win8RTM = "Win8RTM";

    /// <summary>Reduce color mode to 256 colors.</summary>
    public const string Color256 = "256COLOR";

    /// <summary>Run in 640x480 screen resolution.</summary>
    public const string Resolution640x480 = "640X480";

    /// <summary>Disable Data Execution Prevention (show UI).</summary>
    public const string DisableNXShowUI = "DISABLENXSHOWUI";

    /// <summary>Mark application as high-DPI aware.</summary>
    public const string HighDpiAware = "HIGHDPIAWARE";

    /// <summary>Disable visual themes.</summary>
    public const string DisableThemes = "DISABLETHEMES";

    /// <summary>Run the application as administrator.</summary>
    public const string RunAsAdmin = "RUNASADMIN";

    /// <summary>Force DirectDraw emulation.</summary>
    public const string ForceDirectDrawEmulation = "Layer_ForceDirectDrawEmulation";

    /// <summary>Windows XP SP2 compatibility mode.</summary>
    public const string WinXPSP2 = "WinXPSP2";
}
