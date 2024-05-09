using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public abstract class ASpecificCaseTest<TSetter, TGetter>
    where TSetter : class, TGetter
    where TGetter : class, ILoquiObjectGetter
{
    public abstract ModPath Path { get; }
    public abstract GameRelease Release { get; }

    public abstract void TestItem(TGetter item);

    [Fact]
    public void Direct()
    {
        if (!LoquiBinaryTranslation<TSetter>.Instance.Parse(
                TestDataPathing.GetReadFrame(Path, Release),
                out var item))
        {
            throw new ArgumentException();
        }
        TestItem(item);
    }

    [Fact]
    public void Overlay()
    {
        var item = LoquiBinaryOverlayTranslation<TGetter>.Create(
            TestDataPathing.GetOverlayStream(Path, Release),
            new BinaryOverlayFactoryPackage(
                new ParsingBundle(
                    GameConstants.Get(Release),
                    Path.ModKey,
                    new MasterReferenceCollection(
                        Path.ModKey))),
            default);
        TestItem(item);
    }
}