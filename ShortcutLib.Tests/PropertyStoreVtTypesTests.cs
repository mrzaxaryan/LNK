using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class PropertyStoreVtTypesTests
{
    [Fact]
    public void Named_Int16_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedInt16Property("ShortVal", 42);
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 2)); // VT_I2 = 2
    }

    [Fact]
    public void Named_Int32_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedInt32Property("IntVal", -100);
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 3)); // VT_I4 = 3
    }

    [Fact]
    public void Named_UInt16_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedUInt16Property("UShortVal", 65535);
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 18)); // VT_UI2 = 18
    }

    [Fact]
    public void Named_Int64_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedInt64Property("LongVal", long.MaxValue);
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 20)); // VT_I8 = 20
    }

    [Fact]
    public void Named_UInt64_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedUInt64Property("ULongVal", ulong.MaxValue);
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 21)); // VT_UI8 = 21
    }

    [Fact]
    public void Named_FileTime_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedFileTimeProperty("DateVal", new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc));
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 64)); // VT_FILETIME = 64
    }

    [Fact]
    public void Named_AnsiString_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedAnsiStringProperty("AnsiVal", "hello");
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 30)); // VT_LPSTR = 30
    }

    [Fact]
    public void Named_Blob_ContainsVtCode()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedBlobProperty("BlobVal", [0xDE, 0xAD, 0xBE, 0xEF]);
        byte[] result = builder.Build();
        Assert.True(ContainsUInt16(result, 65)); // VT_BLOB = 65
    }

    [Fact]
    public void Named_Int32_RoundTripsViaLnk()
    {
        var builder = new PropertyStoreBuilder();
        builder.AddNamedInt32Property("TestInt", 12345);
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            PropertyStoreData = builder.Build()
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.PropertyStoreData);
    }

    [Fact]
    public void Named_FileTime_RoundTripsViaLnk()
    {
        var builder = new PropertyStoreBuilder();
        var dt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.AddNamedFileTimeProperty("TestDate", dt);
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            PropertyStoreData = builder.Build()
        });
        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.PropertyStoreData);

        var entries = PropertyStoreReader.Parse(options.PropertyStoreData);
        var entry = entries.Find(e => e.Name == "TestDate");
        Assert.NotNull(entry);
        Assert.Equal(64, entry.VtType); // VT_FILETIME
        Assert.Equal(dt, entry.Value);
    }

    private static bool ContainsUInt16(byte[] data, ushort value)
    {
        byte[] target = BitConverter.GetBytes(value);
        for (int i = 0; i <= data.Length - 2; i++)
            if (data[i] == target[0] && data[i + 1] == target[1])
                return true;
        return false;
    }
}
