using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.UnitTests.Placeholders;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class FormKeyTests
{
    [Fact]
    public void Import_String()
    {
        Assert.True(
            FormKey.TryFactory("00C51A:Oblivion.esm", out FormKey id));
        Assert.Equal(
            new FormKey(modKey: new ModKey("Oblivion", ModType.Master), id: 0x00C51A),
            id);
    }
    
    [Theory, MutagenAutoData]
    public void Import_FilesafeString(FormKey fk)
    {
        FormKey.TryFactory(fk.ToFilesafeString(), out var id)
            .Should().BeTrue();
        id.Should().Be(fk);
    }

    [Fact]
    public void Import_String0x()
    {
        Assert.True(
            FormKey.TryFactory("00C51A:Oblivion.esm", out FormKey id));
        Assert.Equal(
            new FormKey(modKey: new ModKey("Oblivion", ModType.Master), id: 0x00C51A),
            id);
    }

    [Fact]
    public void Import_LoopbackString()
    {
        var formKey = new FormKey(modKey: new ModKey("Oblivion", ModType.Master), id: 0x00C51A);
        Assert.True(
            FormKey.TryFactory(formKey.ToString(), out FormKey id));
        Assert.Equal(
            formKey,
            id);
    }

    [Fact]
    public void Import_Null()
    {
        Assert.True(
            FormKey.TryFactory(FormKey.NullStr, out FormKey id));
        Assert.Equal(
            FormKey.Null,
            id);
        Assert.Equal(
            ModKey.Null,
            id.ModKey);
        Assert.True(id.IsNull);
    }

    [Fact]
    public void Import_None()
    {
        Assert.True(
            FormKey.TryFactory(FormKey.NoneStr, out FormKey id));
        Assert.Equal(
            FormKey.None,
            id);
        Assert.Equal(
            ModKey.Null,
            id.ModKey);
        Assert.True(id.IsNull);
    }

    [Fact]
    public void Import_Null_ID()
    {
        Assert.True(
            FormKey.TryFactory($"{FormKey.NullStr}:{TestConstants.Skyrim}", out FormKey id));
        Assert.Equal(
            TestConstants.Skyrim.MakeFormKey(0),
            id);
        Assert.Equal(
            TestConstants.Skyrim,
            id.ModKey);
        Assert.True(id.IsNull);
    }

    [Fact]
    public void Import_Null_Strs()
    {
        Assert.True(
            FormKey.TryFactory($"{FormKey.NullStr}:{FormKey.NullStr}", out FormKey id));
        Assert.Equal(
            FormKey.Null,
            id);
        Assert.Equal(
            ModKey.Null,
            id.ModKey);
        Assert.True(id.IsNull);
    }

    [Fact]
    public void Import_Whitespace()
    {
        Assert.False(
            FormKey.TryFactory(" ", out FormKey id));
    }

    [Fact]
    public void Import_Malformed()
    {
        Assert.False(
            FormKey.TryFactory("00C51AOblivionesm", out FormKey id));
    }

    #region BinaryTranslation
    public ModKey TargetModKey() => new ModKey("Master2", ModType.Master);

    public IReadOnlyMasterReferenceCollection TypicalMasters() =>
        new MasterReferenceCollection(
            TestConstants.PluginModKey,
            new List<IMasterReferenceGetter>()
            {
                new MasterReference()
                {
                    Master = new ModKey("Master1", ModType.Master)
                },
                new MasterReference()
                {
                    Master = TargetModKey()
                },
                new MasterReference()
                {
                    Master = new ModKey("Master3", type: ModType.Master)
                },
            });

    [Fact]
    public void BinaryTranslation_Typical()
    {
        byte[] b = new byte[]
        {
            0x56,
            0x34,
            0x12,
            1
        };
        var formKey = FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters());
        Assert.Equal(TargetModKey(), formKey.ModKey);
        Assert.Equal((uint)0x123456, formKey.ID);
        Assert.False(formKey.IsNull);
    }

    [Fact]
    public void BinaryTranslation_TooShort()
    {
        byte[] b = new byte[]
        {
            0x56,
            0x34,
            0x12,
        };
        Assert.Throws<ArgumentOutOfRangeException>(() => FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters()));
    }

    [Fact]
    public void BinaryTranslation_TooLong()
    {
        byte[] b = new byte[]
        {
            0x56,
            0x34,
            0x12,
            1,
            0x99
        };
        var formKey = FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters());
        Assert.Equal(TargetModKey(), formKey.ModKey);
        Assert.Equal((uint)0x123456, formKey.ID);
        Assert.False(formKey.IsNull);
    }

    [Fact]
    public void BinaryTranslation_Zeros()
    {
        byte[] b = new byte[]
        {
            0,0,0,0
        };
        var formKey = FormKeyBinaryTranslation.Instance.Parse(b.AsSpan(), TypicalMasters());
        Assert.Equal(ModKey.Null, formKey.ModKey);
        Assert.Equal((uint)0, formKey.ID);
        Assert.True(formKey.IsNull);
    }
    #endregion

    #region Null
    [Fact]
    public void Null_TypicalNotNull()
    {
        TestConstants.Skyrim.IsNull.Should().BeFalse();
    }

    [Fact]
    public void Null_NullIsNull()
    {
        FormKey.Null.IsNull.Should().BeTrue();
    }

    [Fact]
    public void Null_NoneIsNull()
    {
        FormKey.None.IsNull.Should().BeTrue();
    }

    [Fact]
    public void Null_ExistingModKeyIsNull()
    {
        new FormKey(TestConstants.Skyrim, 0).IsNull.Should().BeTrue();
    }

    [Fact]
    public void Null_ExistingIdIsNull()
    {
        new FormKey(ModKey.Null, 123456).IsNull.Should().BeTrue();
    }
    #endregion
    
    #region Equality

    [Fact]
    public void SelfEquality()
    {
        var fk = FormKey.Factory("123456:Skyrim.esm");
        fk.Should().Be(fk);
    }

    [Fact]
    public void NullEquality()
    {
        var fk = FormKey.Null;
        fk.Should().Be(fk);
    }

    [Fact]
    public void NoneEquality()
    {
        var fk = FormKey.None;
        fk.Should().Be(fk);
    }

    [Fact]
    public void NotNullEquality()
    {
        var fk = FormKey.Factory("123456:Skyrim.esm");
        fk.Should().NotBe(FormKey.Null);
    }

    [Fact]
    public void DifferentModKeyEquality()
    {
        var fk = FormKey.Factory("123456:Skyrim.esm");
        var fk2 = FormKey.Factory("123456:Skyrim2.esm");
        fk.Should().NotBe(fk2);
    }
    
    #endregion

    #region Comparers
    #region Alphabetical
    [Fact]
    public void Comparer_Alphabetical_ByMaster()
    {
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Oblivion.esp");
        var compare = FormKey.AlphabeticalComparer();
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_Alphabetical_ByMasterGreater()
    {
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Oblivion.esp");
        var compare = FormKey.AlphabeticalComparer();
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_Alphabetical_ByName()
    {
        FormKey k1 = FormKey.Factory("00C51A:Knights.esm");
        FormKey k2 = FormKey.Factory("00C51A:Oblivion.esm");
        var compare = FormKey.AlphabeticalComparer();
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_Alphabetical_ByNameGreater()
    {
        FormKey k1 = FormKey.Factory("00C51A:Knights.esm");
        FormKey k2 = FormKey.Factory("00C51A:Oblivion.esm");
        var compare = FormKey.AlphabeticalComparer();
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_Alphabetical_ByID()
    {
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.AlphabeticalComparer();
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_Alphabetical_ByIDGreater()
    {
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.AlphabeticalComparer();
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_Alphabetical_Equal()
    {
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Oblivion.esm");
        var compare = FormKey.AlphabeticalComparer();
        Assert.Equal(0, compare.Compare(k2, k1));
    }
    #endregion
    #region ModKey List
    [Fact]
    public void Comparer_ModKeyList_Typical()
    {
        List<ModKey> modKeys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Oblivion.esm"),
            ModKey.FromNameAndExtension("Knights.esm"),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
        var compare = FormKey.LoadOrderComparer(modKeys);
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_ModKeyList_TypicalGreater()
    {
        List<ModKey> modKeys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Oblivion.esm"),
            ModKey.FromNameAndExtension("Knights.esm"),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
        var compare = FormKey.LoadOrderComparer(modKeys);
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_ModKeyList_Fallback()
    {
        List<ModKey> modKeys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Oblivion.esm"),
            ModKey.FromNameAndExtension("Knights.esm"),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.LoadOrderComparer(modKeys);
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_ModKeyList_FallbackGreater()
    {
        List<ModKey> modKeys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Oblivion.esm"),
            ModKey.FromNameAndExtension("Knights.esm"),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.LoadOrderComparer(modKeys);
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_ModKeyList_Unknown()
    {
        List<ModKey> modKeys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Oblivion.esm"),
            ModKey.FromNameAndExtension("Knights.esm"),
        };
        FormKey k1 = FormKey.Factory("00C51A:MyMod.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.LoadOrderComparer(modKeys);
        Assert.Throws<ArgumentOutOfRangeException>(() => compare.Compare(k1, k2));
    }
    #endregion
    #region LoadOrder List
    [Theory, MutagenAutoData]
    public void Comparer_LoadOrder_Typical()
    {
        var loadOrder = new LoadOrder<IMod>()
        {
            new TestMod(ModKey.FromNameAndExtension("Oblivion.esm")),
            new TestMod(ModKey.FromNameAndExtension("Knights.esm")),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
        var compare = FormKey.LoadOrderComparer(loadOrder);
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_LoadOrder_TypicalGreater()
    {
        var loadOrder = new LoadOrder<IMod>()
        {
            new TestMod(ModKey.FromNameAndExtension("Oblivion.esm")),
            new TestMod(ModKey.FromNameAndExtension("Knights.esm")),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51A:Knights.esm");
        var compare = FormKey.LoadOrderComparer(loadOrder);
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_LoadOrder_Fallback()
    {
        var loadOrder = new LoadOrder<IMod>()
        {
            new TestMod(ModKey.FromNameAndExtension("Oblivion.esm")),
            new TestMod(ModKey.FromNameAndExtension("Knights.esm")),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.LoadOrderComparer(loadOrder);
        Assert.True(compare.Compare(k1, k2) < 0);
    }

    [Fact]
    public void Comparer_LoadOrder_FallbackGreater()
    {
        var loadOrder = new LoadOrder<IMod>()
        {
            new TestMod(ModKey.FromNameAndExtension("Oblivion.esm")),
            new TestMod(ModKey.FromNameAndExtension("Knights.esm")),
        };
        FormKey k1 = FormKey.Factory("00C51A:Oblivion.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.LoadOrderComparer(loadOrder);
        Assert.True(compare.Compare(k2, k1) > 0);
    }

    [Fact]
    public void Comparer_LoadOrder_Unknown()
    {
        var loadOrder = new LoadOrder<IMod>()
        {
            new TestMod(ModKey.FromNameAndExtension("Oblivion.esm")),
            new TestMod(ModKey.FromNameAndExtension("Knights.esm")),
        };
        FormKey k1 = FormKey.Factory("00C51A:MyMod.esm");
        FormKey k2 = FormKey.Factory("00C51B:Oblivion.esm");
        var compare = FormKey.LoadOrderComparer(loadOrder);
        Assert.Throws<ArgumentOutOfRangeException>(() => compare.Compare(k1, k2));
    }
    #endregion
    #endregion

    #region Strings

    [Fact]
    public void TypicalToString()
    {
        var str = "123456:Skyrim.esm";
        FormKey.Factory(str)
            .ToString()
            .Should().Be(str);
    }

    [Fact]
    public void NoneToString()
    {
        FormKey.None
            .ToString()
            .Should().Be(FormKey.NoneStr);
    }

    [Fact]
    public void NullToString()
    {
        FormKey.Null
            .ToString()
            .Should().Be(FormKey.NullStr);
        FormKey.Factory("Null:Null")
            .ToString()
            .Should().Be("Null");
        FormKey.Factory("000000:Null")
            .ToString()
            .Should().Be("Null");
        FormKey.Factory("000000:Skyrim.esm")
            .ToString()
            .Should().Be("000000:Skyrim.esm");
        FormKey.Factory("123456:Null")
            .ToString()
            .Should().Be("123456:Null");
    }

    #endregion
}