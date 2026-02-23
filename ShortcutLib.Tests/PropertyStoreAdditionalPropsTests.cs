using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class PropertyStoreAdditionalPropsTests
{
    [Fact]
    public void IsDualMode_SerializesAsBool()
    {
        var builder = new PropertyStoreBuilder { IsDualMode = true };
        byte[] result = builder.Build();
        Assert.True(ContainsUInt32(result, 11)); // PID 11
    }

    [Fact]
    public void StartPinOption_SerializesAsUInt32()
    {
        var builder = new PropertyStoreBuilder { StartPinOption = 2 }; // UserPinned
        byte[] result = builder.Build();
        Assert.True(ContainsUInt32(result, 12)); // PID 12
    }

    [Fact]
    public void PackageFamilyName_ContainsString()
    {
        var builder = new PropertyStoreBuilder { PackageFamilyName = "Microsoft.App_8wekyb3d8bbwe" };
        byte[] result = builder.Build();
        Assert.True(ContainsUnicodeString(result, "Microsoft.App_8wekyb3d8bbwe"));
    }

    [Fact]
    public void DestListProvidedTitle_ContainsString()
    {
        var builder = new PropertyStoreBuilder { DestListProvidedTitle = "My Title" };
        byte[] result = builder.Build();
        Assert.True(ContainsUnicodeString(result, "My Title"));
    }

    [Fact]
    public void RunFlags_SerializesAsUInt32()
    {
        var builder = new PropertyStoreBuilder { RunFlags = 0x01 };
        byte[] result = builder.Build();
        Assert.True(ContainsUInt32(result, 27)); // PID 27
    }

    [Fact]
    public void ExcludedFromLauncher_SerializesAsBool()
    {
        var builder = new PropertyStoreBuilder { ExcludedFromLauncher = true };
        byte[] result = builder.Build();
        Assert.True(ContainsUInt32(result, 30)); // PID 30
    }

    [Fact]
    public void LinkComment_InLinkStorage()
    {
        var builder = new PropertyStoreBuilder { LinkComment = "Test comment" };
        byte[] result = builder.Build();
        Guid linkGuid = new("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25");
        Assert.True(ContainsBytes(result, linkGuid.ToByteArray()));
        Assert.True(ContainsUnicodeString(result, "Test comment"));
    }

    [Fact]
    public void DateVisited_SerializesAsFileTime()
    {
        var dt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var builder = new PropertyStoreBuilder { DateVisited = dt };
        byte[] result = builder.Build();
        // VT_FILETIME = 64 (0x40)
        Assert.True(ContainsUInt16(result, 64));
    }

    [Fact]
    public void LinkStatus_SerializesAsInt32()
    {
        var builder = new PropertyStoreBuilder { LinkStatus = -1 };
        byte[] result = builder.Build();
        // VT_I4 = 3
        Assert.True(ContainsUInt16(result, 3));
    }

    [Fact]
    public void TargetUrl_HasCorrectFormatId()
    {
        var builder = new PropertyStoreBuilder { TargetUrl = "https://example.com" };
        byte[] result = builder.Build();
        Guid urlGuid = new("5CBF2787-48CF-4208-B90E-EE5E5D420294");
        Assert.True(ContainsBytes(result, urlGuid.ToByteArray()));
    }

    [Fact]
    public void TargetExtension_HasCorrectFormatId()
    {
        var builder = new PropertyStoreBuilder { TargetExtension = ".exe" };
        byte[] result = builder.Build();
        Guid extGuid = new("7A7D76F4-B630-4BD7-95FF-37CC51A975C9");
        Assert.True(ContainsBytes(result, extGuid.ToByteArray()));
    }

    [Fact]
    public void AllNewProps_RoundTripViaLnk()
    {
        var builder = new PropertyStoreBuilder
        {
            IsDualMode = true,
            PackageFamilyName = "TestPackage",
            DestListProvidedTitle = "Title",
            TargetUrl = "https://test.com"
        };
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            PropertyStoreData = builder.Build()
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.PropertyStoreData);

        var entries = PropertyStoreReader.Parse(options.PropertyStoreData);
        Assert.True(entries.Count > 0);
    }

    private static bool ContainsUInt16(byte[] data, ushort value)
    {
        byte[] target = BitConverter.GetBytes(value);
        for (int i = 0; i <= data.Length - 2; i++)
            if (data[i] == target[0] && data[i + 1] == target[1])
                return true;
        return false;
    }

    private static bool ContainsUInt32(byte[] data, uint value)
    {
        byte[] target = BitConverter.GetBytes(value);
        for (int i = 0; i <= data.Length - 4; i++)
            if (data[i] == target[0] && data[i + 1] == target[1] &&
                data[i + 2] == target[2] && data[i + 3] == target[3])
                return true;
        return false;
    }

    private static bool ContainsUnicodeString(byte[] data, string value)
    {
        byte[] strBytes = System.Text.Encoding.Unicode.GetBytes(value);
        return ContainsBytes(data, strBytes);
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
