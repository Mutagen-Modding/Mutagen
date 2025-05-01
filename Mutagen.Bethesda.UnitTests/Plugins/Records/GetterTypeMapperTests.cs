using Shouldly;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class GetterTypeMapperTests
{
    [Fact]
    public void Warmup()
    {
        var mapper = new GetterTypeMapper(null!);
    }
    
    [Fact]
    public void Warmup2()
    {
        GetterTypeMapping.Instance.TryGetGetterType("Test", out var getter);
    }
    
    [Fact]
    internal void Typical()
    {
        GetterTypeMapping.Instance.TryGetGetterType(typeof(Armor), out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType(typeof(IArmor), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType(typeof(IArmorGetter), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
    }
    
    [Fact]
    internal void ByCategory()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(Armor), out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(IArmor), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(IArmorGetter), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
    }
    
    [Fact]
    internal void ByCategoryMiss()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,typeof(Armor), out var result)
            .ShouldBeFalse();
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,typeof(IArmor), out result)
            .ShouldBeFalse();
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,typeof(IArmorGetter), out result)
            .ShouldBeFalse();
    }
    
    [Fact]
    internal void TypicalByName()
    {
        GetterTypeMapping.Instance.TryGetGetterType("Mutagen.Bethesda.Skyrim.Armor", out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType("Mutagen.Bethesda.Skyrim.IArmor", out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType("Mutagen.Bethesda.Skyrim.IArmorGetter", out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
    }
    
    [Fact]
    internal void ByCategoryByName()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,"Mutagen.Bethesda.Skyrim.Armor", out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,"Mutagen.Bethesda.Skyrim.IArmor", out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,"Mutagen.Bethesda.Skyrim.IArmorGetter", out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IArmorGetter));
    }
    
    [Fact]
    internal void ByCategoryByNameMiss()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,"Mutagen.Bethesda.Skyrim.Armor", out var result)
            .ShouldBeFalse();
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,"Mutagen.Bethesda.Skyrim.IArmor", out result)
            .ShouldBeFalse();
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,"Mutagen.Bethesda.Skyrim.IArmorGetter", out result)
            .ShouldBeFalse();
    }
    
    [Fact]
    internal void AspectInterface()
    {
        GetterTypeMapping.Instance.TryGetGetterType(typeof(INamed), out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(INamedGetter));
        GetterTypeMapping.Instance.TryGetGetterType(typeof(INamedGetter), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(INamedGetter));
    }
    
    [Fact]
    internal void AspectInterfaceByCategory()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(INamed), out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(INamedGetter));
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(INamedGetter), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(INamedGetter));
    }
    
    [Fact]
    internal void LinkInterface()
    {
        GetterTypeMapping.Instance.TryGetGetterType(typeof(IPlaced), out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IPlacedGetter));
        GetterTypeMapping.Instance.TryGetGetterType(typeof(IPlacedGetter), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IPlacedGetter));
    }
    
    [Fact]
    internal void LinkInterfaceByCategory()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(IPlaced), out var result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IPlacedGetter));
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Skyrim,typeof(IPlacedGetter), out result)
            .ShouldBeTrue();
        result.ShouldBe(typeof(IPlacedGetter));
    }
    
    [Fact]
    internal void LinkInterfaceByCategoryMiss()
    {
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,typeof(IPlaced), out var result)
            .ShouldBeFalse();
        GetterTypeMapping.Instance.TryGetGetterType(GameCategory.Oblivion,typeof(IPlacedGetter), out result)
            .ShouldBeFalse();
    }
}