using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class PropertyStoreReaderTests
{
    [Fact]
    public void Parse_AppUserModelId_ReturnsStringEntry()
    {
        var builder = new PropertyStoreBuilder { AppUserModelId = "MyApp.Test" };
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.PropertyId == 5); // PID_AppUserModelId
        Assert.NotNull(entry);
        Assert.Equal(31, entry.VtType); // VT_LPWSTR
        Assert.Equal("MyApp.Test", entry.Value);
    }

    [Fact]
    public void Parse_PreventPinning_ReturnsBoolEntry()
    {
        var builder = new PropertyStoreBuilder { PreventPinning = true };
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.PropertyId == 9); // PID_PreventPinning
        Assert.NotNull(entry);
        Assert.Equal(11, entry.VtType); // VT_BOOL
        Assert.Equal(true, entry.Value);
    }

    [Fact]
    public void Parse_TargetSFGAOFlags_ReturnsUInt32Entry()
    {
        var builder = new PropertyStoreBuilder { TargetSFGAOFlags = 0x40000020u };
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.PropertyId == 8 &&
            e.FormatId == new Guid("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25"));
        Assert.NotNull(entry);
        Assert.Equal(19, entry.VtType); // VT_UI4
        Assert.Equal(0x40000020u, entry.Value);
    }

    [Fact]
    public void Parse_ToastActivatorCLSID_ReturnsGuidEntry()
    {
        var clsid = Guid.Parse("12345678-1234-1234-1234-123456789ABC");
        var builder = new PropertyStoreBuilder { ToastActivatorCLSID = clsid };
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.PropertyId == 26); // PID_ToastActivatorCLSID
        Assert.NotNull(entry);
        Assert.Equal(72, entry.VtType); // VT_CLSID
        Assert.Equal(clsid, entry.Value);
    }

    [Fact]
    public void Parse_NamedStringProperty_ReturnsNamedEntry()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedStringProperty("CustomKey", "CustomValue");
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.Name == "CustomKey");
        Assert.NotNull(entry);
        Assert.Null(entry.PropertyId);
        Assert.Equal("CustomValue", entry.Value);
    }

    [Fact]
    public void Parse_NamedBoolProperty_ReturnsNamedEntry()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedBoolProperty("Enabled", true);
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.Name == "Enabled");
        Assert.NotNull(entry);
        Assert.Equal(true, entry.Value);
    }

    [Fact]
    public void Parse_MixedStorages_AllParsed()
    {
        var builder = new PropertyStoreBuilder
        {
            AppUserModelId = "Test",
            TargetParsingPath = @"C:\test.exe",
            ItemTypeText = "Application"
        };
        builder.AddNamedStringProperty("Tag", "Value");
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        Assert.True(entries.Count >= 4);
        Assert.Contains(entries, e => e.Value is string s && s == "Test");
        Assert.Contains(entries, e => e.Value is string s && s == @"C:\test.exe");
        Assert.Contains(entries, e => e.Value is string s && s == "Application");
        Assert.Contains(entries, e => e.Name == "Tag");
    }

    [Fact]
    public void Parse_Empty_ReturnsEmptyList()
    {
        var builder = new PropertyStoreBuilder();
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);
        Assert.Empty(entries);
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PropertyStoreReader.Parse(null!));
    }

    [Fact]
    public void Parse_FileTime_ReturnsDateTime()
    {
        var dt = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var builder = new PropertyStoreBuilder { DateVisited = dt };
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.VtType == 64); // VT_FILETIME
        Assert.NotNull(entry);
        Assert.Equal(dt, entry.Value);
    }

    [Fact]
    public void Parse_Int32_ReturnsInt()
    {
        var builder = new PropertyStoreBuilder { LinkStatus = -42 };
        byte[] data = builder.Build();
        var entries = PropertyStoreReader.Parse(data);

        var entry = entries.Find(e => e.VtType == 3); // VT_I4
        Assert.NotNull(entry);
        Assert.Equal(-42, entry.Value);
    }
}
