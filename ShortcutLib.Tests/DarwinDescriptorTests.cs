using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class DarwinDescriptorTests
{
    [Fact]
    public void TryDecode_Null_ReturnsNull()
    {
        Assert.Null(DarwinDescriptor.TryDecode(null));
    }

    [Fact]
    public void TryDecode_Empty_ReturnsNull()
    {
        Assert.Null(DarwinDescriptor.TryDecode(""));
    }

    [Fact]
    public void TryDecode_TooShort_ReturnsNull()
    {
        Assert.Null(DarwinDescriptor.TryDecode("ABC"));
    }

    [Fact]
    public void EncodeAndDecode_RoundTrips()
    {
        var productCode = Guid.Parse("12345678-ABCD-EF01-2345-6789ABCDEF01");
        string packed = DarwinDescriptor.EncodeCompressedGuid(productCode);

        // Should be 32 chars (full hex reversed)
        Assert.Equal(32, packed.Length);

        // Now create a descriptor and decode it
        var componentCode = Guid.Parse("FEDCBA98-7654-3210-FEDC-BA9876543210");
        string packedComponent = DarwinDescriptor.EncodeCompressedGuid(componentCode);
        string descriptor = packed + "MyFeature>" + packedComponent;

        var result = DarwinDescriptor.TryDecode(descriptor);
        Assert.NotNull(result);
        Assert.Equal(productCode, result.ProductCode);
        Assert.Equal("MyFeature", result.FeatureId);
        Assert.Equal(componentCode, result.ComponentCode);
    }

    [Fact]
    public void TryDecode_NoFeatureSeparator_FeatureIsRemainder()
    {
        var guid = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE");
        string packed = DarwinDescriptor.EncodeCompressedGuid(guid);
        // Just product code + feature with no ">" separator
        string descriptor = packed + "SomeFeature";

        var result = DarwinDescriptor.TryDecode(descriptor);
        Assert.NotNull(result);
        Assert.Equal(guid, result.ProductCode);
        Assert.Equal("SomeFeature", result.FeatureId);
        Assert.Equal(Guid.Empty, result.ComponentCode);
    }

    [Fact]
    public void EncodeCompressedGuid_Deterministic()
    {
        var guid = Guid.Parse("12345678-9ABC-DEF0-1234-567890ABCDEF");
        string packed1 = DarwinDescriptor.EncodeCompressedGuid(guid);
        string packed2 = DarwinDescriptor.EncodeCompressedGuid(guid);
        Assert.Equal(packed1, packed2);
    }
}
