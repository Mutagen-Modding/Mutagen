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
using static Mutagen.Bethesda.ModAligner;

namespace Mutagen.Bethesda.Tests
{
    public class OblivionESM_Passthrough_Tests : Oblivion_Passthrough_Test
    {
        public override string Nickname => TestingConstants.OBLIVION_ESM;

        public OblivionESM_Passthrough_Tests(string path = null)
            : base(path ?? Properties.Settings.Default.OblivionESM)
        {
        }

        protected override BinaryFileProcessor.Config GetInstructions(
            Dictionary<long, uint> lengthTracker,
            MajorRecordLocator.FileLocations fileLocs)
        {
            var instructions = base.GetInstructions(lengthTracker, fileLocs);
            instructions.SetSubstitution(
                loc: 0xC46695,
                sub: new byte[] { 0x66, 0xDC, 0x05, 0x00 });
            instructions.SetSubstitution(
                loc: 0xCA88D9,
                sub: new byte[] { 0xDB, 0xBC, 0x04, 0x00 });
            instructions.SetSubstitution(
                loc: 0xCEAEB5,
                sub: new byte[] { 0x76, 0x0A, 0x00, 0x00 });
            return instructions;
        }

        public async Task OblivionESM_GroupMask_Import()
        {
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask,
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
                    out var outputErrMask);
                Assert.False(outputErrMask?.IsInError() ?? false);
                var fileLocs = MajorRecordLocator.GetFileLocations(oblivionOutputPath);
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
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask);
            Assert.False(inputErrMask?.IsInError() ?? false);

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Export"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
                mod.Write_Binary(
                    oblivionOutputPath,
                    out var outputErrMask,
                    importMask: new GroupMask()
                    {
                        NPCs = true
                    });
                Assert.False(outputErrMask?.IsInError() ?? false);
                var fileLocs = MajorRecordLocator.GetFileLocations(oblivionOutputPath);
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
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask);
            Assert.False(inputErrMask?.IsInError() ?? false);
            using (var tmp = new TempFolder("Mutagen_Oblivion_XmlFolder", deleteAfter: false))
            {
                mod[FormID.Factory("0006371E")].Write_XML(Path.Combine(tmp.Dir.Path, "Test"));
                var exportMask = await mod.Write_XmlFolder(
                    tmp.Dir);
                Assert.False(exportMask?.IsInError() ?? false);
            }
        }
    }
}
