using System.Text;

namespace LNKLib;

internal static class ExtraDataBlockWriter
{
    /// <summary>
    /// Writes the LinkInfo structure per [MS-SHLLINK] 2.3.
    /// </summary>
    internal static void WriteLinkInfo(BinaryWriter writer, LinkInfo info)
    {
        bool hasLocal = info.Local is not null;
        bool hasNetwork = info.Network is not null;

        int flags = 0;
        if (hasLocal) flags |= 0x01; // VolumeIDAndLocalBasePath
        if (hasNetwork) flags |= 0x02; // CommonNetworkRelativeLinkAndPathSuffix

        // LinkInfoHeaderSize = 0x1C (28 bytes) for the basic header
        const int headerSize = 0x1C;

        // Pre-compute VolumeID structure if local
        byte[]? volumeIdBytes = null;
        byte[]? localBasePathBytes = null;
        if (hasLocal)
        {
            var local = info.Local!;
            byte[] volumeLabelAnsi = Encoding.Default.GetBytes(local.VolumeLabel + "\0");
            int volumeIdSize = 4 + 4 + 4 + 4 + volumeLabelAnsi.Length; // size + driveType + serialNum + labelOffset + label
            using var volMs = new MemoryStream();
            using var volW = new BinaryWriter(volMs);
            volW.Write(volumeIdSize);
            volW.Write(local.DriveType);
            volW.Write(local.DriveSerialNumber);
            volW.Write(0x10); // VolumeLabelOffset = 16 (offset within VolumeID structure)
            volW.Write(volumeLabelAnsi);
            volW.Flush();
            volumeIdBytes = volMs.ToArray();

            localBasePathBytes = Encoding.Default.GetBytes(local.BasePath + "\0");
        }

        // Pre-compute CommonNetworkRelativeLink if network
        byte[]? cnrlBytes = null;
        byte[]? commonPathSuffixBytes = null;
        if (hasNetwork)
        {
            var network = info.Network!;
            byte[] shareNameAnsi = Encoding.Default.GetBytes(network.ShareName + "\0");
            // CommonNetworkRelativeLink: size(4) + flags(4) + netNameOffset(4) + deviceNameOffset(4) + networkProviderType(4) + netName
            int cnrlSize = 4 + 4 + 4 + 4 + 4 + shareNameAnsi.Length;
            using var cnrlMs = new MemoryStream();
            using var cnrlW = new BinaryWriter(cnrlMs);
            cnrlW.Write(cnrlSize);
            cnrlW.Write(0); // CommonNetworkRelativeLinkFlags = 0
            cnrlW.Write(0x14); // NetNameOffset = 20 (offset within CNRL)
            cnrlW.Write(0); // DeviceNameOffset = 0
            cnrlW.Write(0x00020000); // NetworkProviderType = WNNC_NET_LANMAN
            cnrlW.Write(shareNameAnsi);
            cnrlW.Flush();
            cnrlBytes = cnrlMs.ToArray();

            commonPathSuffixBytes = Encoding.Default.GetBytes(network.CommonPathSuffix + "\0");
        }

        // Compute offsets from start of LinkInfo
        int volumeIdOffset = 0;
        int localBasePathOffset = 0;
        int cnrlOffset = 0;
        int commonPathSuffixOffset = 0;

        int currentOffset = headerSize;

        if (hasLocal)
        {
            volumeIdOffset = currentOffset;
            currentOffset += volumeIdBytes!.Length;
            localBasePathOffset = currentOffset;
            currentOffset += localBasePathBytes!.Length;
        }

        if (hasNetwork)
        {
            cnrlOffset = currentOffset;
            currentOffset += cnrlBytes!.Length;
            commonPathSuffixOffset = currentOffset;
            currentOffset += commonPathSuffixBytes!.Length;
        }
        else
        {
            // CommonPathSuffix is always present, even for local-only
            commonPathSuffixOffset = currentOffset;
            commonPathSuffixBytes = [0]; // empty null-terminated string
            currentOffset += 1;
        }

        int linkInfoSize = currentOffset;

        // Write the complete LinkInfo structure
        writer.Write(linkInfoSize);
        writer.Write(headerSize);
        writer.Write(flags);
        writer.Write(volumeIdOffset);
        writer.Write(localBasePathOffset);
        writer.Write(cnrlOffset);
        writer.Write(commonPathSuffixOffset);

        if (hasLocal)
        {
            writer.Write(volumeIdBytes!);
            writer.Write(localBasePathBytes!);
        }

        if (hasNetwork)
        {
            writer.Write(cnrlBytes!);
        }

        writer.Write(commonPathSuffixBytes!);
    }

    /// <summary>
    /// Writes a KnownFolderDataBlock (signature 0xA000000B, size 28 bytes).
    /// </summary>
    internal static void WriteKnownFolderDataBlock(BinaryWriter writer, KnownFolderData data)
    {
        writer.Write(28); // BlockSize
        writer.Write(LnkConstants.KnownFolderBlockSignature);
        writer.Write(data.FolderId.ToByteArray()); // 16 bytes
        writer.Write(data.Offset);
    }

    /// <summary>
    /// Writes a TrackerDataBlock (signature 0xA0000003, size 96 bytes).
    /// </summary>
    internal static void WriteTrackerDataBlock(BinaryWriter writer, TrackerData data)
    {
        writer.Write(96); // BlockSize
        writer.Write(LnkConstants.TrackerBlockSignature);
        writer.Write(88); // Length
        writer.Write(0);  // Version

        // MachineID: 16 bytes, null-padded
        byte[] machineBytes = new byte[16];
        byte[] nameBytes = Encoding.ASCII.GetBytes(data.MachineId);
        int copyLen = Math.Min(nameBytes.Length, 15);
        Array.Copy(nameBytes, machineBytes, copyLen);
        writer.Write(machineBytes);

        // Droid[0], Droid[1], DroidBirth[0], DroidBirth[1]
        writer.Write(data.VolumeId.ToByteArray());
        writer.Write(data.ObjectId.ToByteArray());
        writer.Write((data.BirthVolumeId ?? data.VolumeId).ToByteArray());
        writer.Write((data.BirthObjectId ?? data.ObjectId).ToByteArray());
    }

    /// <summary>
    /// Writes a PropertyStoreDataBlock (signature 0xA0000009, variable size).
    /// </summary>
    internal static void WritePropertyStoreDataBlock(BinaryWriter writer, byte[] data)
    {
        int blockSize = 8 + data.Length;
        writer.Write(blockSize);
        writer.Write(LnkConstants.PropertyStoreBlockSignature);
        writer.Write(data);
    }

    /// <summary>
    /// Writes a SpecialFolderDataBlock (signature 0xA0000005, size 16 bytes).
    /// </summary>
    internal static void WriteSpecialFolderDataBlock(BinaryWriter writer, SpecialFolderData data)
    {
        writer.Write(16); // BlockSize
        writer.Write(LnkConstants.SpecialFolderBlockSignature);
        writer.Write(data.FolderId);
        writer.Write(data.Offset);
    }
}
