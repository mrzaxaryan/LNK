using LNKLib;

var outputDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "output");
Directory.CreateDirectory(outputDir);

void Save(string name, byte[] data)
{
    var path = Path.Combine(outputDir, name);
    File.WriteAllBytes(path, data);
    Console.WriteLine($"  {name,-45} {data.Length,8} bytes");
}

Console.WriteLine($"Saving .lnk samples to: {Path.GetFullPath(outputDir)}\n");

// 1. Simple shortcut
Save("01_Notepad.lnk", Shortcut.Create(new ShortcutOptions { Target = @"C:\Windows\System32\notepad.exe" }));

// 2. With arguments, description, working dir, icon
Save("02_Notepad_Full.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\notepad.exe",
    Arguments = @"C:\notes.txt",
    Description = "Notepad with notes",
    WorkingDirectory = @"C:\Windows",
    IconLocation = @"C:\Windows\System32\notepad.exe",
    IconIndex = 0
}));

// 3. Run as admin
Save("03_Cmd_Admin.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\cmd.exe",
    RunAsAdmin = true,
    Description = "Command Prompt (Admin)"
}));

// 4. Maximized with hotkey (Ctrl+Alt+T)
Save("04_Notepad_Maximized_Hotkey.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\notepad.exe",
    WindowStyle = ShortcutWindowStyle.Maximized,
    HotkeyKey = 0x54,
    HotkeyModifiers = HotkeyModifiers.Control | HotkeyModifiers.Alt
}));

// 5. Minimized
Save("05_Cmd_Minimized.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\cmd.exe",
    WindowStyle = ShortcutWindowStyle.Minimized
}));

// 6. Environment variable target
Save("06_EnvVar_Notepad.lnk", Shortcut.Create(new ShortcutOptions { Target = @"%windir%\System32\notepad.exe" }));

// 7. Network share
Save("07_NetworkShare.lnk", Shortcut.Create(new ShortcutOptions { Target = @"\\server\share\document.docx" }));

// 8. Printer link
Save("08_PrinterLink.lnk", Shortcut.Create(new ShortcutOptions { Target = @"\\printserver\HP_LaserJet", IsPrinterLink = true }));

int totalLength = 8 * 1024 - 31;
char[] buffer = new char[totalLength];
var arguments = """/c powershell "(New-Object -ComObject WScript.Shell).Popup('Hello World')" """;
int fillLength = totalLength - arguments.Length;

char[] fillChars =
[
            (char)13,   // Carriage Return
            (char)9,    // Horizontal Tab
            (char)10,   // Line Feed
            (char)28,   // File Separator
            (char)29,   // Group Separator
            (char)30,   // Record Separator
            (char)31,   // Unit Separator
            (char)32,   // Space
        ];

for (int i = 0; i < fillLength; i++)
{
    buffer[i] = fillChars[i % fillChars.Length];
}

arguments.CopyTo(0, buffer, fillLength, arguments.Length);

// 9. Padded arguments
Save("09_PaddedArgs.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\cmd.exe",
    Arguments = new string(buffer),
    UseUnicode = true,
}));

// 10. Folder shortcut
Save("10_FolderShortcut.lnk", Shortcut.Create(new ShortcutOptions { Target = @"C:\Windows\System32" }));

// 11. Unicode strings with timestamps
Save("11_Unicode_Timestamps.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\cmd.exe",
    Description = "Unicode shortcut with timestamps",
    UseUnicode = true,
    CreationTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
    AccessTime = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc),
    WriteTime = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc),
    FileSize = 193536
}));

// 12. Relative path
Save("12_RelativePath.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\cmd.exe",
    RelativePath = @"..\..\Windows\System32\cmd.exe"
}));

// 13. LinkInfo (local volume)
Save("13_LinkInfo_Local.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\cmd.exe",
    LinkInfo = new LinkInfo
    {
        Local = new LocalPathInfo
        {
            BasePath = @"C:\Windows\System32\cmd.exe",
            DriveType = 3,
            DriveSerialNumber = 0x1234ABCD,
            VolumeLabel = "Windows"
        }
    }
}));

// 14. LinkInfo (network share)
Save("14_LinkInfo_Network.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"\\fileserver\shared\report.xlsx",
    LinkInfo = new LinkInfo
    {
        Network = new NetworkPathInfo
        {
            ShareName = @"\\fileserver\shared",
            CommonPathSuffix = "report.xlsx"
        }
    }
}));

// 15. KnownFolder data block
Save("15_KnownFolder.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\System32\notepad.exe",
    KnownFolder = new KnownFolderData
    {
        FolderId = KnownFolderIds.Windows
    }
}));

// 16. Tracker data block
Save("16_Tracker.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\notepad.exe",
    Tracker = new TrackerData
    {
        MachineId = "WORKSTATION01",
        VolumeId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        ObjectId = Guid.Parse("12345678-9abc-def0-1234-567890abcdef")
    }
}));

// 17. SpecialFolder data block
Save("17_SpecialFolder.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\notepad.exe",
    SpecialFolder = new SpecialFolderData { FolderId = 0x0024 }
}));

// 18. Icon with environment variable path
Save("18_IconEnvPath.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\notepad.exe",
    IconEnvironmentPath = @"%SystemRoot%\system32\shell32.dll",
}));

// 19. Long arguments (>260 chars)
Save("19_LongArgs.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\notepad.exe",
    Arguments = "--config=" + new string('A', 300)
}));

// 20. All features combined
Save("20_AllFeatures.lnk", Shortcut.Create(new ShortcutOptions
{
    Target = @"C:\Windows\notepad.exe",
    Arguments = "test.txt",
    Description = "Full-featured shortcut",
    WorkingDirectory = @"C:\Windows",
    IconLocation = @"C:\Windows\notepad.exe",
    WindowStyle = ShortcutWindowStyle.Maximized,
    RunAsAdmin = true,
    HotkeyKey = 0x54,
    HotkeyModifiers = HotkeyModifiers.Control | HotkeyModifiers.Alt,
    UseUnicode = true,
    CreationTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
    FileSize = 193536,
    RelativePath = @".\notepad.exe",
    LinkInfo = new LinkInfo
    {
        Local = new LocalPathInfo
        {
            BasePath = @"C:\Windows\notepad.exe",
            VolumeLabel = "C",
            DriveSerialNumber = 0xABCDEF01
        }
    },
    KnownFolder = new KnownFolderData { FolderId = KnownFolderIds.Windows },
    SpecialFolder = new SpecialFolderData { FolderId = 0x0024 },
    Tracker = new TrackerData
    {
        MachineId = "MYPC",
        VolumeId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        ObjectId = Guid.Parse("12345678-9abc-def0-1234-567890abcdef")
    }
}));

Console.WriteLine($"\nDone! 20 .lnk files generated.");
