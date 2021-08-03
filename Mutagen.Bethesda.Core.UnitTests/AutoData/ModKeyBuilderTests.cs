using AutoFixture.Kernel;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class ModKeyBuilderTests
    {
        [Theory, BasicAutoData]
        public void TypeReturnsModKey(
            ISpecimenContext context,
            ModKeyBuilder sut)
        {
            sut.Create(typeof(ModKey), context)
                .Should().BeOfType<ModKey>();
        }
        
        [Theory, BasicAutoData]
        public void TypeReturnsPlugin(
            ISpecimenContext context,
            ModKeyBuilder sut)
        {
            ((ModKey) sut.Create(typeof(ModKey), context))
                .Type.Should().Be(ModType.Plugin);
        }
        
        [Theory, BasicAutoData]
        public void TypeReturnsDifferent(
            ISpecimenContext context,
            ModKeyBuilder sut)
        {
            sut.Create(typeof(ModKey), context)
                .Should().NotBe(
                    sut.Create(typeof(ModKey), context));
        }
    }
}