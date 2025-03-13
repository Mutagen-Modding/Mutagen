﻿using AutoFixture.Kernel;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class ModKeyMultipleBuilderTests
{
    [Theory, BasicAutoData]
    public void ReturnsModKeys(
        ISpecimenContext context,
        ModKeyMultipleBuilder sut)
    {
        sut.Create(new MultipleRequest(typeof(ModKey)), context)
            .ShouldBeAssignableTo<IEnumerable<ModKey>>();
    }
        
    [Theory, BasicAutoData]
    public void ReturnsDifferentModKeysWithin(
        ISpecimenContext context,
        ModKeyMultipleBuilder sut)
    {
        var keys = sut.Create(new MultipleRequest(typeof(ModKey)), context) as IEnumerable<ModKey>;
        foreach (var key in keys!)
        {
            keys.Where(x => x == key).Count().ShouldBe(1);
        }
    }
        
    [Theory, BasicAutoData]
    public void MultipleEnumerationsYieldsSameResults(
        ISpecimenContext context,
        ModKeyMultipleBuilder sut)
    {
        var keys = sut.Create(new MultipleRequest(typeof(ModKey)), context) as IEnumerable<ModKey>;
        keys.ShouldBe(keys);
    }
        
    [Theory, BasicAutoData]
    public void SeparateCreatesReturnDifferentKeys(
        ISpecimenContext context,
        ModKeyMultipleBuilder sut)
    {
        var keys = sut.Create(new MultipleRequest(typeof(ModKey)), context) as IEnumerable<ModKey>;
        var keys2 = sut.Create(new MultipleRequest(typeof(ModKey)), context) as IEnumerable<ModKey>;
        foreach (var key in keys!)
        {
            keys2!.ShouldNotContain(key);
        }
    }
}