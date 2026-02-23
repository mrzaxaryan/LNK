namespace ShortcutLib;

/// <summary>
/// Data for TrackerDataBlock (signature 0xA0000003).
/// Used by the distributed link tracking service.
/// Includes forensic extraction methods for MAC address and timestamp from Version 1 UUIDs.
/// </summary>
public sealed class TrackerData
{
    /// <summary>NetBIOS machine name (max 15 characters, null-padded to 16 bytes).</summary>
    public string MachineId { get; set; } = "";

    /// <summary>Volume GUID (Droid[0]).</summary>
    public Guid VolumeId { get; set; }

    /// <summary>Object GUID (Droid[1]).</summary>
    public Guid ObjectId { get; set; }

    /// <summary>Birth volume GUID (DroidBirth[0]). Defaults to VolumeId if null.</summary>
    public Guid? BirthVolumeId { get; set; }

    /// <summary>Birth object GUID (DroidBirth[1]). Defaults to ObjectId if null.</summary>
    public Guid? BirthObjectId { get; set; }

    // --- Forensic extraction methods ---

    /// <summary>Returns true if ObjectId is a Version 1 (time-based) UUID.</summary>
    public bool IsObjectIdVersion1() => IsVersion1(ObjectId);

    /// <summary>Returns true if BirthObjectId (or ObjectId as fallback) is a Version 1 UUID.</summary>
    public bool IsBirthObjectIdVersion1() => IsVersion1(BirthObjectId ?? ObjectId);

    /// <summary>
    /// Extracts the MAC address from ObjectId if it is a Version 1 UUID.
    /// Returns null if not a Version 1 UUID.
    /// </summary>
    public byte[]? ExtractMacAddress() => ExtractMacFromGuid(ObjectId);

    /// <summary>
    /// Extracts the MAC address from BirthObjectId if it is a Version 1 UUID.
    /// </summary>
    public byte[]? ExtractBirthMacAddress() => ExtractMacFromGuid(BirthObjectId ?? ObjectId);

    /// <summary>
    /// Formats the MAC address from ObjectId as "AA:BB:CC:DD:EE:FF".
    /// Returns null if not a Version 1 UUID.
    /// </summary>
    public string? ExtractMacAddressString()
    {
        var mac = ExtractMacAddress();
        return mac != null ? string.Join(":", mac.Select(b => b.ToString("X2"))) : null;
    }

    /// <summary>
    /// Formats the MAC address from BirthObjectId as "AA:BB:CC:DD:EE:FF".
    /// </summary>
    public string? ExtractBirthMacAddressString()
    {
        var mac = ExtractBirthMacAddress();
        return mac != null ? string.Join(":", mac.Select(b => b.ToString("X2"))) : null;
    }

    /// <summary>
    /// Extracts the timestamp from ObjectId if it is a Version 1 UUID.
    /// Returns null if not a Version 1 UUID.
    /// </summary>
    public DateTime? ExtractTimestamp() => ExtractTimestampFromGuid(ObjectId);

    /// <summary>
    /// Extracts the timestamp from BirthObjectId if it is a Version 1 UUID.
    /// </summary>
    public DateTime? ExtractBirthTimestamp() => ExtractTimestampFromGuid(BirthObjectId ?? ObjectId);

    /// <summary>
    /// Checks if the given GUID is a Version 1 UUID (time-based).
    /// The version nibble is bits 4-7 of byte 7 in .NET's GUID binary layout.
    /// </summary>
    private static bool IsVersion1(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        return (bytes[7] >> 4) == 1;
    }

    private static byte[]? ExtractMacFromGuid(Guid guid)
    {
        if (!IsVersion1(guid)) return null;
        byte[] bytes = guid.ToByteArray();
        // In .NET's GUID byte layout, the node (MAC) occupies bytes 10-15
        return [bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15]];
    }

    private static DateTime? ExtractTimestampFromGuid(Guid guid)
    {
        if (!IsVersion1(guid)) return null;
        byte[] bytes = guid.ToByteArray();

        // Reconstruct the 60-bit UUID timestamp from .NET GUID byte layout:
        // time_low: bytes 0-3 (little-endian uint32)
        // time_mid: bytes 4-5 (little-endian uint16)
        // time_hi_and_version: bytes 6-7 (little-endian uint16, high nibble is version)
        uint timeLow = BitConverter.ToUInt32(bytes, 0);
        ushort timeMid = BitConverter.ToUInt16(bytes, 4);
        ushort timeHiAndVersion = BitConverter.ToUInt16(bytes, 6);
        ushort timeHi = (ushort)(timeHiAndVersion & 0x0FFF); // mask out version nibble

        long timestamp = ((long)timeHi << 48) | ((long)timeMid << 32) | timeLow;

        // UUID epoch is October 15, 1582; timestamp is 100-nanosecond intervals
        DateTime uuidEpoch = new(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        return uuidEpoch.AddTicks(timestamp);
    }
}
