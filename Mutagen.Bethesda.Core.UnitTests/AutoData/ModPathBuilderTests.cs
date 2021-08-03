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
    public class ModPathBuilderTests
    {
        [Theory, BasicAutoData]
        public void QueriesModKey(
            ModKey modKey,
            ISpecimenContext context,
            ModPathBuilder sut)
        {
            context.MockToReturn(modKey);
            context.MockToReturn<IDataDirectoryProvider>();
            
            sut.Create(typeof(ModPath), context);
            
            context.AssertWasCreated<ModKey>();
        }
        
        [Theory, BasicAutoData]
        public void QueriesDataDirectory(
            ModKey modKey,
            ISpecimenContext context,
            ModPathBuilder sut)
        {
            context.MockToReturn(modKey);
            context.MockToReturn<IDataDirectoryProvider>();
            
            sut.Create(typeof(ModPath), context);
            
            context.AssertWasCreated<IDataDirectoryProvider>();
        }
        
        [Theory, BasicAutoData]
        public void ReturnsModPathWithinDataDirectory(
            ModKey modKey,
            DirectoryPath directoryPath,
            IDataDirectoryProvider dataDirectoryProvider,
            ISpecimenContext context,
            ModPathBuilder sut)
        {
            context.MockToReturn(modKey);
            dataDirectoryProvider.Path.Returns(directoryPath);
            context.MockToReturn(dataDirectoryProvider);
            
            var ret = sut.Create(typeof(ModPath), context);
            
            ret.Should().BeOfType<ModPath>();
            var modPath = (ModPath)ret;
            modPath.Path.IsUnderneath(directoryPath).Should().BeTrue();
        }
        
        [Theory, BasicAutoData]
        public void ReturnsModPathWithModKeyMatchingPath(
            ModKey modKey,
            DirectoryPath directoryPath,
            IDataDirectoryProvider dataDirectoryProvider,
            ISpecimenContext context,
            ModPathBuilder sut)
        {
            context.MockToReturn(modKey);
            dataDirectoryProvider.Path.Returns(directoryPath);
            context.MockToReturn(dataDirectoryProvider);
            
            var ret = sut.Create(typeof(ModPath), context);
            
            ret.Should().BeOfType<ModPath>();
            var modPath = (ModPath)ret;
            modPath.Path.Name.Should().Be(modPath.ModKey.FileName);
        }
    }
}