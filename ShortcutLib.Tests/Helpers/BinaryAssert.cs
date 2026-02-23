namespace ShortcutLib.Tests.Helpers;

internal static class BinaryAssert
{
    internal static bool ContainsBytes(byte[] haystack, byte[] needle)
    {
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j]) { match = false; break; }
            }
            if (match) return true;
        }
        return false;
    }

    internal static bool ContainsSignature(byte[] data, uint signature)
    {
        for (int i = 0; i <= data.Length - 4; i++)
        {
            if (BitConverter.ToUInt32(data, i) == signature)
                return true;
        }
        return false;
    }

    internal static int FindSignatureOffset(byte[] data, uint signature)
    {
        for (int i = 4; i <= data.Length - 4; i++)
        {
            if (BitConverter.ToUInt32(data, i) == signature)
                return i;
        }
        return -1;
    }

    internal static int CountOccurrences(byte[] haystack, byte[] needle)
    {
        int count = 0;
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j]) { match = false; break; }
            }
            if (match) count++;
        }
        return count;
    }
}
