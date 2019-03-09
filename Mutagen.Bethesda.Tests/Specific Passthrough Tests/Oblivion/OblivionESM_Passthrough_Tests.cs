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
    public class OblivionESM_Passthrough_Tests
    {
        public static async Task OblivionESM_GroupMask_Import(PassthroughSettings settings, Passthrough passthrough)
        {
            var mod = OblivionMod.Create_Binary(
                Path.Combine(settings.DataFolder, passthrough.Path),
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

        public static async Task OblivionESM_GroupMask_Export(PassthroughSettings settings, Passthrough passthrough)
        {
            var mod = OblivionMod.Create_Binary(
                Path.Combine(settings.DataFolder, passthrough.Path),
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

        public static async Task OblivionESM_Folder_Reimport(PassthroughSettings settings, Passthrough passthrough, Oblivion_Passthrough_Test oblivPassthrough)
        {
            using (var processedTmp = await oblivPassthrough.SetupProcessedFiles())
            {
                using (var tmp = new TempFolder("Mutagen_Oblivion_XmlFolder", deleteAfter: false))
                {
                    var sourcePath = oblivPassthrough.ProcessedPath(processedTmp);
                    var mod = OblivionMod.Create_Binary(
                        sourcePath,
                        modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                        errorMask: out var inputErrMask);
                    Assert.False(inputErrMask?.IsInError() ?? false);
                    var exportMask = await mod.Write_XmlFolder(
                        tmp.Dir);
                    Assert.False(exportMask?.IsInError() ?? false);
                    var reimport = await OblivionMod.Create_Xml_Folder(
                        dir: tmp.Dir,
                        modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion);
                    var eqMask = reimport.Mod.GetEqualsMask(mod, include: Loqui.EqualsMaskHelper.Include.OnlyFailures);
                    Assert.True(eqMask.AllEqual(b => b));
                    var reexportPath = Path.Combine(tmp.Dir.Path, "Reexport");
                    reimport.Mod.Write_Binary(
                        reexportPath,
                        modKey: Mutagen.Bethesda.Oblivion.Constants.Oblivion);
                    using (var stream = new BinaryReadStream(sourcePath))
                    {
                        var eqException = Passthrough_Tests.AssertFilesEqual(
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
    }
}
