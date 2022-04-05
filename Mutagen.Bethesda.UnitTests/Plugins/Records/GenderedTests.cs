using System.IO;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class GenderedTests
{
    /// <summary>
    /// Tests that a head marker with a gendered marker that is empty doesn't throw,
    /// but instead acts as if it didn't exist
    /// </summary>
    [Fact]
    public void RaceHeadPart_DanglingMarker()
    {
        var frame = new MutagenFrame(
            new MutagenBinaryReadStream(
                File.OpenRead(TestDataPathing.RaceHeadPartDanglingMaster), 
                new ParsingBundle(
                    GameRelease.SkyrimSE, 
                    new MasterReferenceCollection(Constants.Skyrim))));
        var headData = GenderedItemBinaryTranslation.ParseMarkerPerItem<HeadData>(
            frame: frame,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            marker: RecordTypes.NAM0,
            femaleRecordConverter: Race_Registration.HeadDataFemaleConverter,
            transl: HeadData.TryCreateFromBinary);
    }
}