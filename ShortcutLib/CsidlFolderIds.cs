namespace ShortcutLib;

/// <summary>
/// CSIDL (Constant Special Item ID List) folder ID constants for
/// <see cref="SpecialFolderData.FolderId"/>.
/// </summary>
public static class CsidlFolderIds
{
    /// <summary>Desktop virtual folder (root of the namespace).</summary>
    public const uint Desktop = 0x0000;

    /// <summary>Internet virtual folder.</summary>
    public const uint Internet = 0x0001;

    /// <summary>Programs folder inside the Start menu.</summary>
    public const uint Programs = 0x0002;

    /// <summary>Control Panel virtual folder.</summary>
    public const uint Controls = 0x0003;

    /// <summary>Printers virtual folder.</summary>
    public const uint Printers = 0x0004;

    /// <summary>User's Documents (My Documents) folder.</summary>
    public const uint Personal = 0x0005;

    /// <summary>User's Favorites folder.</summary>
    public const uint Favorites = 0x0006;

    /// <summary>Startup folder for the current user.</summary>
    public const uint Startup = 0x0007;

    /// <summary>Recent documents folder.</summary>
    public const uint Recent = 0x0008;

    /// <summary>SendTo folder.</summary>
    public const uint SendTo = 0x0009;

    /// <summary>Recycle Bin virtual folder.</summary>
    public const uint RecycleBin = 0x000A;

    /// <summary>Start Menu folder for the current user.</summary>
    public const uint StartMenu = 0x000B;

    /// <summary>User's Music folder.</summary>
    public const uint MyMusic = 0x000D;

    /// <summary>User's Videos folder.</summary>
    public const uint MyVideo = 0x000E;

    /// <summary>Desktop directory for the current user.</summary>
    public const uint DesktopDirectory = 0x0010;

    /// <summary>My Computer (drives and mapped network drives).</summary>
    public const uint Drives = 0x0011;

    /// <summary>Network Neighborhood virtual folder.</summary>
    public const uint Network = 0x0012;

    /// <summary>Network Neighborhood\Entire Network.</summary>
    public const uint NetHood = 0x0013;

    /// <summary>Fonts folder.</summary>
    public const uint Fonts = 0x0014;

    /// <summary>Templates folder.</summary>
    public const uint Templates = 0x0015;

    /// <summary>Start Menu folder shared by all users.</summary>
    public const uint CommonStartMenu = 0x0016;

    /// <summary>Programs folder shared by all users.</summary>
    public const uint CommonPrograms = 0x0017;

    /// <summary>Startup folder shared by all users.</summary>
    public const uint CommonStartup = 0x0018;

    /// <summary>Desktop directory shared by all users.</summary>
    public const uint CommonDesktopDirectory = 0x0019;

    /// <summary>Application Data folder (roaming).</summary>
    public const uint AppData = 0x001A;

    /// <summary>PrintHood folder.</summary>
    public const uint PrintHood = 0x001B;

    /// <summary>Local (non-roaming) Application Data folder.</summary>
    public const uint LocalAppData = 0x001C;

    /// <summary>Common Application Data folder (ProgramData).</summary>
    public const uint CommonAppData = 0x0023;

    /// <summary>Windows directory.</summary>
    public const uint Windows = 0x0024;

    /// <summary>System directory (System32).</summary>
    public const uint System = 0x0025;

    /// <summary>Program Files folder.</summary>
    public const uint ProgramFiles = 0x0026;

    /// <summary>User's Pictures folder.</summary>
    public const uint MyPictures = 0x0027;

    /// <summary>User's profile folder.</summary>
    public const uint Profile = 0x0028;

    /// <summary>System directory for x86 on 64-bit Windows.</summary>
    public const uint SystemX86 = 0x0029;

    /// <summary>Program Files (x86) folder on 64-bit Windows.</summary>
    public const uint ProgramFilesX86 = 0x002A;

    /// <summary>Common Files folder under Program Files.</summary>
    public const uint CommonFiles = 0x002B;

    /// <summary>Common Files (x86) folder on 64-bit Windows.</summary>
    public const uint CommonFilesX86 = 0x002C;

    /// <summary>Common Templates folder shared by all users.</summary>
    public const uint CommonTemplates = 0x002D;

    /// <summary>Common Documents folder shared by all users.</summary>
    public const uint CommonDocuments = 0x002E;

    /// <summary>Administrative Tools folder for the current user.</summary>
    public const uint AdminTools = 0x0030;

    /// <summary>Administrative Tools folder shared by all users.</summary>
    public const uint CommonAdminTools = 0x002F;

    /// <summary>Per-user Cookies folder (Internet Explorer).</summary>
    public const uint Cookies = 0x0021;

    /// <summary>Per-user Internet History folder.</summary>
    public const uint History = 0x0022;

    /// <summary>Per-user Temporary Internet Files folder.</summary>
    public const uint InternetCache = 0x0020;
}
