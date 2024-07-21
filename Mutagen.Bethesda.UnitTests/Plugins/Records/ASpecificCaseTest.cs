using System.IO.Abstractions;
using FluentAssertions;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing;
using Noggog;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public abstract class ASpecificCaseTest<TSetter, TGetter>
    where TSetter : class, TGetter
    where TGetter : class, ILoquiObjectGetter, IBinaryItem
{
    public abstract ModPath Path { get; }
    public abstract GameRelease Release { get; }
    public virtual bool TestPassthrough => true;

    public abstract void TestItem(TGetter item);

    [Theory, DefaultAutoData]
    public void Direct(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        if (!LoquiBinaryTranslation<TSetter>.Instance.Parse(
                TestDataPathing.GetReadFrame(Path, Release),
                out var item))
        {
            throw new ArgumentException();
        }
        TestItem(item);
        if (TestPassthrough)
        {
            var somePath = System.IO.Path.Combine(existingDir, Path.ModKey.FileName);
            using (var writer = TestDataPathing.GetWriter(somePath, Release, fileSystem: fileSystem))
            {
                item.WriteToBinary(writer);
            }

            var origBytes = File.ReadAllBytes(Path);
            var newBytes = fileSystem.File.ReadAllBytes(somePath);
            newBytes.Should().Equal(origBytes);
        }
    }

    [Theory, DefaultAutoData]
    public void Overlay(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var masters = SeparatedMasterPackage.NotSeparate(
            new MasterReferenceCollection(
                Path.ModKey));
        var item = LoquiBinaryOverlayTranslation<TGetter>.Create(
            TestDataPathing.GetOverlayStream(Path, Release),
            new BinaryOverlayFactoryPackage(
                new ParsingMeta(
                    GameConstants.Get(Release),
                    Path.ModKey,
                    masters)),
            default);
        TestItem(item);
        if (TestPassthrough)
        {
            var somePath = System.IO.Path.Combine(existingDir, Path.ModKey.FileName);
            using (var writer = TestDataPathing.GetWriter(somePath, Release, fileSystem: fileSystem))
            {
                item.WriteToBinary(writer);
            }

            var origBytes = File.ReadAllBytes(Path);
            var newBytes = fileSystem.File.ReadAllBytes(somePath);
            newBytes.Should().Equal(origBytes);
        }
    }
}