using Autofac;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class EnvironmentSharingTests
{
    [Theory, MutagenAutoData]
    public void DataDirectoryShared(
        IDataDirectoryProvider a,
        IDataDirectoryProvider b)
    {
        a.ShouldBeSameAs(b);
    }

    [Theory, MutagenContainerAutoData]
    public void ContainerDataDirectoryShared(
        IDataDirectoryProvider dataDirectory,
        IContainer container)
    {
        container.Resolve<IDataDirectoryProvider>().ShouldBeSameAs(dataDirectory);
    }

    [Theory, MutagenContainerAutoData]
    public void ContainerGameDirectoryMatches(
        IGameDirectoryProvider gameDirectory,
        IContainer container)
    {
        container.Resolve<IGameDirectoryProvider>().Path.ShouldBe(gameDirectory.Path);
    }
}
