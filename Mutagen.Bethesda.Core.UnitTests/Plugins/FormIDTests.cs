using Mutagen.Bethesda.Plugins;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class FormIDTests
{
    [Fact]
    public void Import_Zero()
    {
        byte[] bytes = new byte[4];
        FormID id = FormID.Factory(bytes);
        Assert.Equal(0, id.ModIndex.ID);
        Assert.Equal(uint.MinValue, id.ID);
    }

    [Fact]
    public void Import_Typical()
    {
        byte[] bytes = new byte[4]
        {
            216,
            203,
            0,
            5,
        };
        FormID id = FormID.Factory(bytes);
        Assert.Equal(5, id.ModIndex.ID);
        Assert.Equal((uint)0xCBD8, id.ID);
    }

    [Fact]
    public void Import_String()
    {
        Assert.True(
            FormID.TryFactory("0100C51A", out FormID id));
        Assert.Equal(
            new FormID(modID: new ModIndex(1), id: 0x00C51A),
            id);
    }

    [Fact]
    public void Import_String0x()
    {
        Assert.True(
            FormID.TryFactory("0x0100C51A", out FormID id));
        Assert.Equal(
            new FormID(modID: new ModIndex(1), id: 0x00C51A),
            id);
    }

    [Fact]
    public void Ctor_Typical()
    {
        FormID id = new FormID(0x12345678);
        Assert.Equal((uint)(0x345678), id.ID);
        Assert.Equal((byte)(0x12), id.ModIndex.ID);
        Assert.Equal((uint)0x12345678, id.Raw);
    }

    [Fact]
    public void Ctor_WithModID()
    {
        FormID id = new FormID(new ModIndex(0x12), 0x00345678);
        Assert.Equal((uint)(0x345678), id.ID);
        Assert.Equal((byte)(0x12), id.ModIndex.ID);
        Assert.Equal((uint)0x12345678, id.Raw);
    }

    [Fact]
    public void Ctor_WithIncorrectID()
    {
        Assert.Throws<ArgumentException>(() => new FormID(new ModIndex(0x12), 0x99345678));
    }

    #region Light

    [Fact]
    public void Light_Ctor_Typical()
    {
        var id = new LightMasterFormID(0xFE345678);
        Assert.Equal((uint)(0x678), id.ID);
        Assert.Equal((uint)(0x345), id.ModIndex);
        Assert.Equal((uint)0xFE345678, id.Raw);
    }

    [Fact]
    public void Light_Ctor_WithModID()
    {
        var id = new LightMasterFormID(0x345, 0x678);
        Assert.Equal((uint)(0x678), id.ID);
        Assert.Equal((uint)(0x345), id.ModIndex);
        Assert.Equal((uint)0xFE345678, id.Raw);
    }

    [Fact]
    public void Light_Ctor_WithIncorrectID()
    {
        Assert.Throws<ArgumentException>(() => new LightMasterFormID(
            0x3456, 0x00000678));
        Assert.Throws<ArgumentException>(() => new LightMasterFormID(
            0x12, 0x00005678));
        Assert.Throws<ArgumentException>(() => new LightMasterFormID(
            0x12, 0x12000678));
        Assert.Throws<ArgumentException>(() => new LightMasterFormID(
            0x12345678));
    }

    #endregion

    #region Medium
    
    [Fact]
    public void Medium_Ctor_Typical()
    {
        var id = new MediumMasterFormID(0xFD345678);
        Assert.Equal((uint)(0x5678), id.ID);
        Assert.Equal((uint)(0x34), id.ModIndex);
        Assert.Equal((uint)0xFD345678, id.Raw);
    }

    [Fact]
    public void Medium_Ctor_WithModID()
    {
        var id = new MediumMasterFormID(0x34, 0x5678);
        Assert.Equal((uint)(0x5678), id.ID);
        Assert.Equal((uint)(0x34), id.ModIndex);
        Assert.Equal((uint)0xFD345678, id.Raw);
    }

    [Fact]
    public void Medium_Ctor_WithIncorrectID()
    {
        Assert.Throws<ArgumentException>(() => new MediumMasterFormID(
            0x345, 0x00000678));
        Assert.Throws<ArgumentException>(() => new MediumMasterFormID(
            0x34, 0x00045678));
        Assert.Throws<ArgumentException>(() => new MediumMasterFormID(
            0x34, 0x12000678));
        Assert.Throws<ArgumentException>(() => new MediumMasterFormID(
            0x12345678));
    }

    #endregion
}