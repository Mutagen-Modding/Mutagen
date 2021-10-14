using FluentAssertions;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.Tests
{
    public class OtherTests
    {
        public static async Task OblivionESM_GroupMask_Import(TestingSettings settings, Target target)
        {
            var mod = OblivionMod.CreateFromBinary(
                new ModPath(
                    Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    Path.Combine(settings.DataFolderLocations.Oblivion, target.Path)),
                importMask: new Mutagen.Bethesda.Oblivion.GroupMask()
                {
                    Npcs = true
                });

            using var tmp = TempFolder.FactoryByAddedPath("Mutagen_Oblivion_Binary_GroupMask_Import");
            var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestConstants.Oblivion.FileName);
            mod.WriteToBinary(oblivionOutputPath);
            var fileLocs = RecordLocator.GetLocations(oblivionOutputPath, constants: GameConstants.Get(GameRelease.Oblivion));
            using var reader = new BinaryReadStream(oblivionOutputPath);
            foreach (var rec in fileLocs.ListedRecords.Keys)
            {
                reader.Position = rec;
                var t = HeaderTranslation.ReadNextRecordType(reader);
                if (!t.Equals(Oblivion.Internals.RecordTypes.NPC_))
                {
                    throw new ArgumentException("Exported a non-NPC record.");
                }
            }
        }

        public static async Task OblivionESM_GroupMask_Export(TestingSettings settings, Target target)
        {
            var mod = OblivionMod.CreateFromBinary(
                new ModPath(
                    Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    Path.Combine(settings.DataFolderLocations.Oblivion, target.Path)));

            using var tmp = TempFolder.FactoryByAddedPath("Mutagen_Oblivion_Binary_GroupMask_Export");
            var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestConstants.Oblivion.FileName);
            mod.WriteToBinary(
                oblivionOutputPath,
                importMask: new GroupMask()
                {
                    Npcs = true
                });
            var fileLocs = RecordLocator.GetLocations(oblivionOutputPath, GameRelease.Oblivion);
            using var reader = new BinaryReadStream(oblivionOutputPath);
            foreach (var rec in fileLocs.ListedRecords.Keys)
            {
                reader.Position = rec;
                var t = HeaderTranslation.ReadNextRecordType(reader);
                if (!t.Equals(Oblivion.Internals.RecordTypes.NPC_))
                {
                    throw new ArgumentException("Exported a non-NPC record.");
                }
            }
        }
        
        //public static async Task BaseGroupIterator(Target settings, DataFolderLocations locs)
        //{
        //    if (!settings.ExpectedBaseGroupCount.TryGet(out var expected)) return;
        //    var loc = settings.GetFilePath(locs);
        //    using var stream = new MutagenBinaryReadStream(loc.Path, settings.GameRelease);
        //    var grups = RecordLocator.IterateBaseGroupLocations(stream).ToArray();
        //    Assert.Equal(expected, grups.Length);
        //}

        public static async Task RecordEnumerations(TestingSettings settings, Target target)
        {
            var mod = OblivionMod.CreateFromBinaryOverlay(
                Path.Combine(settings.DataFolderLocations.Oblivion, target.Path));
            var set1 = new HashSet<FormKey>(mod.EnumerateMajorRecords().Select(m => m.FormKey));
            var set2 = new HashSet<FormKey>(mod.EnumerateMajorRecords<IMajorRecordCommonGetter>().Select(m => m.FormKey));
            set1.Count.Should().Equals(set2.Count);
            set1.Should().Equals(set2);
        }
    }
}
