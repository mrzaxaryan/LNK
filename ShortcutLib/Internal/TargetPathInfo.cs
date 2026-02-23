namespace ShortcutLib;

internal sealed class TargetPathInfo
{
    internal bool IsNetworkLink { get; init; }
    internal bool IsRootLink { get; set; }
    internal string TargetRoot { get; init; } = "";
    internal string? TargetLeaf { get; init; }
    internal int ExtensionLength { get; init; }

    private static readonly byte[] PrefixLocalRoot = [0x2F];
    private static readonly byte[] PrefixFolder = [0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    private static readonly byte[] PrefixFile = [0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    private static readonly byte[] PrefixNetworkRoot = [0xC3, 0x01, 0x81];
    private static readonly byte[] PrefixNetworkPrinter = [0xC3, 0x02, 0xC1];

    internal static readonly byte[] FileAttrDirectory = [0x10, 0x00, 0x00, 0x00];
    internal static readonly byte[] FileAttrFile = [0x20, 0x00, 0x00, 0x00];

    internal byte[] RootPrefix { get; init; } = PrefixLocalRoot;
    internal byte[] TargetPrefix { get; init; } = PrefixFolder;
    internal byte[] FileAttributes { get; init; } = FileAttrDirectory;

    internal static TargetPathInfo Parse(string target, bool isPrinterLink)
    {
        bool isNetworkLink = target.StartsWith(@"\\");
        bool isRootLink = false;
        int extensionLength = 0;

        byte[] rootPrefix = isNetworkLink
            ? (isPrinterLink ? PrefixNetworkPrinter : PrefixNetworkRoot)
            : PrefixLocalRoot;
        if (isNetworkLink && isPrinterLink)
            isRootLink = true;

        // Split target path into root and leaf parts
        string targetRoot;
        string? targetLeaf = null;
        if (isRootLink)
        {
            targetRoot = target;
        }
        else if (isNetworkLink)
        {
            int lastSlash = target.LastIndexOf('\\');
            if (lastSlash != -1)
            {
                targetLeaf = target.Substring(lastSlash + 1);
                targetRoot = target.Substring(0, lastSlash);
            }
            else
            {
                targetRoot = target;
            }
        }
        else
        {
            int firstSlash = target.IndexOf('\\');
            if (firstSlash != -1)
            {
                targetLeaf = target.Substring(firstSlash + 1);
                targetRoot = target.Substring(0, firstSlash);
            }
            else
            {
                targetRoot = target;
            }
            // Append trailing backslash for local paths.
            targetRoot += "\\";
        }
        if (targetLeaf is null or { Length: 0 })
            isRootLink = true;

        // Determine target prefix based on file extension
        if (!string.IsNullOrEmpty(targetLeaf))
        {
            int dotIndex = targetLeaf.LastIndexOf('.');
            if (dotIndex != -1 && dotIndex + 1 < targetLeaf.Length)
                extensionLength = targetLeaf.Substring(dotIndex + 1).Length;
        }

        byte[] targetPrefix;
        byte[] fileAttributes;
        if (extensionLength >= 1 && extensionLength <= 3)
        {
            targetPrefix = PrefixFile;
            fileAttributes = FileAttrFile;
        }
        else
        {
            targetPrefix = PrefixFolder;
            fileAttributes = FileAttrDirectory;
        }

        return new TargetPathInfo
        {
            IsNetworkLink = isNetworkLink,
            IsRootLink = isRootLink,
            TargetRoot = targetRoot,
            TargetLeaf = targetLeaf,
            ExtensionLength = extensionLength,
            RootPrefix = rootPrefix,
            TargetPrefix = targetPrefix,
            FileAttributes = fileAttributes
        };
    }
}
