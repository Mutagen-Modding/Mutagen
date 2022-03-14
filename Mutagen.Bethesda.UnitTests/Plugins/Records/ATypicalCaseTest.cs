using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public abstract class ATypicalCaseTest<TGetter>
{
    private readonly string _path;
    
    public abstract GameRelease Release { get; }

    public ATypicalCaseTest(string path)
    {
        _path = path;
    }
    
    [Fact]
    public void Direct()
    {
        Test(GetDirect(TestDataPathing.GetReadFrame(_path, Release)));
    }
    
    [Fact]
    public void Overlay()
    {
        Test(GetOverlay(TestDataPathing.GetOverlayStream(_path, Release), new BinaryOverlayFactoryPackage(
            new ParsingBundle(
                GameConstants.Get(Release), null!))));
    }
    
    public abstract void Test(TGetter getter);

    public abstract TGetter GetDirect(MutagenFrame frame);

    public abstract TGetter GetOverlay(OverlayStream stream, BinaryOverlayFactoryPackage package);
}