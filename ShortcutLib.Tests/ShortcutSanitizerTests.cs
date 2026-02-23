using Xunit;

namespace ShortcutLib.Tests;

public class ShortcutSanitizerTests
{
    [Fact]
    public void Sanitize_RemovesTracker()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            Tracker = new TrackerData
            {
                MachineId = "TESTPC",
                VolumeId = Guid.NewGuid(),
                ObjectId = Guid.NewGuid()
            }
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.Null(options.Tracker);
    }

    [Fact]
    public void Sanitize_RemovesPropertyStoreData()
    {
        var builder = new PropertyStoreBuilder { AppUserModelId = "Test.App" };
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            PropertyStoreData = builder.Build()
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.Null(options.PropertyStoreData);
    }

    [Fact]
    public void Sanitize_RemovesOverlayData()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            OverlayData = new byte[] { 0x01, 0x02, 0x03 }
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.Null(options.OverlayData);
    }

    [Fact]
    public void Sanitize_PreservesTarget()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\Windows\notepad.exe",
            Tracker = new TrackerData
            {
                MachineId = "PC",
                VolumeId = Guid.NewGuid(),
                ObjectId = Guid.NewGuid()
            }
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.Equal(@"C:\Windows\notepad.exe", options.Target);
    }

    [Fact]
    public void Sanitize_PreservesLinkInfo()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo
                {
                    BasePath = @"C:\test.exe",
                    DriveType = DriveTypes.Fixed
                }
            },
            Tracker = new TrackerData
            {
                MachineId = "PC",
                VolumeId = Guid.NewGuid(),
                ObjectId = Guid.NewGuid()
            }
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.NotNull(options.LinkInfo);
        Assert.Equal(@"C:\test.exe", options.LinkInfo!.Local!.BasePath);
    }

    [Fact]
    public void Sanitize_PreservesConsole()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\Windows\System32\cmd.exe",
            Console = new ConsoleData { ScreenBufferSizeX = 120, WindowSizeX = 120 },
            Tracker = new TrackerData
            {
                MachineId = "PC",
                VolumeId = Guid.NewGuid(),
                ObjectId = Guid.NewGuid()
            }
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.NotNull(options.Console);
        Assert.Equal(120, options.Console!.ScreenBufferSizeX);
    }

    [Fact]
    public void Sanitize_NullOptions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ShortcutSanitizer.Sanitize(null!));
    }

    [Fact]
    public void SanitizeBytes_RoundTrip_ProducesCleanLnk()
    {
        // Create an LNK with tracker data
        byte[] original = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Windows\notepad.exe",
            Tracker = new TrackerData
            {
                MachineId = "SECRETPC",
                VolumeId = Guid.NewGuid(),
                ObjectId = Guid.NewGuid()
            },
            PropertyStoreData = new PropertyStoreBuilder { AppUserModelId = "Test" }.Build()
        });

        // Sanitize
        byte[] sanitized = ShortcutSanitizer.SanitizeBytes(original);

        // Re-open and verify
        var options = Shortcut.Open(sanitized);
        Assert.Equal(@"C:\Windows\notepad.exe", options.Target);
        Assert.Null(options.Tracker);
        Assert.Null(options.PropertyStoreData);
    }

    [Fact]
    public void SanitizeBytes_NullData_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ShortcutSanitizer.SanitizeBytes(null!));
    }
}
