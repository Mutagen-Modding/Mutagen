using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class OverlayPrimitiveBreakHandlingTests
{
    [Fact]
    public void PlacedObjectReflectionMissingData()
    {
        var overlayStream = TestDataPathing.GetOverlayStream(
            TestDataPathing.SkyrimPlacedObjectReflectedWaterMissingData, 
            GameRelease.SkyrimSE);
        var obj = PlacedObjectBinaryOverlay.PlacedObjectFactory(
            overlayStream,
            new BinaryOverlayFactoryPackage(overlayStream.MetaData));
        obj.Reflections.Count.Should().Be(1);
        obj.Reflections.First().Type.Should().Be(default);
    }
}