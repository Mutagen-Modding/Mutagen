using System.Collections.Generic;
using System.Linq;
using AutoFixture.Kernel;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class ModKeyMultipleBuilderTests
    {
        [Theory, BasicAutoData]
        public void ReturnsModKeys(
            ISpecimenContext context,
            ModKeyMultipleBuilder sut)
        {
            sut.Create(new MultipleRequest(typeof(ModKey)), context)
                .Should().BeAssignableTo<IEnumerable<ModKey>>();
        }
        
        [Theory, BasicAutoData]
        public void ReturnsDifferentModKeysWithin(
            ISpecimenContext context,
            ModKeyMultipleBuilder sut)
        {
            var keys = sut.Create(new MultipleRequest(typeof(ModKey)), context) as IEnumerable<ModKey>;
            foreach (var key in keys!)
            {
                keys.Where(x => x == key).Should().HaveCount(1);
            }
        }
        
        [Theory, BasicAutoData]
        public void MultipleEnumerationsYieldsSameResults(
            ISpecimenContext context,
            ModKeyMultipleBuilder sut)
        {
            var keys = sut.Create(new MultipleRequest(typeof(ModKey)), context) as IEnumerable<ModKey>;
            keys.Should().Equal(keys);
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
                keys2.Should().NotContain(key);
            }
        }
    }
}