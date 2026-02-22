using System.Text;

namespace LNKLib;

internal static class IdListWriter
{
    private static readonly byte[] NullTerminator = [0x00];

    internal static void Write(BinaryWriter writer, TargetPathInfo pathInfo)
    {
        // Prepare Shell Item ID data
        byte[] rootShellItem = new byte[18];
        rootShellItem[0] = 0x1F;
        rootShellItem[1] = pathInfo.IsNetworkLink ? (byte)0x58 : (byte)0x50;
        byte[] clsidToUse = pathInfo.IsNetworkLink
            ? LnkConstants.NetworkClsid.ToByteArray()
            : LnkConstants.ComputerClsid.ToByteArray();
        Array.Copy(clsidToUse, 0, rootShellItem, 2, 16);

        // Prepare padded target root (targetRoot + 21 null characters)
        string paddedTargetRoot = pathInfo.TargetRoot + new string('\0', 21);

        if (pathInfo.IsRootLink)
        {
            int rootShellItemSize = rootShellItem.Length;
            int rootItemSize = pathInfo.RootPrefix.Length + Encoding.Default.GetByteCount(paddedTargetRoot) + NullTerminator.Length;
            int idListSize = rootShellItemSize + 2 + rootItemSize + 2;
            int totalIdListSize = idListSize + 2;
            writer.WriteUInt16Le(totalIdListSize);

            writer.WriteUInt16Le(rootShellItemSize + 2);
            writer.Write(rootShellItem);

            writer.WriteUInt16Le(rootItemSize + 2);
            writer.Write(pathInfo.RootPrefix);
            writer.Write(Encoding.Default.GetBytes(paddedTargetRoot));
            writer.Write(NullTerminator);
        }
        else
        {
            int rootShellItemSize = rootShellItem.Length;
            int rootItemSize = pathInfo.RootPrefix.Length + Encoding.Default.GetByteCount(paddedTargetRoot) + NullTerminator.Length;
            int targetItemSize = pathInfo.TargetPrefix.Length + (pathInfo.TargetLeaf != null ? Encoding.Default.GetByteCount(pathInfo.TargetLeaf) : 0) + NullTerminator.Length;
            int idListSize = rootShellItemSize + 2 + rootItemSize + 2 + targetItemSize + 2;
            int totalIdListSize = idListSize + 2;
            writer.WriteUInt16Le(totalIdListSize);

            writer.WriteUInt16Le(rootShellItemSize + 2);
            writer.Write(rootShellItem);

            writer.WriteUInt16Le(rootItemSize + 2);
            writer.Write(pathInfo.RootPrefix);
            writer.Write(Encoding.Default.GetBytes(paddedTargetRoot));
            writer.Write(NullTerminator);

            writer.WriteUInt16Le(targetItemSize + 2);
            writer.Write(pathInfo.TargetPrefix);
            if (pathInfo.TargetLeaf != null)
                writer.Write(Encoding.Default.GetBytes(pathInfo.TargetLeaf));
            writer.Write(NullTerminator);
        }
    }
}
