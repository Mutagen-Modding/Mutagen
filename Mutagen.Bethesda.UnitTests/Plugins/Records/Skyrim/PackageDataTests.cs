using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class PackageDataTests
{
    private static readonly RecordType ANAM = new("ANAM");
    private static readonly RecordType CNAM = new("CNAM");
    private static readonly RecordType UNAM = new("UNAM");
    private static readonly RecordType BNAM = new("BNAM");
    private static readonly RecordType PNAM = new("PNAM");

    private static IMutagenReadStream CreateStream(byte[] bytes)
    {
        return new MutagenMemoryReadStream(
            bytes,
            new ParsingMeta(
                GameConstants.SkyrimSE,
                ModKey.Null,
                SeparatedMasterPackage.EmptyNull));
    }

    /// <summary>
    /// Writes package data binary with optional count mismatch.
    /// The binary format is:
    ///   Phase 1 (types+values): ANAM subrecords with type strings, followed by CNAM with values
    ///   Phase 2 (indexes): UNAM subrecords with index bytes
    /// </summary>
    private static byte[] WritePackageDataBytes(int headerCount, params (string type, sbyte index)[] entries)
    {
        var memStream = new MemoryStream();
        using (var writer = new MutagenWriter(memStream, GameConstants.SkyrimSE, dispose: false))
        {
            // Phase 1: Write ANAM type entries with their values
            foreach (var entry in entries)
            {
                using (HeaderExport.Subrecord(writer, ANAM))
                {
                    writer.Write(entry.type, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                }

                // Write a default CNAM value for each type
                using (HeaderExport.Subrecord(writer, CNAM))
                {
                    switch (entry.type)
                    {
                        case "Bool":
                            writer.Write((byte)1);
                            break;
                        case "Float":
                            writer.Write(1.0f);
                            break;
                        case "Int":
                            writer.Write((uint)42);
                            break;
                        default:
                            writer.Write((uint)0);
                            break;
                    }
                }
            }

            // Phase 2: Write UNAM index entries
            foreach (var entry in entries)
            {
                using (HeaderExport.Subrecord(writer, UNAM))
                {
                    writer.Write(entry.index);
                }
            }
        }

        return memStream.ToArray();
    }

    [Fact]
    public void FillPackageData_MatchingCount_ParsesSuccessfully()
    {
        var bytes = WritePackageDataBytes(
            headerCount: 2,
            ("Bool", 0),
            ("Int", 1));

        var stream = CreateStream(bytes);
        var data = new Dictionary<sbyte, APackageData>();

        PackageBinaryCreateTranslation.FillPackageData(stream, expectedCount: 2, data);

        data.Count.ShouldBe(2);
        data[0].ShouldBeOfType<PackageDataBool>();
        data[1].ShouldBeOfType<PackageDataInt>();
    }

    [Fact]
    public void FillPackageData_MatchingCount_ParsesValues()
    {
        var bytes = WritePackageDataBytes(
            headerCount: 2,
            ("Bool", 0),
            ("Int", 1));

        var stream = CreateStream(bytes);
        var data = new Dictionary<sbyte, APackageData>();

        PackageBinaryCreateTranslation.FillPackageData(stream, expectedCount: 2, data);

        var boolData = data[0].ShouldBeOfType<PackageDataBool>();
        boolData.Data.ShouldBeTrue();

        var intData = data[1].ShouldBeOfType<PackageDataInt>();
        intData.Data.ShouldBe(42u);
    }

    [Fact]
    public void FillPackageData_FewerActualThanExpected_DoesNotThrow()
    {
        // Header says 3, but only 2 entries exist in the binary data.
        // This is the scenario that previously threw "Unexpected data count mismatch".
        var bytes = WritePackageDataBytes(
            headerCount: 3,
            ("Bool", 0),
            ("Int", 1));

        var stream = CreateStream(bytes);
        var data = new Dictionary<sbyte, APackageData>();

        // Should not throw - the parser should use the actual count
        PackageBinaryCreateTranslation.FillPackageData(stream, expectedCount: 3, data);

        data.Count.ShouldBe(2);
        data[0].ShouldBeOfType<PackageDataBool>();
        data[1].ShouldBeOfType<PackageDataInt>();
    }

    [Fact]
    public void FillPackageData_MoreActualThanExpected_DoesNotThrow()
    {
        // Header says 1, but 2 entries exist in the binary data.
        var bytes = WritePackageDataBytes(
            headerCount: 1,
            ("Bool", 0),
            ("Float", 1));

        var stream = CreateStream(bytes);
        var data = new Dictionary<sbyte, APackageData>();

        // Should not throw
        PackageBinaryCreateTranslation.FillPackageData(stream, expectedCount: 1, data);

        data.Count.ShouldBe(2);
        data[0].ShouldBeOfType<PackageDataBool>();
        data[1].ShouldBeOfType<PackageDataFloat>();
    }

    [Fact]
    public void FillPackageData_SingleEntry_ParsesCorrectly()
    {
        var bytes = WritePackageDataBytes(
            headerCount: 1,
            ("Float", 0));

        var stream = CreateStream(bytes);
        var data = new Dictionary<sbyte, APackageData>();

        PackageBinaryCreateTranslation.FillPackageData(stream, expectedCount: 1, data);

        data.Count.ShouldBe(1);
        var floatData = data[0].ShouldBeOfType<PackageDataFloat>();
        floatData.Data.ShouldBe(1.0f);
    }

    [Fact]
    public void FillPackageData_ZeroExpectedWithActualEntries_DoesNotThrow()
    {
        // Header says 0, but entries exist
        var bytes = WritePackageDataBytes(
            headerCount: 0,
            ("Bool", 0));

        var stream = CreateStream(bytes);
        var data = new Dictionary<sbyte, APackageData>();

        PackageBinaryCreateTranslation.FillPackageData(stream, expectedCount: 0, data);

        data.Count.ShouldBe(1);
    }
}
