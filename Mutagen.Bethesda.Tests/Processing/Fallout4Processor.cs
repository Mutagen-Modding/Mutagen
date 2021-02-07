using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Fallout4.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public class Fallout4Processor : Processor
    {
        public Fallout4Processor(bool multithread) : base(multithread)
        {
        }

        public override GameRelease GameRelease => GameRelease.Fallout4;

        protected override void AddDynamicProcessorInstructions()
        {
            base.AddDynamicProcessorInstructions();
            AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
            AddDynamicProcessing(RecordTypes.TRNS, ProcessTransforms);
        }

        protected override IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
        {
            foreach (var t in base.ExtraJobs(streamGetter))
            {
                yield return t;
            }
            foreach (var source in EnumExt.GetValues<StringsSource>())
            {
                yield return TaskExt.Run(DoMultithreading, () =>
                {
                    return ProcessStringsFilesIndices(streamGetter, new DirectoryInfo(Path.GetDirectoryName(this.SourcePath)), Language.English, source, ModKey.FromNameAndExtension(Path.GetFileName(this.SourcePath)));
                });
            }
        }

        private void ProcessGameSettings(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (!majorFrame.TryLocateSubrecordFrame("EDID", out var edidFrame)) return;
            if ((char)edidFrame.Content[0] != 'f') return;

            if (!majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec)) return;
            ProcessZeroFloat(dataRec, fileOffset);
        }

        private void ProcessTransforms(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (!majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec)) return;
            int offset = 0;
            ProcessZeroFloats(dataRec, fileOffset, ref offset, 9);
        }

        public void GameSettingStringHandler(
            IMutagenReadStream stream,
            MajorRecordHeader major,
            BinaryFileProcessor.ConfigConstructor instr,
            List<KeyValuePair<uint, uint>> processedStrings,
            IStringsLookup overlay,
            ref uint newIndex)
        {
            stream.Position -= major.HeaderLength;
            var majorRec = stream.GetMajorRecordFrame();
            if (!majorRec.TryLocateSubrecordFrame("EDID", out var edidRec)) throw new ArgumentException();
            if (edidRec.Content[0] != (byte)'s') return;
            if (!majorRec.TryLocateSubrecordPinFrame("DATA", out var dataRec)) throw new ArgumentException();
            stream.Position += dataRec.Location;
            AStringsAlignment.ProcessStringLink(stream, instr, processedStrings, overlay, ref newIndex);
        }

        private async Task ProcessStringsFilesIndices(Func<IMutagenReadStream> streamGetter, DirectoryInfo dataFolder, Language language, StringsSource source, ModKey modKey)
        {
            using var stream = streamGetter();
            switch (source)
            {
                case StringsSource.Normal:
                    ProcessStringsFiles(
                        GameRelease.Fallout4,
                        modKey,
                        dataFolder,
                        language,
                        StringsSource.Normal,
                        strict: false,
                        RenumberStringsFileEntries(
                            GameRelease.Fallout4,
                            modKey,
                            stream,
                            dataFolder,
                            language,
                            StringsSource.Normal,
                            new StringsAlignmentCustom("GMST", GameSettingStringHandler),
                            new RecordType[] { "KYWD", "FULL" }
                        ));
                    break;
                    //case StringsSource.DL:
                    //    ProcessStringsFiles(
                    //        modKey,
                    //        dataFolder,
                    //        language,
                    //        StringsSource.DL,
                    //        strict: true,
                    //        RenumberStringsFileEntries(
                    //            modKey,
                    //            stream,
                    //            dataFolder,
                    //            language,
                    //            StringsSource.DL,
                    //            new RecordType[] { "SCRL", "DESC" },
                    //        ));
                    //    break;
                    //case StringsSource.IL:
                    //    ProcessStringsFiles(
                    //        modKey,
                    //        dataFolder,
                    //        language,
                    //        StringsSource.IL,
                    //        strict: true,
                    //        RenumberStringsFileEntries(
                    //            modKey,
                    //            stream,
                    //            dataFolder,
                    //            language,
                    //            StringsSource.IL,
                    //            new RecordType[] { "DIAL" },
                    //            new RecordType[] { "INFO", "NAM1" }
                    //        ));
                    //    break;
            }
        }

    }
}
