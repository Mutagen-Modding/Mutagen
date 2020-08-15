using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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

            using var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Import");
            var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
            mod.WriteToBinary(oblivionOutputPath);
            var fileLocs = RecordLocator.GetFileLocations(oblivionOutputPath, constants: GameConstants.Get(GameRelease.Oblivion));
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

            using var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Export");
            var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
            mod.WriteToBinary(
                oblivionOutputPath,
                importMask: new GroupMask()
                {
                    Npcs = true
                });
            var fileLocs = RecordLocator.GetFileLocations(oblivionOutputPath, GameRelease.Oblivion);
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
            Assert.Equal(set1.Count, set2.Count);
            Assert.Equal(set1, set2);
        }
    }
}
