using System;
using FluentAssertions;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Skyrim.Records
{
    public abstract class ABodyTemplateTests
    {
        protected abstract IRaceGetter Get(ModPath modPath);

        [Fact]
        public void BodtLength8With42()
        {
            var race = Get(TestDataPathing.SkyrimBodtLength8With42);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be((BodyTemplate.Flag)0x2);
            race.BodyTemplate!.ArmorType.Should().Be(default);
        }

        [Fact]
        public void BodtLength12With42()
        {
            var race = Get(TestDataPathing.SkyrimBodtLength12With42);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be(BodyTemplate.Flag.NonPlayable);
            race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
        }
        
        [Fact]
        public void BodtLength8With43()
        {
            var race = Get(TestDataPathing.SkyrimBodtLength8With43);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be((BodyTemplate.Flag)0x2);
            race.BodyTemplate!.ArmorType.Should().Be(default);
        }

        [Fact]
        public void BodtLength12With43Normal()
        {
            var race = Get(TestDataPathing.SkyrimBodtLength12With43);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be(BodyTemplate.Flag.NonPlayable);
            race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
        }

        [Fact]
        public void BodtLength12With44()
        {
            Assert.Throws<SubrecordException>(() =>
            {
                var race = Get(TestDataPathing.SkyrimBodtLength12With44);
                race.BodyTemplate.Should().NotBeNull();
                race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
                race.BodyTemplate!.Flags.Should().Be(BodyTemplate.Flag.NonPlayable);
                race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
            });
        }

        [Fact]
        public void BodtLength8With44()
        {
            Assert.Throws<SubrecordException>(() =>
            {
                var race = Get(TestDataPathing.SkyrimBodtLength8With44);
                race.BodyTemplate.Should().NotBeNull();
                race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
                race.BodyTemplate!.Flags.Should().Be(BodyTemplate.Flag.NonPlayable);
                race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
            });
        }

        [Fact]
        public void Bod2Length8With42()
        {
            Assert.Throws<SubrecordException>(() =>
            {
                var race = Get(TestDataPathing.SkyrimBod2Length8With42);
                race.BodyTemplate.Should().NotBeNull();
                race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
                race.BodyTemplate!.Flags.Should().Be(BodyTemplate.Flag.NonPlayable);
                race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
            });
        }

        [Fact]
        public void Bod2Length12With42()
        {
            Assert.Throws<SubrecordException>(() =>
            {
                var race = Get(TestDataPathing.SkyrimBod2Length12With42);
                race.BodyTemplate.Should().NotBeNull();
                race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Feet);
                race.BodyTemplate!.Flags.Should().Be(BodyTemplate.Flag.NonPlayable);
                race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
            });
        }

        [Fact]
        public void Bod2Length12With43()
        {
            var race = Get(TestDataPathing.SkyrimBod2Length12With43);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be(default);
            race.BodyTemplate!.ArmorType.Should().Be((ArmorType)0x10);
        }

        [Fact]
        public void Bod2Length8With43()
        {
            var race = Get(TestDataPathing.SkyrimBod2Length8With43);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be(default);
            race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
        }

        [Fact]
        public void Bod2Length8With44Normal()
        {
            var race = Get(TestDataPathing.SkyrimBod2Length8With44);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be(default);
            race.BodyTemplate!.ArmorType.Should().Be(ArmorType.Clothing);
        }

        [Fact]
        public void Bod2Length12With44()
        {
            var race = Get(TestDataPathing.SkyrimBod2Length12With44);
            race.BodyTemplate.Should().NotBeNull();
            race.BodyTemplate!.FirstPersonFlags.Should().Be(BipedObjectFlag.Tail);
            race.BodyTemplate!.Flags.Should().Be(default);
            race.BodyTemplate!.ArmorType.Should().Be((ArmorType)0x10);
        }
    }

    public class DirectBodyTemplateTests : ABodyTemplateTests
    {
        protected override IRaceGetter Get(ModPath modPath)
        {
            return Race.CreateFromBinary(TestDataPathing.GetReadFrame(modPath.Path, GameRelease.SkyrimSE));
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
}