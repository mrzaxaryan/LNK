using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class TrackerDataForensicTests
{
    // A known Version 1 UUID with known MAC and timestamp
    // UUID v1: time_low=0x01020304, time_mid=0x0506, time_hi_ver=0x1078 (version=1, hi=0x078)
    // node=AA:BB:CC:DD:EE:FF
    private static Guid CreateV1Guid(byte[] mac, DateTime timestamp)
    {
        // Convert timestamp to UUID ticks (100ns since 1582-10-15)
        DateTime epoch = new(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        long ticks = (timestamp - epoch).Ticks;

        uint timeLow = (uint)(ticks & 0xFFFFFFFF);
        ushort timeMid = (ushort)((ticks >> 32) & 0xFFFF);
        ushort timeHi = (ushort)(((ticks >> 48) & 0x0FFF) | 0x1000); // version 1

        byte[] bytes = new byte[16];
        BitConverter.GetBytes(timeLow).CopyTo(bytes, 0);
        BitConverter.GetBytes(timeMid).CopyTo(bytes, 4);
        BitConverter.GetBytes(timeHi).CopyTo(bytes, 6);
        bytes[8] = 0x80; // variant
        bytes[9] = 0x00; // clock seq
        mac.CopyTo(bytes, 10);

        return new Guid(bytes);
    }

    [Fact]
    public void IsObjectIdVersion1_True_ForV1Guid()
    {
        byte[] mac = [0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF];
        var v1 = CreateV1Guid(mac, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var tracker = new TrackerData { ObjectId = v1 };
        Assert.True(tracker.IsObjectIdVersion1());
    }

    [Fact]
    public void IsObjectIdVersion1_False_ForV4Guid()
    {
        var tracker = new TrackerData { ObjectId = Guid.NewGuid() }; // V4
        Assert.False(tracker.IsObjectIdVersion1());
    }

    [Fact]
    public void ExtractMacAddress_ReturnsCorrectMac()
    {
        byte[] expectedMac = [0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF];
        var v1 = CreateV1Guid(expectedMac, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var tracker = new TrackerData { ObjectId = v1 };

        byte[]? mac = tracker.ExtractMacAddress();
        Assert.NotNull(mac);
        Assert.Equal(expectedMac, mac);
    }

    [Fact]
    public void ExtractMacAddress_ReturnsNull_ForV4()
    {
        var tracker = new TrackerData { ObjectId = Guid.NewGuid() };
        Assert.Null(tracker.ExtractMacAddress());
    }

    [Fact]
    public void ExtractMacAddressString_FormatsCorrectly()
    {
        byte[] mac = [0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF];
        var v1 = CreateV1Guid(mac, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var tracker = new TrackerData { ObjectId = v1 };

        Assert.Equal("AA:BB:CC:DD:EE:FF", tracker.ExtractMacAddressString());
    }

    [Fact]
    public void ExtractTimestamp_ReturnsApproximateDateTime()
    {
        var expected = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        byte[] mac = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
        var v1 = CreateV1Guid(mac, expected);
        var tracker = new TrackerData { ObjectId = v1 };

        DateTime? extracted = tracker.ExtractTimestamp();
        Assert.NotNull(extracted);
        // Should be very close to expected (within a second due to tick precision)
        Assert.True(Math.Abs((extracted.Value - expected).TotalSeconds) < 1);
    }

    [Fact]
    public void ExtractTimestamp_ReturnsNull_ForV4()
    {
        var tracker = new TrackerData { ObjectId = Guid.NewGuid() };
        Assert.Null(tracker.ExtractTimestamp());
    }

    [Fact]
    public void BirthMac_UsesBirthObjectId()
    {
        byte[] mac1 = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
        byte[] mac2 = [0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF];
        var dt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var tracker = new TrackerData
        {
            ObjectId = CreateV1Guid(mac1, dt),
            BirthObjectId = CreateV1Guid(mac2, dt)
        };

        Assert.Equal(mac1, tracker.ExtractMacAddress());
        Assert.Equal(mac2, tracker.ExtractBirthMacAddress());
    }

    [Fact]
    public void BirthMac_FallsBackToObjectId()
    {
        byte[] mac = [0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF];
        var dt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var tracker = new TrackerData
        {
            ObjectId = CreateV1Guid(mac, dt),
            BirthObjectId = null
        };

        Assert.Equal(mac, tracker.ExtractBirthMacAddress());
    }
}
