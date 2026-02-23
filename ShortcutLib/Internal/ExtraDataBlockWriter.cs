using System.Text;

namespace ShortcutLib;

internal static class ExtraDataBlockWriter
{
    /// <summary>
    /// Writes the LinkInfo structure per [MS-SHLLINK] 2.3.
    /// When useUnicode is true, writes the extended header (0x24) with Unicode path fields.
    /// </summary>
    internal static void WriteLinkInfo(BinaryWriter writer, LinkInfo info, bool useUnicode = false)
    {
        bool hasLocal = info.Local is not null;
        bool hasNetwork = info.Network is not null;

        int flags = 0;
        if (hasLocal) flags |= 0x01;
        if (hasNetwork) flags |= 0x02;

        int headerSize = useUnicode ? 0x24 : 0x1C;

        // Pre-compute VolumeID
        byte[]? volumeIdBytes = null;
        byte[]? localBasePathAnsi = null;
        byte[]? localBasePathUnicode = null;
        if (hasLocal)
        {
            var local = info.Local!;
            byte[] volumeLabelAnsi = Encoding.Default.GetBytes(local.VolumeLabel + "\0");

            using var volMs = new MemoryStream();
            using var volW = new BinaryWriter(volMs);

            if (useUnicode)
            {
                // Extended VolumeID: VolumeLabelOffset=0x14 (sentinel), then VolumeLabelOffsetUnicode
                byte[] volumeLabelUnicode = Encoding.Unicode.GetBytes(local.VolumeLabel + "\0");
                int volumeIdSize = 4 + 4 + 4 + 4 + 4 + volumeLabelAnsi.Length + volumeLabelUnicode.Length;
                int volumeLabelUnicodeOffset = 4 + 4 + 4 + 4 + 4 + volumeLabelAnsi.Length;
                volW.Write(volumeIdSize);
                volW.Write(local.DriveType);
                volW.Write(local.DriveSerialNumber);
                volW.Write(0x14);  // Sentinel: signals Unicode label follows
                volW.Write(volumeLabelUnicodeOffset);
                volW.Write(volumeLabelAnsi);
                volW.Write(volumeLabelUnicode);
            }
            else
            {
                int volumeIdSize = 4 + 4 + 4 + 4 + volumeLabelAnsi.Length;
                volW.Write(volumeIdSize);
                volW.Write(local.DriveType);
                volW.Write(local.DriveSerialNumber);
                volW.Write(0x10); // VolumeLabelOffset = 16
                volW.Write(volumeLabelAnsi);
            }

            volW.Flush();
            volumeIdBytes = volMs.ToArray();

            localBasePathAnsi = Encoding.Default.GetBytes(local.BasePath + "\0");
            if (useUnicode)
                localBasePathUnicode = Encoding.Unicode.GetBytes(local.BasePath + "\0");
        }

        // Pre-compute CNRL
        byte[]? cnrlBytes = null;
        byte[]? commonPathSuffixAnsi = null;
        byte[]? commonPathSuffixUnicode = null;
        if (hasNetwork)
        {
            var network = info.Network!;
            byte[] shareNameAnsi = Encoding.Default.GetBytes(network.ShareName + "\0");
            byte[] deviceNameAnsi = network.DeviceName != null
                ? Encoding.Default.GetBytes(network.DeviceName + "\0")
                : [];

            int cnrlFlags = 0;
            if (network.DeviceName != null) cnrlFlags |= 0x01;
            if (network.NetworkProviderType.HasValue) cnrlFlags |= 0x02;

            using var cnrlMs = new MemoryStream();
            using var cnrlW = new BinaryWriter(cnrlMs);

            if (useUnicode)
            {
                // Extended CNRL: header size 0x1C (7 DWORDs) with Unicode offsets
                byte[] shareNameUnicode = Encoding.Unicode.GetBytes(network.ShareName + "\0");
                byte[] deviceNameUnicode = network.DeviceName != null
                    ? Encoding.Unicode.GetBytes(network.DeviceName + "\0")
                    : [];

                const int cnrlHeaderSize = 0x1C; // 7 * 4 = 28 bytes (extended)
                int deviceNameOffset = network.DeviceName != null
                    ? cnrlHeaderSize + shareNameAnsi.Length
                    : 0;
                int ansiEnd = cnrlHeaderSize + shareNameAnsi.Length + deviceNameAnsi.Length;
                int netNameOffsetUnicode = ansiEnd;
                int deviceNameOffsetUnicode = network.DeviceName != null
                    ? ansiEnd + shareNameUnicode.Length
                    : 0;
                int cnrlSize = ansiEnd + shareNameUnicode.Length + deviceNameUnicode.Length;

                cnrlW.Write(cnrlSize);
                cnrlW.Write(cnrlFlags);
                cnrlW.Write(cnrlHeaderSize); // NetNameOffset > 0x14 signals extended
                cnrlW.Write(deviceNameOffset);
                cnrlW.Write(network.NetworkProviderType ?? 0x00020000u);
                cnrlW.Write(netNameOffsetUnicode);
                cnrlW.Write(deviceNameOffsetUnicode);
                cnrlW.Write(shareNameAnsi);
                if (deviceNameAnsi.Length > 0) cnrlW.Write(deviceNameAnsi);
                cnrlW.Write(shareNameUnicode);
                if (deviceNameUnicode.Length > 0) cnrlW.Write(deviceNameUnicode);
            }
            else
            {
                const int cnrlHeaderSize = 0x14;
                int deviceNameOffset = network.DeviceName != null
                    ? cnrlHeaderSize + shareNameAnsi.Length
                    : 0;
                int cnrlSize = cnrlHeaderSize + shareNameAnsi.Length + deviceNameAnsi.Length;

                cnrlW.Write(cnrlSize);
                cnrlW.Write(cnrlFlags);
                cnrlW.Write(cnrlHeaderSize);
                cnrlW.Write(deviceNameOffset);
                cnrlW.Write(network.NetworkProviderType ?? 0x00020000u);
                cnrlW.Write(shareNameAnsi);
                if (deviceNameAnsi.Length > 0) cnrlW.Write(deviceNameAnsi);
            }

            cnrlW.Flush();
            cnrlBytes = cnrlMs.ToArray();

            commonPathSuffixAnsi = Encoding.Default.GetBytes(network.CommonPathSuffix + "\0");
            if (useUnicode)
                commonPathSuffixUnicode = Encoding.Unicode.GetBytes(network.CommonPathSuffix + "\0");
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
            currentOffset += localBasePathAnsi!.Length;
        }

        if (hasNetwork)
        {
            cnrlOffset = currentOffset;
            currentOffset += cnrlBytes!.Length;
            commonPathSuffixOffset = currentOffset;
            currentOffset += commonPathSuffixAnsi!.Length;
        }
        else
        {
            commonPathSuffixOffset = currentOffset;
            commonPathSuffixAnsi = [0];
            currentOffset += 1;
        }

        // Unicode offsets (after all ANSI data)
        int localBasePathOffsetUnicode = 0;
        int commonPathSuffixOffsetUnicode = 0;

        if (useUnicode)
        {
            if (hasLocal)
            {
                localBasePathOffsetUnicode = currentOffset;
                currentOffset += localBasePathUnicode!.Length;
            }
            commonPathSuffixOffsetUnicode = currentOffset;
            if (commonPathSuffixUnicode != null)
                currentOffset += commonPathSuffixUnicode.Length;
            else
            {
                commonPathSuffixUnicode = Encoding.Unicode.GetBytes("\0");
                currentOffset += commonPathSuffixUnicode.Length;
            }
        }

        int linkInfoSize = currentOffset;

        // Write header
        writer.Write(linkInfoSize);
        writer.Write(headerSize);
        writer.Write(flags);
        writer.Write(volumeIdOffset);
        writer.Write(localBasePathOffset);
        writer.Write(cnrlOffset);
        writer.Write(commonPathSuffixOffset);

        if (useUnicode)
        {
            writer.Write(localBasePathOffsetUnicode);
            writer.Write(commonPathSuffixOffsetUnicode);
        }

        // Write data
        if (hasLocal)
        {
            writer.Write(volumeIdBytes!);
            writer.Write(localBasePathAnsi!);
        }

        if (hasNetwork)
            writer.Write(cnrlBytes!);

        writer.Write(commonPathSuffixAnsi!);

        // Write Unicode data
        if (useUnicode)
        {
            if (hasLocal)
                writer.Write(localBasePathUnicode!);
            writer.Write(commonPathSuffixUnicode!);
        }
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

    /// <summary>
    /// Writes a ConsoleDataBlock (signature 0xA0000002, size 204 bytes).
    /// </summary>
    internal static void WriteConsoleDataBlock(BinaryWriter writer, ConsoleData data)
    {
        writer.Write(204); // BlockSize: always 0xCC
        writer.Write(LnkConstants.ConsoleBlockSignature);

        writer.Write(data.FillAttributes);
        writer.Write(data.PopupFillAttributes);
        writer.Write(data.ScreenBufferSizeX);
        writer.Write(data.ScreenBufferSizeY);
        writer.Write(data.WindowSizeX);
        writer.Write(data.WindowSizeY);
        writer.Write(data.WindowOriginX);
        writer.Write(data.WindowOriginY);
        writer.Write(0); // Unused1
        writer.Write(0); // Unused2
        writer.Write(data.FontSize);
        writer.Write(data.FontFamily);
        writer.Write(data.FontWeight);

        // FaceName: 32 WCHARs = 64 bytes, null-padded
        byte[] faceNameBytes = new byte[64];
        byte[] encoded = Encoding.Unicode.GetBytes(data.FaceName);
        int copyLen = Math.Min(encoded.Length, 62); // leave room for null terminator
        Array.Copy(encoded, faceNameBytes, copyLen);
        writer.Write(faceNameBytes);

        writer.Write(data.CursorSize);
        writer.Write(data.FullScreen ? 1u : 0u);
        writer.Write(data.QuickEdit ? 1u : 0u);
        writer.Write(data.InsertMode ? 1u : 0u);
        writer.Write(data.AutoPosition ? 1u : 0u);
        writer.Write(data.HistoryBufferSize);
        writer.Write(data.NumberOfHistoryBuffers);
        writer.Write(data.HistoryNoDup ? 1u : 0u);

        // ColorTable: 16 x uint32
        for (int i = 0; i < 16; i++)
            writer.Write(i < data.ColorTable.Length ? data.ColorTable[i] : 0u);
    }

    /// <summary>
    /// Writes a ConsoleFEDataBlock (signature 0xA0000004, size 12 bytes).
    /// </summary>
    internal static void WriteConsoleFEDataBlock(BinaryWriter writer, uint codePage)
    {
        writer.Write(12); // BlockSize
        writer.Write(LnkConstants.ConsoleFEBlockSignature);
        writer.Write(codePage);
    }

    /// <summary>
    /// Writes a ShimDataBlock (signature 0xA0000008, variable size).
    /// </summary>
    internal static void WriteShimDataBlock(BinaryWriter writer, string layerName)
    {
        byte[] unicodeBytes = Encoding.Unicode.GetBytes(layerName + "\0");
        int blockSize = 8 + unicodeBytes.Length;
        writer.Write(blockSize);
        writer.Write(LnkConstants.ShimBlockSignature);
        writer.Write(unicodeBytes);
    }

    /// <summary>
    /// Writes a VistaAndAboveIDListDataBlock (signature 0xA000000C, variable size).
    /// </summary>
    internal static void WriteVistaIdListDataBlock(BinaryWriter writer, byte[] idListData)
    {
        int blockSize = 8 + idListData.Length;
        writer.Write(blockSize);
        writer.Write(LnkConstants.VistaIdListBlockSignature);
        writer.Write(idListData);
    }

    /// <summary>
    /// Writes an environment-variable-style data block (used for EnvironmentVariable,
    /// IconEnvironment, and Darwin data blocks — they share the same layout, differing only in signature).
    /// </summary>
    internal static void WriteEnvironmentDataBlock(BinaryWriter writer, string target, uint signature)
    {
        int blockSize = 4 + 4 + LnkConstants.MaxPath + (LnkConstants.MaxPath * 2); // 788 bytes
        writer.Write(blockSize);
        writer.Write(signature);

        // Prepare ANSI buffer (260 bytes): zero-filled, copy target, ensure null termination.
        byte[] ansiBuffer = new byte[LnkConstants.MaxPath];
        byte[] targetAnsi = Encoding.Default.GetBytes(target);
        int copyLen = Math.Min(targetAnsi.Length, LnkConstants.MaxPath - 1);
        Array.Copy(targetAnsi, 0, ansiBuffer, 0, copyLen);
        ansiBuffer[copyLen] = 0;
        writer.Write(ansiBuffer);

        // Prepare Unicode buffer (520 bytes = 260 WCHARs): zero-filled and copy target.
        char[] unicodeBuffer = new char[LnkConstants.MaxPath];
        for (int i = 0; i < LnkConstants.MaxPath; i++)
            unicodeBuffer[i] = '\0';
        copyLen = Math.Min(target.Length, LnkConstants.MaxPath - 1);
        target.CopyTo(0, unicodeBuffer, 0, copyLen);
        byte[] unicodeBytes = Encoding.Unicode.GetBytes(unicodeBuffer);
        if (unicodeBytes.Length < LnkConstants.MaxPath * 2)
        {
            byte[] temp = new byte[LnkConstants.MaxPath * 2];
            Array.Copy(unicodeBytes, temp, unicodeBytes.Length);
            writer.Write(temp);
        }
        else
        {
            writer.Write(unicodeBytes, 0, LnkConstants.MaxPath * 2);
        }
    }
}
