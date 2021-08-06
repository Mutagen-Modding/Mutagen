using System.IO;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class MakeModExistTests
    {
        [Theory, BasicAutoData]
        public void QueriesContextForDataDirectory(
            ISpecimenContext context,
            ModKey modKey,
            MakeModExist sut)
        {
            context.MockToReturn<IDataDirectoryProvider>();
            sut.MakeExist(modKey, context);
            context.ShouldHaveCreated<IDataDirectoryProvider>();
        }
        
        [Theory, BasicAutoData]
        public void ClassMakeFileExistWithDataDirectory(
            DirectoryPath directoryPath,
            IDataDirectoryProvider dataDirectoryProvider,
            ISpecimenContext context,
            ModKey modKey,
            MakeModExist sut)
        {
            dataDirectoryProvider.Path.Returns(directoryPath);
            context.MockToReturn<IDataDirectoryProvider>(dataDirectoryProvider);
            sut.MakeExist(modKey, context);
            sut.MakeFileExist
                .Received(1)
                .MakeExist(
                    Path.Combine(directoryPath, modKey.FileName),
                    context);
        }
    }
}