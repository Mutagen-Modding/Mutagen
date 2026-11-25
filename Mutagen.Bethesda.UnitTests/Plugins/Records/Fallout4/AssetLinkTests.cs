using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class AssetLinkTests
{
    private static readonly IAssetType Model = Fallout4ModelAssetType.Instance;
    private static readonly IAssetType DeformedModel = Fallout4DeformedModelAssetType.Instance;
    private static readonly IAssetType Behavior = Fallout4BehaviorAssetType.Instance;
    private static readonly IAssetType Texture = Fallout4TextureAssetType.Instance;
    private static readonly IAssetType Material = Fallout4MaterialAssetType.Instance;
    private static readonly IAssetType Sound = Fallout4SoundAssetType.Instance;
    private static readonly IAssetType Music = Fallout4MusicAssetType.Instance;
    private static readonly IAssetType Interface = Fallout4InterfaceAssetType.Instance;
    private static readonly IAssetType Program = Fallout4ProgramAssetType.Instance;

    private const int CloudLayerSeventeen = 17;

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void EachPathARecordNamesIsAnAssetLinkOfItsType(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        Fallout4Mod mod)
    {
        var expected = AddRecordsNamingAssets(mod);
        var path = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        using var overlay = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();
        ListedAssetLinks(overlay).ShouldBe(expected, ignoreOrder: true);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(rewritePath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        ListedAssetLinks(rewritten).ShouldBe(expected, ignoreOrder: true);
    }

    private static List<(string? EditorID, IAssetType AssetType, string Path)> ListedAssetLinks(IFallout4ModGetter mod) =>
        mod.EnumerateMajorRecords()
            .SelectMany(r => r.EnumerateAssetLinks(AssetLinkQuery.Listed)
                .Select(l => (r.EditorID, l.AssetTypeInstance, l.DataRelativePath.Path)))
            .ToList();

    private static List<(string? EditorID, IAssetType AssetType, string Path)> AddRecordsNamingAssets(Fallout4Mod mod)
    {
        var expected = new List<(string? EditorID, IAssetType AssetType, string Path)>();

        void Expect(IMajorRecordGetter record, params (IAssetType AssetType, string File)[] links) =>
            expected.AddRange(links.Select(l => (record.EditorID, l.AssetType, Path.Combine(l.AssetType.BaseFolder, l.File))));

        var ammo = mod.Ammunitions.AddNew("AMMO");
        ammo.CasingModel = "casing.nif";
        Expect(ammo, (Model, "casing.nif"));

        var arma = mod.ArmorAddons.AddNew("ARMA");
        arma.WorldModel = new GenderedItem<Model?>(new Model { File = "male.nif" }, new Model { File = "female.nif" });
        arma.FirstPersonModel = new GenderedItem<Model?>(new Model { File = "male1st.nif" }, new Model { File = "female1st.nif" });
        Expect(arma,
            (Model, "male.nif"),
            (Model, "female.nif"),
            (Model, "male1st.nif"),
            (Model, "female1st.nif"));

        var bptd = mod.BodyParts.AddNew("BPTD");
        bptd.Parts.Add(new BodyPart
        {
            PartNode = "node",
            VatsTarget = "target",
            LimbReplacementModel = "limb.nif",
        });
        Expect(bptd, (Model, "limb.nif"));

        var cell = new Cell(mod)
        {
            EditorID = "CELL",
            WaterEnvironmentMap = "cellenvironment.dds",
        };
        mod.Cells.Records.Add(new CellBlock
        {
            GroupType = GroupTypeEnum.InteriorCellBlock,
            SubBlocks =
            {
                new CellSubBlock
                {
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    Cells = { cell },
                },
            },
        });
        Expect(cell, (Texture, "cellenvironment.dds"));

        var clas = mod.Classes.AddNew("CLAS");
        clas.Icon = "class.dds";
        Expect(clas, (Texture, "class.dds"));

        var clmt = mod.Climates.AddNew("CLMT");
        clmt.SunTexture = "sun.dds";
        clmt.SunGlareTexture = "sunglare.dds";
        Expect(clmt, (Texture, "sun.dds"), (Texture, "sunglare.dds"));

        var debr = mod.Debris.AddNew("DEBR");
        debr.Models.Add(new DebrisModel { ModelFilename = "debris.nif" });
        Expect(debr, (Model, "debris.nif"));

        var efsh = mod.EffectShaders.AddNew("EFSH");
        efsh.FillTexture = "fill.dds";
        efsh.ParticleShaderTexture = "particleshader.dds";
        efsh.HolesTexture = "holes.dds";
        efsh.MembranePaletteTexture = "membranepalette.dds";
        efsh.ParticlePaletteTexture = "particlepalette.dds";
        Expect(efsh,
            (Texture, "fill.dds"),
            (Texture, "particleshader.dds"),
            (Texture, "holes.dds"),
            (Texture, "membranepalette.dds"),
            (Texture, "particlepalette.dds"));

        var fact = mod.Factions.AddNew("FACT");
        fact.Ranks.Add(new Rank { Number = 0, Insignia = "insignia.dds" });
        Expect(fact, (Texture, "insignia.dds"));

        var furn = mod.Furniture.AddNew("FURN");
        furn.MarkerModel = "furnituremarker.nif";
        Expect(furn, (Model, "furnituremarker.nif"));

        var hdpt = mod.HeadParts.AddNew("HDPT");
        hdpt.Parts.Add(new Part { PartType = Part.PartTypeEnum.Tri, FileName = "part.tri" });
        Expect(hdpt, (DeformedModel, "part.tri"));

        var idle = mod.IdleAnimations.AddNew("IDLE");
        idle.BehaviorGraph = "behavior.hkx";
        Expect(idle, (Behavior, "behavior.hkx"));

        var imgs = mod.ImageSpaces.AddNew("IMGS");
        imgs.Lut = "lut.dds";
        Expect(imgs, (Texture, "lut.dds"));

        var lens = mod.LensFlares.AddNew("LENS");
        lens.Sprites = [new LensFlareSprite { LensFlareSpriteId = "sprite", Texture = "sprite.dds" }];
        Expect(lens, (Texture, "sprite.dds"));

        var ligh = mod.Lights.AddNew("LIGH");
        ligh.Gobo = "gobo.dds";
        Expect(ligh, (Texture, "gobo.dds"));

        var lscr = mod.LoadScreens.AddNew("LSCR");
        lscr.CameraPath = "camera.nif";
        Expect(lscr, (Model, "camera.nif"));

        var matt = mod.MaterialTypes.AddNew("MATT");
        matt.BreakableFX = "breakable.nif";
        Expect(matt, (Model, "breakable.nif"));

        var mesg = mod.Messages.AddNew("MESG");
        mesg.Swf = "message.swf";
        Expect(mesg, (Interface, "message.swf"));

        var mswp = mod.MaterialSwaps.AddNew("MSWP");
        mswp.Substitutions.Add(new MaterialSubstitution
        {
            OriginalMaterial = "original.bgsm",
            ReplacementMaterial = "replacement.bgsm",
        });
        Expect(mswp, (Material, "original.bgsm"), (Material, "replacement.bgsm"));

        var must = mod.MusicTracks.AddNew("MUST");
        must.TrackFilename = "track.wav";
        must.FinaleFilename = "finale.wav";
        Expect(must, (Music, "track.wav"), (Music, "finale.wav"));

        var nocm = mod.NavigationMeshObstacleManagers.AddNew("NOCM");
        nocm.SubObjects.Add(new NavigationMeshObstacleManagerSubObject { Index = 0, Model = "obstacle.nif" });
        Expect(nocm, (Model, "obstacle.nif"));

        var note = mod.Holotapes.AddNew("NOTE");
        note.Data = new HolotapeProgram { File = "program.swf" };
        Expect(note, (Program, "program.swf"));

        var perk = mod.Perks.AddNew("PERK");
        perk.Icon = "perk.dds";
        perk.Swf = "perk.swf";
        Expect(perk, (Texture, "perk.dds"), (Interface, "perk.swf"));

        var proj = mod.Projectiles.AddNew("PROJ");
        proj.MuzzleFlashModel = "muzzleflash.nif";
        Expect(proj, (Model, "muzzleflash.nif"));

        var qust = mod.Quests.AddNew("QUST");
        qust.SwfFile = "quest.swf";
        Expect(qust, (Interface, "quest.swf"));

        var race = mod.Races.AddNew("RACE");
        race.SkeletalModel = new GenderedItem<SimpleModel?>(
            new SimpleModel { File = "maleskeleton.nif" },
            new SimpleModel { File = "femaleskeleton.nif" });
        race.HeadData = new GenderedItem<HeadData?>(
            new HeadData
            {
                TintLayers =
                {
                    new TintGroup
                    {
                        Name = "group",
                        Options =
                        {
                            new TintTemplateOption
                            {
                                Name = "option",
                                Textures = { new AssetLink<Fallout4TextureAssetType>("tint.dds") },
                            },
                        },
                    },
                },
            },
            null);
        race.Subgraphs.Add(new Subgraph { BehaviorGraph = "subgraph.hkx" });
        race.HairColorLookupTexture = "haircolor.dds";
        race.HairColorExtendedLookupTexture = "haircolorextended.dds";
        Expect(race,
            (Model, "maleskeleton.nif"),
            (Model, "femaleskeleton.nif"),
            (Texture, "tint.dds"),
            (Behavior, "subgraph.hkx"),
            (Texture, "haircolor.dds"),
            (Texture, "haircolorextended.dds"));

        var sndr = mod.SoundDescriptors.AddNew("SNDR");
        sndr.Data = new SoundDescriptorAutoweaponData();
        sndr.SoundFiles.Add(new AssetLink<Fallout4SoundAssetType>("first.wav"));
        sndr.SoundFiles.Add(new AssetLink<Fallout4SoundAssetType>("second.wav"));
        sndr.RatesOfFire = [new SoundRateOfFire { File = "rateoffire.wav" }];
        Expect(sndr, (Sound, "first.wav"), (Sound, "second.wav"), (Sound, "rateoffire.wav"));

        var spgd = mod.ShaderParticleGeometries.AddNew("SPGD");
        spgd.ParticleTexture = "particle.bgem";
        Expect(spgd, (Material, "particle.bgem"));

        var stat = mod.Statics.AddNew("STAT");
        stat.Model = new Model { File = "static.nif" };
        stat.DistantLods.Add(new DistantLod { Mesh = "lod0.nif" });
        stat.DistantLods.Add(new DistantLod { Mesh = "lod1.nif" });
        stat.DistantLods.Add(new DistantLod { Mesh = "lod2.nif" });
        stat.DistantLods.Add(new DistantLod { Mesh = "lod3.nif" });
        Expect(stat,
            (Model, "static.nif"),
            (Model, "lod0.nif"),
            (Model, "lod1.nif"),
            (Model, "lod2.nif"),
            (Model, "lod3.nif"));

        var term = mod.Terminals.AddNew("TERM");
        term.MarkerModel = "terminalmarker.nif";
        term.MenuItems =
        [
            new TerminalMenuItem
            {
                ItemText = "item",
                Type = TerminalMenuItem.Types.DisplayImage,
                ImageFile = "image.dds",
            },
        ];
        Expect(term, (Model, "terminalmarker.nif"), (Texture, "image.dds"));

        var txst = mod.TextureSets.AddNew("TXST");
        txst.Diffuse = "diffuse.dds";
        txst.NormalOrGloss = "normal.dds";
        txst.Wrinkles = "wrinkles.dds";
        txst.Glow = "glow.dds";
        txst.Height = "height.dds";
        txst.Environment = "environment.dds";
        txst.Multilayer = "multilayer.dds";
        txst.SmoothSpec = "smoothspec.dds";
        txst.Material = "decal.bgsm";
        Expect(txst,
            (Texture, "diffuse.dds"),
            (Texture, "normal.dds"),
            (Texture, "wrinkles.dds"),
            (Texture, "glow.dds"),
            (Texture, "height.dds"),
            (Texture, "environment.dds"),
            (Texture, "multilayer.dds"),
            (Texture, "smoothspec.dds"),
            (Material, "decal.bgsm"));

        var watr = mod.Waters.AddNew("WATR");
        watr.NoiseLayerOne.Texture = "noise1.dds";
        watr.NoiseLayerTwo.Texture = "noise2.dds";
        watr.NoiseLayerThree.Texture = "noise3.dds";
        Expect(watr, (Texture, "noise1.dds"), (Texture, "noise2.dds"), (Texture, "noise3.dds"));

        var weap = mod.Weapons.AddNew("WEAP");
        weap.Model = new Model { File = "weapon.nif" };
        weap.Icons = new Icons { InventoryImage = "inventory.dds", MessageIcon = "message.dds" };
        weap.FirstPersonModel = new Model { File = "weapon1st.nif" };
        Expect(weap,
            (Model, "weapon.nif"),
            (Texture, "inventory.dds"),
            (Texture, "message.dds"),
            (Model, "weapon1st.nif"));

        var wrld = mod.Worldspaces.AddNew("WRLD");
        wrld.MapImage = "map.dds";
        wrld.CanopyShadow = "canopyshadow.dds";
        wrld.WaterEnvironmentMap = "worldenvironment.dds";
        wrld.HdLodDiffuseTexture = "hdloddiffuse.dds";
        wrld.HdLodNormalTexture = "hdlodnormal.dds";
        Expect(wrld,
            (Texture, "map.dds"),
            (Texture, "canopyshadow.dds"),
            (Texture, "worldenvironment.dds"),
            (Texture, "hdloddiffuse.dds"),
            (Texture, "hdlodnormal.dds"));

        var wthr = mod.Weather.AddNew("WTHR");
        wthr.CloudTextures[0] = new AssetLink<Fallout4TextureAssetType>("cloud0.dds");
        wthr.CloudTextures[CloudLayerSeventeen] = new AssetLink<Fallout4TextureAssetType>("cloud17.dds");
        Expect(wthr, (Texture, "cloud0.dds"), (Texture, "cloud17.dds"));

        return expected;
    }
}
