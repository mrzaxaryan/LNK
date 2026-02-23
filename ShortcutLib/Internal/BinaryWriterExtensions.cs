using System.Text;

namespace ShortcutLib;

internal static class BinaryWriterExtensions
{
    /// <summary>
    /// Writes a 16-bit unsigned integer in little-endian byte order.
    /// </summary>
    internal static void WriteUInt16Le(this BinaryWriter writer, int value)
    {
        writer.Write((byte)(value % 256));
        writer.Write((byte)(value / 256));
    }

    /// <summary>
    /// Writes an optional string to the BinaryWriter (2-byte length then string bytes).
    /// When unicode is false, uses ANSI encoding per the .lnk spec (IS_UNICODE not set).
    /// When unicode is true, uses UTF-16LE encoding.
    /// </summary>
    internal static void WriteStringData(this BinaryWriter writer, string? value, bool unicode)
    {
        if (value is null)
            return;
        writer.WriteUInt16Le(value.Length);
        if (unicode)
            writer.Write(Encoding.Unicode.GetBytes(value));
        else
            writer.Write(Encoding.Default.GetBytes(value));
    }

    /// <summary>
    /// Writes an environment-variable-style data block (used for both EnvironmentVariableDataBlock
    /// and IconEnvironmentDataBlock — they share the same layout, differing only in signature).
    /// </summary>
    internal static void WriteEnvironmentDataBlock(this BinaryWriter writer, string target, uint signature)
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
        // Buffer remains null-terminated.
        byte[] unicodeBytes = Encoding.Unicode.GetBytes(unicodeBuffer);
        // Ensure exactly 520 bytes are written.
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
