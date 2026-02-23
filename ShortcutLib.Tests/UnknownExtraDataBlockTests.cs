using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class UnknownExtraDataBlockTests
{
    [Fact]
    public void UnknownBlock_RoundTrips()
    {
        // Create a shortcut, manually inject an unknown block, verify round-trip
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            UnknownExtraDataBlocks =
            [
                new RawExtraDataBlock { Signature = 0xA00000FF, Data = [0xDE, 0xAD, 0xBE, 0xEF] }
            ]
        };

        byte[] lnk = Shortcut.Create(options);
        var parsed = Shortcut.Open(lnk);

        Assert.NotNull(parsed.UnknownExtraDataBlocks);
        Assert.Single(parsed.UnknownExtraDataBlocks);
        Assert.Equal(0xA00000FFu, parsed.UnknownExtraDataBlocks[0].Signature);
        Assert.Equal([0xDE, 0xAD, 0xBE, 0xEF], parsed.UnknownExtraDataBlocks[0].Data);
    }

    [Fact]
    public void MultipleUnknownBlocks_RoundTrip()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            UnknownExtraDataBlocks =
            [
                new RawExtraDataBlock { Signature = 0xA00000FE, Data = [0x01, 0x02] },
                new RawExtraDataBlock { Signature = 0xA00000FD, Data = [0x03, 0x04, 0x05] }
            ]
        };

        byte[] lnk = Shortcut.Create(options);
        var parsed = Shortcut.Open(lnk);

        Assert.NotNull(parsed.UnknownExtraDataBlocks);
        Assert.Equal(2, parsed.UnknownExtraDataBlocks.Count);
        Assert.Equal(0xA00000FEu, parsed.UnknownExtraDataBlocks[0].Signature);
        Assert.Equal(0xA00000FDu, parsed.UnknownExtraDataBlocks[1].Signature);
    }

    [Fact]
    public void KnownBlocks_StillParsed_AlongsideUnknown()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            Tracker = new TrackerData
            {
                MachineId = "TESTPC",
                VolumeId = Guid.NewGuid(),
                ObjectId = Guid.NewGuid()
            },
            UnknownExtraDataBlocks =
            [
                new RawExtraDataBlock { Signature = 0xA00000FF, Data = [0xAB] }
            ]
        };

        byte[] lnk = Shortcut.Create(options);
        var parsed = Shortcut.Open(lnk);

        Assert.NotNull(parsed.Tracker);
        Assert.Equal("TESTPC", parsed.Tracker.MachineId);
        Assert.NotNull(parsed.UnknownExtraDataBlocks);
        Assert.Single(parsed.UnknownExtraDataBlocks);
    }

    [Fact]
    public void Sanitizer_StripsUnknownBlocks()
    {
        var options = new ShortcutOptions
        {
            Target = @"C:\test.exe",
            UnknownExtraDataBlocks =
            [
                new RawExtraDataBlock { Signature = 0xA00000FF, Data = [0x01] }
            ]
        };

        ShortcutSanitizer.Sanitize(options);
        Assert.Null(options.UnknownExtraDataBlocks);
    }

    [Fact]
    public void NoUnknownBlocks_PropertyIsNull()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions { Target = @"C:\test.exe" });
        var parsed = Shortcut.Open(lnk);
        Assert.Null(parsed.UnknownExtraDataBlocks);
    }
}
