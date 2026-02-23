using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class WinXHasherTests
{
    [Fact]
    public void ComputeHash_Consistent()
    {
        uint hash1 = WinXHasher.ComputeHash(@"C:\Windows\System32\cmd.exe");
        uint hash2 = WinXHasher.ComputeHash(@"C:\Windows\System32\cmd.exe");
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_CaseInsensitive()
    {
        uint lower = WinXHasher.ComputeHash(@"c:\windows\system32\cmd.exe");
        uint upper = WinXHasher.ComputeHash(@"C:\WINDOWS\SYSTEM32\CMD.EXE");
        Assert.Equal(lower, upper);
    }

    [Fact]
    public void ComputeHash_DifferentInputs_DifferentHashes()
    {
        uint hash1 = WinXHasher.ComputeHash(@"C:\Windows\System32\cmd.exe");
        uint hash2 = WinXHasher.ComputeHash(@"C:\Windows\notepad.exe");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_WithArguments()
    {
        uint noArgs = WinXHasher.ComputeHash(@"C:\Windows\System32\cmd.exe");
        uint withArgs = WinXHasher.ComputeHash(@"C:\Windows\System32\cmd.exe", "/k echo test");
        Assert.NotEqual(noArgs, withArgs);
    }

    [Fact]
    public void BuildPropertyStore_ContainsWinXFormatId()
    {
        byte[] data = WinXHasher.BuildPropertyStore(@"C:\Windows\System32\cmd.exe");
        Guid winxGuid = new("FB8D2D7B-90D1-4E34-BF60-6EAC09922BBF");
        byte[] guidBytes = winxGuid.ToByteArray();
        Assert.True(ContainsBytes(data, guidBytes));
    }

    [Fact]
    public void BuildPropertyStore_RoundTripsViaLnk()
    {
        byte[] propStore = WinXHasher.BuildPropertyStore(@"C:\Windows\System32\cmd.exe");
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Windows\System32\cmd.exe",
            PropertyStoreData = propStore
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.PropertyStoreData);
    }

    [Fact]
    public void WinXHash_ViaPropertyStoreBuilder()
    {
        uint hash = WinXHasher.ComputeHash(@"C:\test.exe");
        var builder = new PropertyStoreBuilder { WinXHash = hash };
        byte[] result = builder.Build();
        Guid winxGuid = new("FB8D2D7B-90D1-4E34-BF60-6EAC09922BBF");
        Assert.True(ContainsBytes(result, winxGuid.ToByteArray()));
    }

    [Fact]
    public void ComputeHash_NullTarget_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => WinXHasher.ComputeHash(null!));
    }

    private static bool ContainsBytes(byte[] data, byte[] pattern)
    {
        for (int i = 0; i <= data.Length - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (data[i + j] != pattern[j]) { match = false; break; }
            }
            if (match) return true;
        }
        return false;
    }
}
