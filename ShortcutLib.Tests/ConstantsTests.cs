using Xunit;

namespace ShortcutLib.Tests;

public class ConstantsTests
{
    // --- DriveTypes value verification ---

    [Fact]
    public void DriveTypes_Fixed_Is3()
    {
        Assert.Equal(3u, DriveTypes.Fixed);
    }

    [Fact]
    public void DriveTypes_CDRom_Is5()
    {
        Assert.Equal(5u, DriveTypes.CDRom);
    }

    [Fact]
    public void DriveTypes_RamDisk_Is6()
    {
        Assert.Equal(6u, DriveTypes.RamDisk);
    }

    // --- CsidlFolderIds value verification ---

    [Fact]
    public void CsidlFolderIds_Windows_Is0x0024()
    {
        Assert.Equal(0x0024u, CsidlFolderIds.Windows);
    }

    [Fact]
    public void CsidlFolderIds_System_Is0x0025()
    {
        Assert.Equal(0x0025u, CsidlFolderIds.System);
    }

    [Fact]
    public void CsidlFolderIds_Desktop_Is0x0000()
    {
        Assert.Equal(0x0000u, CsidlFolderIds.Desktop);
    }

    [Fact]
    public void CsidlFolderIds_ProgramFiles_Is0x0026()
    {
        Assert.Equal(0x0026u, CsidlFolderIds.ProgramFiles);
    }

    // --- VirtualKeys value verification ---

    [Fact]
    public void VirtualKeys_A_Is0x41()
    {
        Assert.Equal(0x41, VirtualKeys.A);
    }

    [Fact]
    public void VirtualKeys_Z_Is0x5A()
    {
        Assert.Equal(0x5A, VirtualKeys.Z);
    }

    [Fact]
    public void VirtualKeys_F1_Is0x70()
    {
        Assert.Equal(0x70, VirtualKeys.F1);
    }

    [Fact]
    public void VirtualKeys_F24_Is0x87()
    {
        Assert.Equal(0x87, VirtualKeys.F24);
    }

    [Fact]
    public void VirtualKeys_D0_Is0x30()
    {
        Assert.Equal(0x30, VirtualKeys.D0);
    }

    [Fact]
    public void VirtualKeys_NumLock_Is0x90()
    {
        Assert.Equal(0x90, VirtualKeys.NumLock);
    }

    // --- ConsoleFillAttributes value verification ---

    [Fact]
    public void ConsoleFillAttributes_ForegroundRed_Is0x0004()
    {
        Assert.Equal((ushort)0x0004, ConsoleFillAttributes.ForegroundRed);
    }

    [Fact]
    public void ConsoleFillAttributes_BackgroundBlue_Is0x0010()
    {
        Assert.Equal((ushort)0x0010, ConsoleFillAttributes.BackgroundBlue);
    }

    [Fact]
    public void ConsoleFillAttributes_Combinable()
    {
        ushort combined = ConsoleFillAttributes.ForegroundRed | ConsoleFillAttributes.BackgroundBlue;
        Assert.Equal((ushort)0x0014, combined);
    }

    // --- ConsoleFontFamilies value verification ---

    [Fact]
    public void ConsoleFontFamilies_Modern_Is0x0030()
    {
        Assert.Equal(0x0030u, ConsoleFontFamilies.Modern);
    }

    [Fact]
    public void ConsoleFontFamilies_TrueType_Is0x04()
    {
        Assert.Equal(0x04u, ConsoleFontFamilies.TrueType);
    }

    [Fact]
    public void ConsoleFontFamilies_Combinable()
    {
        uint combined = ConsoleFontFamilies.Modern | ConsoleFontFamilies.TrueType;
        Assert.Equal(0x0034u, combined);
    }

    // --- ShimLayerNames value verification ---

    [Fact]
    public void ShimLayerNames_WinXPSP3_IsCorrectString()
    {
        Assert.Equal("WinXPSP3", ShimLayerNames.WinXPSP3);
    }

    [Fact]
    public void ShimLayerNames_HighDpiAware_IsCorrectString()
    {
        Assert.Equal("HIGHDPIAWARE", ShimLayerNames.HighDpiAware);
    }

    // --- NetworkProviderTypes expanded values ---

    [Fact]
    public void NetworkProviderTypes_Dav_Is0x002E0000()
    {
        Assert.Equal(0x002E0000u, NetworkProviderTypes.Dav);
    }

    [Fact]
    public void NetworkProviderTypes_Zenworks_Is0x003D0000()
    {
        Assert.Equal(0x003D0000u, NetworkProviderTypes.Zenworks);
    }

    // --- Round-trip tests using named constants ---

    [Fact]
    public void DriveTypes_Fixed_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo
                {
                    BasePath = @"C:\test.exe",
                    DriveType = DriveTypes.CDRom,
                    VolumeLabel = "DVD"
                }
            }
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.LinkInfo?.Local);
        Assert.Equal(DriveTypes.CDRom, options.LinkInfo!.Local!.DriveType);
    }

    [Fact]
    public void CsidlFolderIds_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Windows\notepad.exe",
            SpecialFolder = new SpecialFolderData { FolderId = CsidlFolderIds.Windows }
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.SpecialFolder);
        Assert.Equal(CsidlFolderIds.Windows, options.SpecialFolder!.FolderId);
    }

    [Fact]
    public void VirtualKeys_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            HotkeyKey = VirtualKeys.F5,
            HotkeyModifiers = HotkeyModifiers.Control | HotkeyModifiers.Alt
        });
        var options = Shortcut.Open(lnk);
        Assert.Equal(VirtualKeys.F5, options.HotkeyKey);
    }

    [Fact]
    public void ConsoleFillAttributes_RoundTrips()
    {
        ushort fill = ConsoleFillAttributes.ForegroundRed | ConsoleFillAttributes.ForegroundIntensity
                    | ConsoleFillAttributes.BackgroundBlue;
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Windows\System32\cmd.exe",
            Console = new ConsoleData { FillAttributes = fill }
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.Console);
        Assert.Equal(fill, options.Console!.FillAttributes);
    }

    [Fact]
    public void ShimLayerNames_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\OldApp\app.exe",
            ShimLayerName = ShimLayerNames.WinXPSP3
        });
        var options = Shortcut.Open(lnk);
        Assert.Equal(ShimLayerNames.WinXPSP3, options.ShimLayerName);
    }

    [Fact]
    public void KnownFolderIds_NewEntries_RoundTrip()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Users\Downloads\file.txt",
            KnownFolder = new KnownFolderData { FolderId = KnownFolderIds.Downloads }
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.KnownFolder);
        Assert.Equal(KnownFolderIds.Downloads, options.KnownFolder!.FolderId);
    }

    [Fact]
    public void KnownFolderIds_RecycleBin_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            KnownFolder = new KnownFolderData { FolderId = KnownFolderIds.RecycleBin }
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.KnownFolder);
        Assert.Equal(KnownFolderIds.RecycleBin, options.KnownFolder!.FolderId);
    }
}
