using System.Text;

namespace ShortcutLib;

/// <summary>
/// Parses raw PropertyStoreData bytes into structured <see cref="PropertyStoreEntry"/> objects.
/// Useful for forensic analysis, display, and inspection of shortcut metadata.
/// </summary>
public static class PropertyStoreReader
{
    private static readonly Guid NamedPropertyFormatId =
        new("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

    /// <summary>
    /// Parses raw property store bytes and returns a list of all entries.
    /// </summary>
    public static List<PropertyStoreEntry> Parse(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var entries = new List<PropertyStoreEntry>();

        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);

        while (reader.BaseStream.Position + 4 <= reader.BaseStream.Length)
        {
            uint storageSize = reader.ReadUInt32();
            if (storageSize < 4) break; // Terminal (0-size storage)

            long storageStart = reader.BaseStream.Position - 4;
            long storageEnd = storageStart + storageSize;

            if (storageEnd > reader.BaseStream.Length)
                break;

            // Read "1SPS" magic
            if (reader.BaseStream.Position + 4 > storageEnd) break;
            uint magic = reader.ReadUInt32();
            if (magic != 0x53505331u) break; // "1SPS"

            // Read format ID
            if (reader.BaseStream.Position + 16 > storageEnd) break;
            Guid formatId = new(reader.ReadBytes(16));
            bool isNamed = formatId == NamedPropertyFormatId;

            // Read entries within this storage
            while (reader.BaseStream.Position + 4 <= storageEnd)
            {
                uint entrySize = reader.ReadUInt32();
                if (entrySize == 0) break; // Terminal entry

                long entryStart = reader.BaseStream.Position - 4;
                long entryEnd = entryStart + entrySize;

                if (entryEnd > storageEnd || reader.BaseStream.Position + 5 > entryEnd)
                {
                    reader.BaseStream.Position = entryEnd;
                    continue;
                }

                var entry = new PropertyStoreEntry { FormatId = formatId };

                if (isNamed)
                {
                    uint nameByteLength = reader.ReadUInt32();
                    reader.ReadByte(); // reserved
                    if (nameByteLength > 0 && reader.BaseStream.Position + nameByteLength <= entryEnd)
                    {
                        byte[] nameBytes = reader.ReadBytes((int)nameByteLength);
                        entry.Name = Encoding.Unicode.GetString(nameBytes).TrimEnd('\0');
                    }
                }
                else
                {
                    entry.PropertyId = reader.ReadUInt32();
                    reader.ReadByte(); // reserved
                }

                // Read typed property value
                if (reader.BaseStream.Position + 4 <= entryEnd)
                {
                    ushort vtType = reader.ReadUInt16();
                    reader.ReadUInt16(); // padding
                    entry.VtType = vtType;
                    entry.Value = ReadTypedValue(reader, vtType, entryEnd);
                }

                entries.Add(entry);
                reader.BaseStream.Position = entryEnd;
            }

            reader.BaseStream.Position = storageEnd;
        }

        return entries;
    }

    private static object? ReadTypedValue(BinaryReader reader, ushort vtType, long entryEnd)
    {
        long remaining = entryEnd - reader.BaseStream.Position;

        switch (vtType)
        {
            case 31: // VT_LPWSTR
                if (remaining < 4) return null;
                int strByteLen = reader.ReadInt32();
                if (strByteLen <= 0 || reader.BaseStream.Position + strByteLen > entryEnd)
                    return null;
                byte[] strBytes = reader.ReadBytes(strByteLen);
                return Encoding.Unicode.GetString(strBytes).TrimEnd('\0');

            case 11: // VT_BOOL
                if (remaining < 2) return null;
                return reader.ReadInt16() != 0;

            case 19: // VT_UI4
                if (remaining < 4) return null;
                return reader.ReadUInt32();

            case 3: // VT_I4
                if (remaining < 4) return null;
                return reader.ReadInt32();

            case 72: // VT_CLSID
                if (remaining < 16) return null;
                return new Guid(reader.ReadBytes(16));

            case 64: // VT_FILETIME
                if (remaining < 8) return null;
                long ft = reader.ReadInt64();
                return ft > 0 ? DateTime.FromFileTimeUtc(ft) : null;

            case 21: // VT_UI8
                if (remaining < 8) return null;
                return reader.ReadUInt64();

            case 20: // VT_I8
                if (remaining < 8) return null;
                return reader.ReadInt64();

            case 2: // VT_I2
                if (remaining < 2) return null;
                return reader.ReadInt16();

            case 18: // VT_UI2
                if (remaining < 2) return null;
                return reader.ReadUInt16();

            case 65: // VT_BLOB
                if (remaining < 4) return null;
                int blobLen = reader.ReadInt32();
                if (blobLen <= 0 || reader.BaseStream.Position + blobLen > entryEnd)
                    return null;
                return reader.ReadBytes(blobLen);

            case 30: // VT_LPSTR
                if (remaining < 4) return null;
                int astrLen = reader.ReadInt32();
                if (astrLen <= 0 || reader.BaseStream.Position + astrLen > entryEnd)
                    return null;
                byte[] astrBytes = reader.ReadBytes(astrLen);
                return Encoding.Default.GetString(astrBytes).TrimEnd('\0');

            default:
                // Return remaining bytes as raw data
                int rem = (int)(entryEnd - reader.BaseStream.Position);
                return rem > 0 ? reader.ReadBytes(rem) : null;
        }
    }
}
