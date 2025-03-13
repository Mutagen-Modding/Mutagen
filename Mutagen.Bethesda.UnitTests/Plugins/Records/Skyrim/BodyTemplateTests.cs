﻿using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.IO;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public abstract class ABodyTemplateTests
{
    protected abstract IRaceGetter Get(ModPath modPath);

    private void AssertBinaryEquality(IRaceGetter race, string path)
    {
        var fs = new MockFileSystem();
        using (var writer = new MutagenWriter(fs.FileStream.New($"{PathingUtil.DrivePrefix}output", FileMode.Create),
                   new WritingBundle(GameConstants.SkyrimSE)
                   {
                       MasterReferences = new MasterReferenceCollection("Skyrim.esm"),
                       FormVersion = race.FormVersion
                   }))
        {
            BodyTemplateBinaryWriteTranslation.Write(writer, race.BodyTemplate!);
        }
        var exported = fs.File.ReadAllBytes($"{PathingUtil.DrivePrefix}output");
        var expected = File.ReadAllBytes(path);
        exported.ShouldBe(expected);
    }

    [Fact]
    public void BodtLength8With42()
    {
        var race = Get(TestDataPathing.SkyrimBodtLength8With42);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(default);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBodtEmptyFlagsOutput);
    }

    [Fact]
    public void BodtLength12With42()
    {
        var race = Get(TestDataPathing.SkyrimBodtLength12With42);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBodtTypicalOutput);
    }

    [Fact]
    public void BodtLength8With43()
    {
        var race = Get(TestDataPathing.SkyrimBodtLength8With43);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(default);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBodtEmptyFlagsOutput);
    }

    [Fact]
    public void BodtLength12With43Normal()
    {
        var race = Get(TestDataPathing.SkyrimBodtLength12With43);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBodtTypicalOutput);
    }

    [Fact]
    public void BodtLength12With44()
    {
        var race = Get(TestDataPathing.SkyrimBodtLength12With44);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }

    [Fact]
    public void BodtLength8With44()
    {
        Assert.Throws<SubrecordException>(() =>
        {
            var race = Get(TestDataPathing.SkyrimBodtLength8With44);
            race.BodyTemplate.ShouldNotBeNull();
            race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
            race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
            AssertBinaryEquality(race, TestDataPathing.SkyrimBodtEmptyFlagsOutput);
        });
    }

    [Fact]
    public void Bod2Length8With42()
    {
        var race = Get(TestDataPathing.SkyrimBod2Length8With42);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(default);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }

    [Fact]
    public void Bod2Length12With42()
    {
        var race = Get(TestDataPathing.SkyrimBod2Length12With42);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }

    [Fact]
    public void Bod2Length12With43()
    {
        var race = Get(TestDataPathing.SkyrimBod2Length12With43);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }

    [Fact]
    public void Bod2Length8With43()
    {
        var race = Get(TestDataPathing.SkyrimBod2Length8With43);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(default);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }

    [Fact]
    public void Bod2Length8With44Normal()
    {
        var race = Get(TestDataPathing.SkyrimBod2Length8With44);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(default);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }

    [Fact]
    public void Bod2Length12With44()
    {
        var race = Get(TestDataPathing.SkyrimBod2Length12With44);
        race.BodyTemplate.ShouldNotBeNull();
        race.BodyTemplate!.FirstPersonFlags.ShouldBe(BipedObjectFlag.Tail);
        race.BodyTemplate!.Flags.ShouldBe(BodyTemplate.Flag.NonPlayable);
        race.BodyTemplate!.ArmorType.ShouldBe(ArmorType.Clothing);
        AssertBinaryEquality(race, TestDataPathing.SkyrimBod2TypicalOutput);
    }
}

public class DirectBodyTemplateTests : ABodyTemplateTests
{
    protected override IRaceGetter Get(ModPath modPath)
    {
        return Race.CreateFromBinary(TestDataPathing.GetReadFrame(modPath.Path, GameRelease.SkyrimSE,
            "Skyrim.esm"));
    }
}

public class OverlayBodyTemplateTests : ABodyTemplateTests
{
    protected override IRaceGetter Get(ModPath modPath)
    {
        var overlayStream = TestDataPathing.GetOverlayStream(modPath.Path, GameRelease.SkyrimSE);
        return RaceBinaryOverlay.RaceFactory(
            overlayStream,
            new BinaryOverlayFactoryPackage(overlayStream.MetaData));
    }
}