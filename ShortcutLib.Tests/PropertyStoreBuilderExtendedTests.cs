using System.Text;
using Xunit;

namespace ShortcutLib.Tests;

public class PropertyStoreBuilderExtendedTests
{
    // --- New typed property tests ---

    [Fact]
    public void Build_TargetParsingPath_ContainsString()
    {
        var builder = new PropertyStoreBuilder { TargetParsingPath = @"C:\Windows\notepad.exe" };
        byte[] result = builder.Build();
        byte[] expected = Encoding.Unicode.GetBytes(@"C:\Windows\notepad.exe");
        Assert.True(ContainsBytes(result, expected));
    }

    [Fact]
    public void Build_TargetParsingPath_ContainsLinkFormatId()
    {
        var builder = new PropertyStoreBuilder { TargetParsingPath = @"C:\test.exe" };
        byte[] result = builder.Build();
        Guid linkFmtId = new("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25");
        Assert.True(ContainsBytes(result, linkFmtId.ToByteArray()));
    }

    [Fact]
    public void Build_TargetSFGAOFlags_ContainsVtUI4()
    {
        var builder = new PropertyStoreBuilder { TargetSFGAOFlags = 0x40400177 };
        byte[] result = builder.Build();
        // VT_UI4 = 19 (0x0013) should appear
        Assert.True(ContainsUInt16(result, 19));
        // The uint value itself should appear
        Assert.True(ContainsUInt32(result, 0x40400177));
    }

    [Fact]
    public void Build_ItemTypeText_ContainsString()
    {
        var builder = new PropertyStoreBuilder { ItemTypeText = "Text Document" };
        byte[] result = builder.Build();
        byte[] expected = Encoding.Unicode.GetBytes("Text Document");
        Assert.True(ContainsBytes(result, expected));
    }

    [Fact]
    public void Build_ItemTypeText_ContainsSystemFormatId()
    {
        var builder = new PropertyStoreBuilder { ItemTypeText = "Application" };
        byte[] result = builder.Build();
        Guid sysFmtId = new("B725F130-47EF-101A-A5F1-02608C9EEBAC");
        Assert.True(ContainsBytes(result, sysFmtId.ToByteArray()));
    }

    [Fact]
    public void Build_MimeType_ContainsString()
    {
        var builder = new PropertyStoreBuilder { MimeType = "text/plain" };
        byte[] result = builder.Build();
        byte[] expected = Encoding.Unicode.GetBytes("text/plain");
        Assert.True(ContainsBytes(result, expected));
    }

    [Fact]
    public void Build_MultipleStorages_AllPresent()
    {
        var builder = new PropertyStoreBuilder
        {
            AppUserModelId = "Test.App",
            TargetParsingPath = @"C:\test.exe",
            ItemTypeText = "Application"
        };
        byte[] result = builder.Build();

        // All three format IDs should be present
        Assert.True(ContainsBytes(result, new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3").ToByteArray()));
        Assert.True(ContainsBytes(result, new Guid("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25").ToByteArray()));
        Assert.True(ContainsBytes(result, new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC").ToByteArray()));

        // Terminal
        Assert.Equal(0u, BitConverter.ToUInt32(result, result.Length - 4));
    }

    [Fact]
    public void Build_NewProperties_RoundTripThroughPropertyStoreData()
    {
        var builder = new PropertyStoreBuilder
        {
            TargetParsingPath = @"C:\Windows\notepad.exe",
            TargetSFGAOFlags = 0x12345678,
            ItemTypeText = "Application",
            MimeType = "application/x-ms-shortcut"
        };
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            PropertyStoreData = builder.Build()
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.PropertyStoreData);
        Assert.True(options.PropertyStoreData!.Length > 4);
    }

    // --- Named property tests ---

    [Fact]
    public void AddNamedStringProperty_ContainsNameAndValue()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedStringProperty("CustomProp", "CustomValue");
        byte[] result = builder.Build();

        Assert.True(ContainsBytes(result, Encoding.Unicode.GetBytes("CustomProp")));
        Assert.True(ContainsBytes(result, Encoding.Unicode.GetBytes("CustomValue")));
    }

    [Fact]
    public void AddNamedStringProperty_ContainsNamedFormatId()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedStringProperty("Test", "Value");
        byte[] result = builder.Build();

        Guid namedFmtId = new("D5CDD505-2E9C-101B-9397-08002B2CF9AE");
        Assert.True(ContainsBytes(result, namedFmtId.ToByteArray()));
    }

    [Fact]
    public void AddNamedUInt32Property_ContainsVtUI4()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedUInt32Property("Count", 42);
        byte[] result = builder.Build();

        Assert.True(ContainsUInt16(result, 19)); // VT_UI4
        Assert.True(ContainsUInt32(result, 42u));
    }

    [Fact]
    public void AddNamedBoolProperty_ContainsVtBool()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedBoolProperty("Enabled", true);
        byte[] result = builder.Build();

        Assert.True(ContainsUInt16(result, 11)); // VT_BOOL
    }

    [Fact]
    public void FluentChaining_Works()
    {
        var builder = new PropertyStoreBuilder()
            .AddNamedStringProperty("Key1", "Value1")
            .AddNamedUInt32Property("Key2", 100)
            .AddNamedBoolProperty("Key3", false);

        byte[] result = builder.Build();
        Assert.True(ContainsBytes(result, Encoding.Unicode.GetBytes("Key1")));
        Assert.True(ContainsBytes(result, Encoding.Unicode.GetBytes("Key2")));
        Assert.True(ContainsBytes(result, Encoding.Unicode.GetBytes("Key3")));
    }

    [Fact]
    public void Build_MixedTypedAndNamed_AllPresent()
    {
        var builder = new PropertyStoreBuilder
        {
            AppUserModelId = "MyApp"
        };
        builder.AddNamedStringProperty("Custom", "Data");

        byte[] result = builder.Build();

        // AppUserModel format ID
        Assert.True(ContainsBytes(result, new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3").ToByteArray()));
        // Named property format ID
        Assert.True(ContainsBytes(result, new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE").ToByteArray()));
        // Terminal
        Assert.Equal(0u, BitConverter.ToUInt32(result, result.Length - 4));
    }

    [Fact]
    public void Build_NamedProperties_RoundTripThroughPropertyStoreData()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedStringProperty("TestProp", "TestValue");
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            PropertyStoreData = builder.Build()
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.PropertyStoreData);
        Assert.True(options.PropertyStoreData!.Length > 4);
    }

    // --- Helpers ---

    private static bool ContainsUInt32(byte[] data, uint value)
    {
        byte[] needle = BitConverter.GetBytes(value);
        return ContainsBytes(data, needle);
    }

    private static bool ContainsUInt16(byte[] data, ushort value)
    {
        byte[] needle = BitConverter.GetBytes(value);
        return ContainsBytes(data, needle);
    }

    private static bool ContainsBytes(byte[] haystack, byte[] needle)
    {
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return true;
        }
        return false;
    }
}
