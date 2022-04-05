using System.Collections.Generic;
using System.Linq;
using AutoFixture.Kernel;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class ModPathMultipleParameterBuilderTests
{
    class NotInterestingQueries
    {
        public void Enumerable(IEnumerable<ModPath> mks)
        {
        }
    }
        
    [Theory, MutagenAutoData]
    public void UninterestingNameReturnsNoSpecimen(
        ISpecimenContext context,
        ModPathMultipleParameterBuilder sut)
    {
        foreach (var method in typeof(NotInterestingQueries).Methods())
        {
            var param = method.GetParameters().First();
            var ret = sut.Create(param, context);
            ret.Should().BeOfType<NoSpecimen>();
        }
    }
        
    class ExistingQuery
    {
        public void Enumerable(IEnumerable<ModKey> existing)
        {
        }
    }
        
    [Theory, MutagenAutoData]
    public void ExistsQueriesSplitService(
        ISpecimenContext context,
        ModKeyMultipleParameterBuilder sut)
    {
        foreach (var method in typeof(ExistingQuery).Methods())
        {
            var param = method.GetParameters().First();
            var ret = sut.Create(param, context);
            sut.SplitEnumerableIntoSubtypes
                .Received(1)
                .Split<ModKey>(context, param.ParameterType);
        }
    }
        
    [Theory, MutagenAutoData]
    public void NoSpecimenSplitServiceReturnsNoSpecimen(
        ISpecimenContext context,
        ModKeyMultipleParameterBuilder sut)
    {
        sut.SplitEnumerableIntoSubtypes
            .Split<ModKey>(default!, default!)
            .ReturnsForAnyArgs(new NoSpecimen());
        foreach (var method in typeof(ExistingQuery).Methods())
        {
            var param = method.GetParameters().First();
            sut.Create(param, context)
                .Should().BeOfType<NoSpecimen>();
        }
    }
        
    [Theory, MutagenAutoData]
    public void SplitServiceResultsPipedIntoMakeModExist(
        ISpecimenContext context,
        ModKey[] modKeys,
        ModKeyMultipleParameterBuilder sut)
    {
        foreach (var method in typeof(ExistingQuery).Methods())
        {
            sut.SplitEnumerableIntoSubtypes
                .Split<ModKey>(default!, default!)
                .ReturnsForAnyArgs(modKeys);
            var param = method.GetParameters().First();
            sut.MakeModExist.ClearReceivedCalls();
                
            sut.Create(param, context);

            foreach (var modKey in modKeys)
            {
                sut.MakeModExist.MakeExist(modKey, context);
            }
        }
    }
        
    [Theory, MutagenAutoData]
    public void ExistsReturnsSplitResults(
        ISpecimenContext context,
        ModKey[] modKeys,
        ModKeyMultipleParameterBuilder sut)
    {
        foreach (var method in typeof(ExistingQuery).Methods())
        {
            sut.SplitEnumerableIntoSubtypes
                .Split<ModKey>(default!, default!)
                .ReturnsForAnyArgs(modKeys);
            var param = method.GetParameters().First();
            sut.MakeModExist.ClearReceivedCalls();
                
            sut.Create(param, context);

            foreach (var modKey in modKeys)
            {
                sut.MakeModExist.MakeExist(modKey, context);
            }
        }
    }
}