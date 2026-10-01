using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Shouldly;
using Xunit;
using static Mutagen.Bethesda.UnitTests.Plugins.Records.RawRecordBytes;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class GenderedSplitPairTests
{
    private static readonly GameConstants Constants = GameConstants.SkyrimSE;

    private static ParsingMeta Meta() => RawRecordBytes.Meta(Constants);

    private static byte[] MakeRecord(string type, params (string Type, byte[] Content)[] subrecords) =>
        Record(Constants, type, subrecords);

    private static MutagenFrame Frame(byte[] bytes) => new(new MutagenMemoryReadStream(bytes, Meta()));

    private static byte[] Write(ISkyrimMajorRecordGetter record)
    {
        var masters = new MasterReferenceCollection(TestModKey);
        var bundle = new WritingBundle(Constants)
        {
            MasterReferences = masters,
            SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(masters),
        };
        var memStream = new MemoryStream();
        using (var writer = new MutagenWriter(memStream, bundle, dispose: false))
        {
            record.WriteToBinary(writer);
        }
        return memStream.ToArray();
    }

    private static ArmorAddon ReadArmaDirect(byte[] bytes) => ArmorAddon.CreateFromBinary(Frame(bytes));

    private static IArmorAddonGetter ReadArmaOverlay(byte[] bytes)
    {
        var meta = Meta();
        return ArmorAddonBinaryOverlay.ArmorAddonFactory(
            new OverlayStream(bytes, meta),
            new BinaryOverlayFactoryPackage(meta));
    }

    private static IEnumerable<IArmorAddonGetter> ReadArmaBoth(byte[] bytes) =>
        new[] { ReadArmaDirect(bytes), ReadArmaOverlay(bytes) };

    private static IEnumerable<IRaceGetter> ReadRaceBoth(byte[] bytes)
    {
        var meta = Meta();
        yield return Race.CreateFromBinary(Frame(bytes));
        yield return RaceBinaryOverlay.RaceFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));
    }

    private static byte[] ArmaInterleaved() => MakeRecord("ARMA",
        ("EDID", Str("TestArma")),
        ("DNAM", new byte[12]),
        ("MOD2", Str("male.nif")),
        ("MO2T", new byte[] { 1, 2, 3, 4 }),
        ("MOD4", Str("male1st.nif")),
        ("MO4T", new byte[] { 5, 6, 7, 8 }),
        ("MOD3", Str("female.nif")),
        ("MOD5", Str("female1st.nif")));

    private static byte[] ArmaAdjacent() => MakeRecord("ARMA",
        ("EDID", Str("TestArma")),
        ("DNAM", new byte[12]),
        ("MOD2", Str("male.nif")),
        ("MO2T", new byte[] { 1, 2, 3, 4 }),
        ("MOD3", Str("female.nif")),
        ("MOD4", Str("male1st.nif")),
        ("MO4T", new byte[] { 5, 6, 7, 8 }),
        ("MOD5", Str("female1st.nif")));

    private static void AssertAllFourModels(IArmorAddonGetter arma)
    {
        arma.WorldModel.ShouldNotBeNull();
        arma.WorldModel.Male.ShouldNotBeNull();
        arma.WorldModel.Male.File.GivenPath.ShouldBe("male.nif");
        arma.WorldModel.Male.Data.ShouldNotBeNull();
        arma.WorldModel.Male.Data.Value.ToArray().ShouldBe(new byte[] { 1, 2, 3, 4 });
        arma.WorldModel.Female.ShouldNotBeNull();
        arma.WorldModel.Female.File.GivenPath.ShouldBe("female.nif");

        arma.FirstPersonModel.ShouldNotBeNull();
        arma.FirstPersonModel.Male.ShouldNotBeNull();
        arma.FirstPersonModel.Male.File.GivenPath.ShouldBe("male1st.nif");
        arma.FirstPersonModel.Male.Data.ShouldNotBeNull();
        arma.FirstPersonModel.Male.Data.Value.ToArray().ShouldBe(new byte[] { 5, 6, 7, 8 });
        arma.FirstPersonModel.Female.ShouldNotBeNull();
        arma.FirstPersonModel.Female.File.GivenPath.ShouldBe("female1st.nif");
    }

    [Fact]
    public void ArmorAddon_AdjacentPairs_ReadsAllFourModels()
    {
        foreach (var arma in ReadArmaBoth(ArmaAdjacent()))
        {
            AssertAllFourModels(arma);
        }
    }

    [Fact]
    public void ArmorAddon_InterleavedPairs_ReadsAllFourModels()
    {
        foreach (var arma in ReadArmaBoth(ArmaInterleaved()))
        {
            AssertAllFourModels(arma);
        }
    }

    [Fact]
    public void ArmorAddon_InterleavedPairs_WriteKeepsAllModelSubrecords()
    {
        foreach (var arma in ReadArmaBoth(ArmaInterleaved()))
        {
            var types = SubrecordTypes(Constants, Write(arma));
            types.ShouldContain("MOD2");
            types.ShouldContain("MO2T");
            types.ShouldContain("MOD3");
            types.ShouldContain("MOD4");
            types.ShouldContain("MO4T");
            types.ShouldContain("MOD5");
            AssertAllFourModels(ReadArmaDirect(Write(arma)));
        }
    }

    [Fact]
    public void ArmorAddon_InterleavedFormLinkPairs_ReadsAllFour()
    {
        var bytes = MakeRecord("ARMA",
            ("EDID", Str("TestArma")),
            ("DNAM", new byte[12]),
            ("NAM0", U32(0x801)),
            ("NAM2", U32(0x803)),
            ("NAM1", U32(0x802)),
            ("NAM3", U32(0x804)));

        foreach (var arma in ReadArmaBoth(bytes))
        {
            arma.SkinTexture.ShouldNotBeNull();
            arma.SkinTexture.Male.FormKey.ShouldBe(new FormKey(TestModKey, 0x801));
            arma.SkinTexture.Female.FormKey.ShouldBe(new FormKey(TestModKey, 0x802));
            arma.TextureSwapList.ShouldNotBeNull();
            arma.TextureSwapList.Male.FormKey.ShouldBe(new FormKey(TestModKey, 0x803));
            arma.TextureSwapList.Female.FormKey.ShouldBe(new FormKey(TestModKey, 0x804));
        }
    }

    [Fact]
    public void Armor_SeparatedWorldModelPair_ReadsBothHalves()
    {
        var bytes = MakeRecord("ARMO",
            ("EDID", Str("TestArmo")),
            ("MOD2", Str("male.nif")),
            ("EAMT", BitConverter.GetBytes((ushort)7)),
            ("MOD4", Str("female.nif")));
        var meta = Meta();
        var direct = Armor.CreateFromBinary(Frame(bytes));
        var overlay = ArmorBinaryOverlay.ArmorFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));

        foreach (var armo in new IArmorGetter[] { direct, overlay })
        {
            armo.EnchantmentAmount.ShouldBe((ushort)7);
            armo.WorldModel.ShouldNotBeNull();
            armo.WorldModel.Male.ShouldNotBeNull();
            armo.WorldModel.Male.Model.ShouldNotBeNull();
            armo.WorldModel.Male.Model.File.GivenPath.ShouldBe("male.nif");
            armo.WorldModel.Female.ShouldNotBeNull();
            armo.WorldModel.Female.Model.ShouldNotBeNull();
            armo.WorldModel.Female.Model.File.GivenPath.ShouldBe("female.nif");
        }
    }

    [Fact]
    public void AssociationType_InterleavedTitlePairs_ReadsAllFour()
    {
        var bytes = MakeRecord("ASTP",
            ("EDID", Str("TestAstp")),
            ("MPRT", Str("Father")),
            ("MCHT", Str("Son")),
            ("FPRT", Str("Mother")),
            ("FCHT", Str("Daughter")));
        var meta = Meta();
        var direct = AssociationType.CreateFromBinary(Frame(bytes));
        var overlay = AssociationTypeBinaryOverlay.AssociationTypeFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));

        foreach (var astp in new IAssociationTypeGetter[] { direct, overlay })
        {
            astp.ParentTitle.ShouldNotBeNull();
            astp.ParentTitle.Male.ShouldBe("Father");
            astp.ParentTitle.Female.ShouldBe("Mother");
            astp.Title.ShouldNotBeNull();
            astp.Title.Male.ShouldBe("Son");
            astp.Title.Female.ShouldBe("Daughter");
        }
    }

    [Fact]
    public void Race_SeparatedHeadDataBlocks_ReadsBothHalves()
    {
        var bytes = MakeRecord("RACE",
            ("EDID", Str("TestRace")),
            ("NAM0", Array.Empty<byte>()),
            ("MNAM", Array.Empty<byte>()),
            ("RPRM", U32(0x811)),
            ("NAM8", U32(0x900)),
            ("NAM0", Array.Empty<byte>()),
            ("FNAM", Array.Empty<byte>()),
            ("RPRF", U32(0x812)));

        foreach (var race in ReadRaceBoth(bytes))
        {
            race.MorphRace.FormKey.ShouldBe(new FormKey(TestModKey, 0x900));
            race.HeadData.ShouldNotBeNull();
            race.HeadData.Male.ShouldNotBeNull();
            race.HeadData.Male.RacePresets.Select(x => x.FormKey).ShouldBe(new[] { new FormKey(TestModKey, 0x811) });
            race.HeadData.Female.ShouldNotBeNull();
            race.HeadData.Female.RacePresets.Select(x => x.FormKey).ShouldBe(new[] { new FormKey(TestModKey, 0x812) });
        }
    }

    [Fact]
    public void Race_SeparatedSkeletalModelPair_ReadsBothHalves()
    {
        var bytes = MakeRecord("RACE",
            ("EDID", Str("TestRace")),
            ("MNAM", Array.Empty<byte>()),
            ("ANAM", Str("male.hkx")),
            ("GNAM", U32(0x901)),
            ("FNAM", Array.Empty<byte>()),
            ("ANAM", Str("female.hkx")));

        foreach (var race in ReadRaceBoth(bytes))
        {
            race.BodyPartData.FormKey.ShouldBe(new FormKey(TestModKey, 0x901));
            race.SkeletalModel.ShouldNotBeNull();
            race.SkeletalModel.Male.ShouldNotBeNull();
            race.SkeletalModel.Male.File.GivenPath.ShouldBe("male.hkx");
            race.SkeletalModel.Female.ShouldNotBeNull();
            race.SkeletalModel.Female.File.GivenPath.ShouldBe("female.hkx");
        }
    }

    [Fact]
    public void Race_MarkerWithNothingParsed_KeepsHalfReadEarlier()
    {
        var bytes = MakeRecord("RACE",
            ("EDID", Str("TestRace")),
            ("MNAM", Array.Empty<byte>()),
            ("ANAM", Str("male.hkx")),
            ("FNAM", Array.Empty<byte>()),
            ("ANAM", Str("female.hkx")),
            ("NAM1", Array.Empty<byte>()),
            ("MNAM", Array.Empty<byte>()),
            ("INDX", U32(0)),
            ("GNAM", U32(0x901)),
            ("FNAM", Array.Empty<byte>()),
            ("INDX", U32(0)));

        foreach (var race in ReadRaceBoth(bytes))
        {
            race.SkeletalModel.ShouldNotBeNull();
            race.SkeletalModel.Male.ShouldNotBeNull();
            race.SkeletalModel.Male.File.GivenPath.ShouldBe("male.hkx");
            race.SkeletalModel.Female.ShouldNotBeNull();
            race.SkeletalModel.Female.File.GivenPath.ShouldBe("female.hkx");
        }
    }

    [Fact]
    public void ArmorAddon_CopyInFromBinary_ReplacesPresentPairAndKeepsAbsentPair()
    {
        var arma = ReadArmaDirect(ArmaAdjacent());
        var bytes = MakeRecord("ARMA",
            ("EDID", Str("TestArma")),
            ("DNAM", new byte[12]),
            ("MOD3", Str("newfemale.nif")));

        arma.CopyInFromBinary(Frame(bytes));

        arma.WorldModel.ShouldNotBeNull();
        arma.WorldModel.Male.ShouldBeNull();
        arma.WorldModel.Female.ShouldNotBeNull();
        arma.WorldModel.Female.File.GivenPath.ShouldBe("newfemale.nif");
        arma.FirstPersonModel.ShouldNotBeNull();
        arma.FirstPersonModel.Male.ShouldNotBeNull();
        arma.FirstPersonModel.Male.File.GivenPath.ShouldBe("male1st.nif");
        arma.FirstPersonModel.Female.ShouldNotBeNull();
        arma.FirstPersonModel.Female.File.GivenPath.ShouldBe("female1st.nif");
        SubrecordTypes(Constants, Write(arma)).ShouldNotContain("MOD2");
    }

    [Fact]
    public void ArmorAddon_CopyInFromBinary_KeepsBothHalvesOfSplitPair()
    {
        var arma = ReadArmaDirect(ArmaAdjacent());

        arma.CopyInFromBinary(Frame(ArmaInterleaved()));

        AssertAllFourModels(arma);
    }
}
