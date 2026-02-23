namespace ShortcutLib;

/// <summary>
/// Well-known folder GUIDs (KNOWNFOLDERID) for use with KnownFolderDataBlock.
/// See: https://learn.microsoft.com/windows/win32/shell/knownfolderid
/// </summary>
public static class KnownFolderIds
{
    // User folders

    /// <summary>Desktop folder for the current user.</summary>
    public static readonly Guid Desktop = new("B4BFCC3A-DB2C-424C-B029-7FE99A87C641");

    /// <summary>Documents folder for the current user.</summary>
    public static readonly Guid Documents = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7");

    /// <summary>Downloads folder for the current user.</summary>
    public static readonly Guid Downloads = new("374DE290-123F-4565-9164-39C4925E467B");

    /// <summary>Music folder for the current user.</summary>
    public static readonly Guid Music = new("4BD8D571-6D19-48D3-BE97-422220080E43");

    /// <summary>Pictures folder for the current user.</summary>
    public static readonly Guid Pictures = new("33E28130-4E1E-4676-835A-98395C3BC3BB");

    /// <summary>Videos folder for the current user.</summary>
    public static readonly Guid Videos = new("18989B1D-99B5-455B-841C-AB7C74E4DDFC");

    /// <summary>User's profile folder (%USERPROFILE%).</summary>
    public static readonly Guid Profile = new("5E6C858F-0E22-4760-9AFE-EA3317B67173");

    /// <summary>Saved Games folder.</summary>
    public static readonly Guid SavedGames = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4");

    /// <summary>Contacts folder.</summary>
    public static readonly Guid Contacts = new("56784854-C6CB-462B-8169-88E350ACB882");

    /// <summary>Searches folder (saved search queries).</summary>
    public static readonly Guid Searches = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA");

    /// <summary>Favorites folder.</summary>
    public static readonly Guid Favorites = new("1777F761-68AD-4D8A-87BD-30B759FA33DD");

    /// <summary>Links folder (Explorer favorites).</summary>
    public static readonly Guid Links = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968");

    /// <summary>Templates folder for the current user.</summary>
    public static readonly Guid Templates = new("A63293E8-664E-48DB-A079-DF759E0509F7");

    // System folders

    /// <summary>Program Files folder.</summary>
    public static readonly Guid ProgramFiles = new("905E63B6-C1BF-494E-B29C-65B732D3D21A");

    /// <summary>Program Files (x86) folder on 64-bit Windows.</summary>
    public static readonly Guid ProgramFilesX86 = new("7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E");

    /// <summary>Common Files folder under Program Files.</summary>
    public static readonly Guid ProgramFilesCommon = new("F7F1ED05-9F6D-47A2-AAAE-29D317C6F066");

    /// <summary>Common Files (x86) folder on 64-bit Windows.</summary>
    public static readonly Guid ProgramFilesCommonX86 = new("DE974D24-D9C6-4D3E-BF91-F4455120B917");

    /// <summary>System32 directory.</summary>
    public static readonly Guid System = new("1AC14E77-02E7-4E5D-B744-2EB1AE5198B7");

    /// <summary>System32 (x86) directory on 64-bit Windows.</summary>
    public static readonly Guid SystemX86 = new("D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27");

    /// <summary>Windows directory.</summary>
    public static readonly Guid Windows = new("F38BF404-1D43-42F2-9305-67DE0B28FC23");

    /// <summary>Fonts folder.</summary>
    public static readonly Guid Fonts = new("FD228CB7-AE11-4AE3-864C-16F3910AB8FE");

    // Application Data

    /// <summary>Roaming Application Data folder (%APPDATA%).</summary>
    public static readonly Guid AppData = new("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D");

    /// <summary>Local Application Data folder (%LOCALAPPDATA%).</summary>
    public static readonly Guid LocalAppData = new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");

    /// <summary>Local Application Data Low-integrity folder.</summary>
    public static readonly Guid LocalAppDataLow = new("A520A1A4-1780-4FF6-BD18-167343C5AF16");

    /// <summary>ProgramData folder (%PROGRAMDATA%).</summary>
    public static readonly Guid ProgramData = new("62AB5D82-FDC1-4DC3-A9DD-070D1D495D97");

    // Start menu

    /// <summary>Start Menu folder for the current user.</summary>
    public static readonly Guid StartMenu = new("625B53C3-AB48-4EC1-BA1F-A1EF4146FC19");

    /// <summary>Programs folder inside the current user's Start Menu.</summary>
    public static readonly Guid Programs = new("A77F5D77-2E2B-44C3-A6A2-ABA601054A51");

    /// <summary>Startup folder for the current user.</summary>
    public static readonly Guid Startup = new("B97D20BB-F46A-4C97-BA10-5E3608430854");

    /// <summary>Administrative Tools folder for the current user.</summary>
    public static readonly Guid AdminTools = new("724EF170-A42D-4FEF-9F26-B60E846FBA4F");

    // Common (all-users) folders

    /// <summary>Start Menu folder shared by all users.</summary>
    public static readonly Guid CommonStartMenu = new("A4115719-D62E-491D-AA7C-E74B8BE3B067");

    /// <summary>Programs folder shared by all users.</summary>
    public static readonly Guid CommonPrograms = new("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8");

    /// <summary>Startup folder shared by all users.</summary>
    public static readonly Guid CommonStartup = new("82A5EA35-D9CD-47C5-9629-E15D2F714E6E");

    /// <summary>Desktop directory shared by all users.</summary>
    public static readonly Guid CommonDesktopDir = new("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25");

    /// <summary>Templates folder shared by all users.</summary>
    public static readonly Guid CommonTemplates = new("B94237E7-57AC-4347-9151-B08C6C32D1F7");

    /// <summary>Administrative Tools folder shared by all users.</summary>
    public static readonly Guid CommonAdminTools = new("D0384E7D-BAC3-4797-8F14-CBA229B392B5");

    // Public (shared) folders

    /// <summary>User profiles root folder.</summary>
    public static readonly Guid UserProfiles = new("0762D272-C50A-4BB0-A382-697DCD729B80");

    /// <summary>Public folder.</summary>
    public static readonly Guid Public = new("DFDF76A2-C82A-4D63-906A-5644AC457385");

    /// <summary>Public Desktop folder.</summary>
    public static readonly Guid PublicDesktop = new("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25");

    /// <summary>Public Documents folder.</summary>
    public static readonly Guid PublicDocuments = new("ED4824AF-DCE4-45A8-81E2-FC7965083634");

    /// <summary>Public Downloads folder.</summary>
    public static readonly Guid PublicDownloads = new("3D644C9B-1FB8-4F30-9B45-F670235F79C0");

    /// <summary>Public Music folder.</summary>
    public static readonly Guid PublicMusic = new("3214FAB5-9757-4298-BB61-92A9DEAA44FF");

    /// <summary>Public Pictures folder.</summary>
    public static readonly Guid PublicPictures = new("B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5");

    /// <summary>Public Videos folder.</summary>
    public static readonly Guid PublicVideos = new("2400183A-6185-49FB-A2D8-4A392A602BA3");

    // Shell special locations

    /// <summary>Recycle Bin virtual folder.</summary>
    public static readonly Guid RecycleBin = new("B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC");

    /// <summary>Quick Launch toolbar folder.</summary>
    public static readonly Guid QuickLaunch = new("52A4F021-7B75-48A9-9F6B-4B87A210BC8F");

    /// <summary>SendTo folder.</summary>
    public static readonly Guid SendTo = new("8983036C-27C0-404B-8F08-102D10DCFD74");

    /// <summary>Recent Items folder.</summary>
    public static readonly Guid Recent = new("AE50C081-EBD2-438A-8655-8A092E34987A");

    /// <summary>PrintHood folder (network printer links).</summary>
    public static readonly Guid PrintHood = new("9274BD8D-CFD1-41C3-B35E-B13F55A758F4");

    /// <summary>NetHood folder (network place links).</summary>
    public static readonly Guid NetHood = new("C5ABBF53-E17F-4121-8900-86626FC2C973");

    /// <summary>Cookies folder (Internet Explorer).</summary>
    public static readonly Guid Cookies = new("2B0F765D-C0E9-4171-908E-08A611B84FF6");

    /// <summary>Internet History folder.</summary>
    public static readonly Guid History = new("D9DC8A3B-B784-432E-A781-5A1130A75963");

    /// <summary>Temporary Internet Files (Internet cache) folder.</summary>
    public static readonly Guid InternetCache = new("352481E8-33BE-4251-BA85-6007CAEDCF9D");

    /// <summary>Per-user installed programs folder.</summary>
    public static readonly Guid UserProgramFiles = new("5CD7AEE2-2219-4A67-B85D-6C9CE15660CB");
}
