using Shouldly;
using Mutagen.Bethesda.Plugins;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class FormIDTests
{
    [Fact]
    public void Import_Zero()
    {
        byte[] bytes = new byte[4];
        FormID id = FormID.Factory(bytes);
        id.FullMasterIndex.ShouldEqual(0);
        id.FullId.ShouldBe(uint.MinValue);
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
        id.FullMasterIndex.ShouldEqual(5);
        id.FullId.ShouldEqual(0xCBD8);
    }

    [Fact]
    public void Import_String()
    {
        FormID.TryFactory("0100C51A", out FormID id).ShouldBeTrue();
        id.FullMasterIndex.ShouldEqual(1);
        id.FullId.ShouldEqual(0x00C51A);
        id.ShouldBe(new FormID(0x0100C51A));
    }

    [Fact]
    public void Import_String0x()
    {
        Assert.True(
            FormID.TryFactory("0x0100C51A", out FormID id));
        id.FullMasterIndex.ShouldEqual(1);
        id.FullId.ShouldEqual(0x00C51A);
        id.ShouldBe(new FormID(0x0100C51A));
    }

    [Fact]
    public void Ctor_Typical()
    {
        FormID id = new FormID(0x12345678);
        id.FullId.ShouldEqual(0x345678);
        id.FullMasterIndex.ShouldEqual(0x12);
        id.Raw.ShouldEqual(0x12345678);
    }

    [Fact]
    public void Light_Ctor_Typical()
    {
        var id = new FormID(0xFE345678);
        Assert.Equal((uint)(0x678), id.LightId);
        Assert.Equal((uint)(0x345), id.LightMasterIndex);
        Assert.Equal((uint)0xFE345678, id.Raw);
    }

    [Fact]
    public void Medium_Ctor_Typical()
    {
        var id = new FormID(0xFD345678);
        Assert.Equal((uint)(0x5678), id.MediumId);
        Assert.Equal((byte)(0x34), id.MediumMasterIndex);
        Assert.Equal((uint)0xFD345678, id.Raw);
    }
}