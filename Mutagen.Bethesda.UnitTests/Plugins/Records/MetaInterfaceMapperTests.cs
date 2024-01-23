using FluentAssertions;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MetaInterfaceMapperTests
{
    [Fact]
    internal void Typical()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IWorldspaceOrListGetter), out var regis)
            .Should().BeTrue();
        regis.Setter.Should().BeFalse();
        regis.Registrations.Select(x => x.GetterType).Should().Contain(typeof(IWorldspaceGetter));
    }
    
    [Fact]
    internal void ByCategory()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Skyrim, typeof(IWorldspaceOrListGetter), out var regis)
            .Should().BeTrue();
        regis.Setter.Should().BeFalse();
        regis.Registrations.Select(x => x.GetterType).Should().Contain(typeof(IWorldspaceGetter));
    }
    
    [Fact]
    internal void ByCategoryMiss()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Oblivion, typeof(IWorldspaceOrListGetter), out var regis)
            .Should().BeFalse();
    }
}