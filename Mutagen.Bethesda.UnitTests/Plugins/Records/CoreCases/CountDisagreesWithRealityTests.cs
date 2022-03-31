using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.CoreCases;

public class CountDisagreesWithRealityTests
{
    [Fact]
    public void Test()
    {
        item.Items.Should().HaveCount(1);
        item.Class.FormKey.Should().Be(FormKey.Factory("123456:CountDisagreesWithReality.esp"));
    }
}
