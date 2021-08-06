using System.Linq;
using AutoFixture.Kernel;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class ModPathParameterBuilderTests
    {
        class NonInteresting
        {
            public void NormalName(ModPath path)
            {
            }
        }
        
        [Theory, BasicAutoData]
        public void QueryingNonInterestingNameReturnsNoSpecimen(
            ISpecimenContext context,
            ModPathParameterBuilder sut)
        {
            foreach (var method in typeof(NonInteresting).Methods())
            {
                var param = method.GetParameters().First();
                sut.Create(param, context)
                    .Should().BeOfType<NoSpecimen>();
            }
        }
        
        class ExistingName
        {
            public void Start(ModPath existingName)
            {
            }
            
            public void End(ModPath nameExisting)
            {
            }
            
            public void Sandwich(ModPath nameExistingName)
            {
            }
        }
        
        [Theory, BasicAutoData]
        public void ExistingNameQueriesModPath(
            ModPath modPath,
            ISpecimenContext context,
            ModPathParameterBuilder sut)
        {
            context.MockToReturn(modPath);
            foreach (var method in typeof(ExistingName).Methods())
            {
                var param = method.GetParameters().First();
                context.ClearReceivedCalls();
                sut.Create(param, context);
                context.ShouldHaveCreated<ModPath>();
            }
        }
        
        [Theory, BasicAutoData]
        public void ExistingNameCallsToMakeExist(
            ModPath modPath,
            ISpecimenContext context,
            ModPathParameterBuilder sut)
        {
            context.MockToReturn(modPath);
            foreach (var method in typeof(ExistingName).Methods())
            {
                var param = method.GetParameters().First();
                sut.MakeModExist.ClearReceivedCalls();
                ModPath mk = (ModPath)sut.Create(param, context);
                sut.MakeModExist
                    .Received(1)
                    .MakeExist(mk, context);
            }
        }
    }
}