using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class TraversalsTests
{
    [Fact]
    public void EdgeCaseMutable()
    {
        var overlayStream = TestDataPathing.GetReadFrame(
            TestDataPathing.StarfieldTraversals, 
            GameRelease.Starfield);
        var obj = Cell.CreateFromBinary(
            overlayStream);
        obj.Temporary.ShouldHaveCount(2);
        // obj.Traversals.ShouldHaveCount(3);
    }
    
    [Fact]
    public void EdgeCaseOverlay()
    {
        var overlayStream = TestDataPathing.GetOverlayStream(
            TestDataPathing.StarfieldTraversals, 
            GameRelease.Starfield);
        var obj = CellBinaryOverlay.CellFactory(
            overlayStream,
            new BinaryOverlayFactoryPackage(overlayStream.MetaData));
        obj.Temporary.ShouldHaveCount(2);
        // obj.Traversals.ShouldHaveCount(3);
    }
}