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
    public class OblivionESM_Passthrough_Tests : Oblivion_Passthrough_Test
    {
        public override string Nickname => TestingConstants.OBLIVION_ESM;
        public override ModKey ModKey => ModKey.Factory(TestingConstants.OBLIVION_ESM);

        public OblivionESM_Passthrough_Tests(TestingSettings settings)
            : base(
                  numMasters: 0,
                  path: Path.Combine(settings.DataFolder, settings.OblivionESM.Path))
        {
        }

        public async Task OblivionESM_GroupMask_Import()
        {
            var mod = OblivionMod.Create_Binary(
                this.FilePath,
                modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                errorMask: out var inputErrMask,
                importMask: new GroupMask()
                {
                    NPCs = true
                });
            Assert.False(inputErrMask?.IsInError() ?? false);

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Import"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
                mod.Write_Binary(
                    oblivionOutputPath,
                    modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    errorMask: out var outputErrMask);
                Assert.False(outputErrMask?.IsInError() ?? false);
                var fileLocs = RecordLocator.GetFileLocations(oblivionOutputPath);
                using (var reader = new BinaryReadStream(oblivionOutputPath))
                {
                    foreach (var rec in fileLocs.ListedRecords.Keys)
                    {
                        reader.Position = rec;
                        var t = HeaderTranslation.ReadNextRecordType(reader);
                        if (!t.Equals(NPC_Registration.NPC__HEADER))
                        {
                            throw new ArgumentException("Exported a non-NPC record.");
                        }
                    }
                }
            }
        }

        public async Task OblivionESM_GroupMask_Export()
        {
            var mod = OblivionMod.Create_Binary(
                this.FilePath,
                modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                errorMask: out var inputErrMask);
            Assert.False(inputErrMask?.IsInError() ?? false);

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Export"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
                mod.Write_Binary(
                    oblivionOutputPath,
                    modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    errorMask: out var outputErrMask,
                    importMask: new GroupMask()
                    {
                        NPCs = true
                    });
                Assert.False(outputErrMask?.IsInError() ?? false);
                var fileLocs = RecordLocator.GetFileLocations(oblivionOutputPath);
                using (var reader = new BinaryReadStream(oblivionOutputPath))
                {
                    foreach (var rec in fileLocs.ListedRecords.Keys)
                    {
                        reader.Position = rec;
                        var t = HeaderTranslation.ReadNextRecordType(reader);
                        if (!t.Equals(NPC_Registration.NPC__HEADER))
                        {
                            throw new ArgumentException("Exported a non-NPC record.");
                        }
                    }
                }
            }
        }

        public async Task OblivionESM_Folder_Reimport()
        {
            using (var tmp = new TempFolder("Mutagen_Oblivion_XmlFolder", deleteAfter: false))
            {
                var mod = OblivionMod.Create_Binary(
                    this.FilePath,
                    modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    errorMask: out var inputErrMask);
                var exportMask = await mod.Write_XmlFolder(
                    tmp.Dir);
                Assert.False(exportMask?.IsInError() ?? false);
                var reimport = await OblivionMod.Create_Xml_Folder(
                    tmp.Dir);
                Assert.False(inputErrMask?.IsInError() ?? false);
                var eqMask = reimport.Mod.GetEqualsMask(mod);
                Assert.True(eqMask.AllEqual(b => b));
            }
        }
    }
}
