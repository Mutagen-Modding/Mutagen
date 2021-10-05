using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Skyrim.Records
{
    public abstract class ASkyrimRegionTests
    {
        public abstract IRegionGetter Get(ModPath modPath);
        
        [Fact]
        public void FunctionParametersTypeNone()
        {
            var region = Get(TestDataPathing.SkyrimSoundRegionDataWithNoSecondSubrecord);
            region.Sounds.Should().NotBeNull();
            region.Sounds!.Sounds.Should().BeNull();
        }
    }
    
    public class SkyrimRegionDirectTests : ASkyrimRegionTests
    {
        public override IRegionGetter Get(ModPath path)
        {
            return Region.CreateFromBinary(
                TestDataPathing.GetReadFrame(
                    path,
                    GameRelease.SkyrimSE));
        }
    }
    
    public class SkyrimRegionOverlayTests : ASkyrimRegionTests
    {
        public override IRegionGetter Get(ModPath path)
        {
            var overlayStream = TestDataPathing.GetOverlayStream(path.Path, GameRelease.SkyrimSE);
            return RegionBinaryOverlay.RegionFactory(
                overlayStream,
                new BinaryOverlayFactoryPackage(overlayStream.MetaData));
        }
    }
}