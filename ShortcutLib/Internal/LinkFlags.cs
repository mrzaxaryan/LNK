namespace ShortcutLib;

internal static class LinkFlags
{
    internal const int HasLinkTargetIdList = 0x00000001;
    internal const int HasLinkInfo = 0x00000002;
    internal const int HasName = 0x00000004;
    internal const int HasRelativePath = 0x00000008;
    internal const int HasWorkingDir = 0x00000010;
    internal const int HasArguments = 0x00000020;
    internal const int HasIconLocation = 0x00000040;
    internal const int IsUnicode = 0x00000080;
    internal const int HasExpSz = 0x00000200;
    internal const int RunAsUser = 0x00002000;
    internal const int PreferEnvironmentPath = 0x02000000;
}
