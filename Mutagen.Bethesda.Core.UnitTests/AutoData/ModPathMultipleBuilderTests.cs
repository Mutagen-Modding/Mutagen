using System.Collections.Generic;
using System.Linq;
using AutoFixture.Kernel;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class ModPathMultipleBuilderTests
    {
        [Theory, BasicAutoData]
        public void ReturnsModPaths(
            ISpecimenContext context,
            ModPathMultipleBuilder sut)
        {
            context.MockToReturn<IDataDirectoryProvider>();
            
            sut.Create(new MultipleRequest(typeof(ModPath)), context)
                .Should().BeAssignableTo<IEnumerable<ModPath>>();
        }
        
        [Theory, BasicAutoData]
        public void ReturnsDifferentModPathsWithin(
            ISpecimenContext context,
            ModPathMultipleBuilder sut)
        {
            context.MockToReturn<IDataDirectoryProvider>();
            
            var keys = sut.Create(new MultipleRequest(typeof(ModPath)), context) as IEnumerable<ModPath>;
            foreach (var key in keys!)
            {
                keys.Where(x => x == key).Should().HaveCount(1);
            }
        }
        
        [Theory, BasicAutoData]
        public void MultipleEnumerationsYieldsSameResults(
            ISpecimenContext context,
            ModPathMultipleBuilder sut)
        {
            context.MockToReturn<IDataDirectoryProvider>();
            
            var keys = sut.Create(new MultipleRequest(typeof(ModPath)), context) as IEnumerable<ModPath>;
            
            keys.Should().Equal(keys);
        }
        
        [Theory, BasicAutoData]
        public void SeparateCreatesReturnDifferentPaths(
            ISpecimenContext context,
            ModPathMultipleBuilder sut)
        {
            context.MockToReturn<IDataDirectoryProvider>();
            
            var keys = sut.Create(new MultipleRequest(typeof(ModPath)), context) as IEnumerable<ModPath>;
            var keys2 = sut.Create(new MultipleRequest(typeof(ModPath)), context) as IEnumerable<ModPath>;
            foreach (var key in keys!)
            {
                keys2.Should().NotContain(key);
            }
        }
        
        [Theory, BasicAutoData]
        public void ModPathsAreInDataDirectory(
            DirectoryPath dir,
            IDataDirectoryProvider dataDirectoryProvider,
            ISpecimenContext context,
            ModPathMultipleBuilder sut)
        {
            dataDirectoryProvider.Path.Returns(dir);
            context.MockToReturn(dataDirectoryProvider);
            
            var keys = sut.Create(new MultipleRequest(typeof(ModPath)), context) as IEnumerable<ModPath>;
            foreach (var key in keys!)
            {
                key.Path.IsUnderneath(dir);
            }
        }
        
        [Theory, BasicAutoData]
        public void ModKeysMatchPaths(
            ISpecimenContext context,
            ModPathMultipleBuilder sut)
        {
            context.MockToReturn<IDataDirectoryProvider>();
            
            var keys = sut.Create(new MultipleRequest(typeof(ModPath)), context) as IEnumerable<ModPath>;
            foreach (var key in keys!)
            {
                key.Path.Name.Equals(key.ModKey.FileName);
            }
        }
    }
}