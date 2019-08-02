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
            var mod = await OblivionMod.CreateFromBinary(
                Path.Combine(settings.DataFolderLocations.Oblivion, target.Path),
                modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                importMask: new GroupMask()
                {
                    NPCs = true
                });

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Import"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
                mod.WriteToBinary(
                    oblivionOutputPath,
                    modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    errorMask: out var outputErrMask);
                Assert.False(outputErrMask?.IsInError() ?? false);
                var fileLocs = RecordLocator.GetFileLocations(oblivionOutputPath, meta: MetaDataConstants.Get(GameMode.Oblivion));
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

        public static async Task OblivionESM_GroupMask_Export(TestingSettings settings, Target target)
        {
            var mod = await OblivionMod.CreateFromBinary(
                Path.Combine(settings.DataFolderLocations.Oblivion, target.Path),
                modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion);

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Export"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, TestingConstants.OBLIVION_ESM);
                mod.WriteToBinary(
                    oblivionOutputPath,
                    modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                    errorMask: out var outputErrMask,
                    importMask: new GroupMask()
                    {
                        NPCs = true
                    });
                Assert.False(outputErrMask?.IsInError() ?? false);
                var fileLocs = RecordLocator.GetFileLocations(oblivionOutputPath, GameMode.Oblivion);
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

        public static async Task OblivionESM_Folder_Reimport(PassthroughSettings settings, Target target, OblivionPassthroughTest oblivPassthrough)
        {
            Task<OblivionMod> ImportModBinary(string sourcePath)
            {
                return OblivionMod.CreateFromBinary(
                    sourcePath,
                    modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion);
            }

            async Task WriteMod(OblivionMod mod, DirectoryPath dir)
            {
                var exportMask = await mod.WriteToXmlFolder(dir);
                Assert.False(exportMask?.IsInError() ?? false);
            }

            async Task CreateFromXmlFolder(string sourcePath, DirectoryPath dir)
            {
                var mod = await ImportModBinary(sourcePath);
                await WriteMod(mod, dir);
            }

            using (var processedTmp = await oblivPassthrough.SetupProcessedFiles())
            {
                using (var tmp = new TempFolder("Mutagen_Oblivion_XmlFolder", deleteAfter: false))
                {
                    var sourcePath = oblivPassthrough.ProcessedPath(processedTmp);
                    await CreateFromXmlFolder(sourcePath, tmp.Dir);
                    GC.Collect();
                    var reimport = await OblivionMod.CreateFromXmlFolder(
                        dir: tmp.Dir,
                        modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion);
                    GC.Collect();
                    var reexportPath = Path.Combine(tmp.Dir.Path, "Reexport");
                    reimport.WriteToBinary(
                        reexportPath,
                        modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion);
                    using (var stream = new BinaryReadStream(sourcePath))
                    {
                        var eqException = PassthroughTest.AssertFilesEqual(
                            stream,
                            reexportPath,
                            amountToReport: 15);
                        if (eqException.Exception != null)
                        {
                            throw eqException.Exception;
                        }
                    }
                }
            }
        }

        public static async Task BaseGroupIterator(Target settings, DataFolderLocations locs)
        {
            if (!settings.ExpectedBaseGroupCount_IsSet) return;
            var loc = settings.GetFilePath(locs);
            using (var stream = new MutagenBinaryReadStream(loc.Path, settings.GameMode))
            {
                var grups = RecordLocator.IterateBaseGroupLocations(stream).ToArray();
                Assert.Equal(settings.ExpectedBaseGroupCount, grups.Length);
            }
        }
    }
}
