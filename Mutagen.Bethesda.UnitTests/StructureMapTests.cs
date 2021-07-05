using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.StructureMap;
using StructureMap;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class StructureMapTests
    {
        [Fact]
        public void EnsureValid()
        {
            var cont = new Container(c =>
            {
                c.AddRegistry<MutagenRegister>();
                c.For<IFileSystem>().Use<FileSystem>();
                c.For<IGameReleaseContext>().Use<GameReleasePlaceholder>();
            });
            cont.AssertConfigurationIsValid();
        }
    }
}