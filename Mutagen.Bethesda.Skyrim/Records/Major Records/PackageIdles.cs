using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class PackageIdles
    {
        public enum Types
        {
            Random = 8,
            RunInSequence = 9,
            RandomDoOnce = 12,
            RunInSequenceDoOnce = 13,
        }
    }

    namespace Internals
    {
        public partial class PackageIdlesBinaryCreateTranslation
        {
            static partial void FillBinaryTimerSettingCustom(MutagenFrame frame, IPackageIdles item)
            {
                FillBinaryAnimationsCustom(frame, item);
            }

            static partial void FillBinaryAnimationsCustom(MutagenFrame frame, IPackageIdles item)
            {
                byte? count = null;
                for (int i = 0; i < 3; i++)
                {
                    var subRecord = frame.GetSubrecordFrame();
                    if (subRecord.Header.RecordType == RecordTypes.IDLC)
                    {
                        // Counter start
                        if (subRecord.Content.Length != 1)
                        {
                            throw new ArgumentException("Unexpected counter length");
                        }
                        count = subRecord.Content[0];
                        frame.Position += subRecord.TotalLength;
                    }
                    else if (subRecord.Header.RecordType == RecordTypes.IDLA)
                    {
                        if (count == null)
                        {
                            item.Animations.SetTo(
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IFormLink<IdleAnimation>>.Instance.Parse(
                                    frame: frame,
                                    triggeringRecord: RecordTypes.IDLA,
                                    transl: FormLinkBinaryTranslation.Instance.Parse));
                        }
                        else
                        {
                            item.Animations.SetTo(
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IFormLink<IdleAnimation>>.Instance.Parse(
                                    frame: frame,
                                    amount: count.Value,
                                    triggeringRecord: RecordTypes.IDLA,
                                    transl: FormLinkBinaryTranslation.Instance.Parse));
                        }
                    }
                    else if (subRecord.Header.RecordType == RecordTypes.IDLT)
                    {
                        item.TimerSetting = SpanExt.GetFloat(subRecord.Content);
                        frame.Position += subRecord.TotalLength;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count.HasValue && count.Value != item.Animations.Count)
                {
                    throw new ArgumentException("Idle animation counts did not match.");
                }
            }
        }

        public partial class PackageIdlesBinaryWriteTranslation
        {
            static partial void WriteBinaryAnimationsCustom(MutagenWriter writer, IPackageIdlesGetter item)
            {
                var anims = item.Animations;
                using (HeaderExport.Subrecord(writer, RecordTypes.IDLC))
                {
                    writer.Write(anims.Count, 1);
                }

                using (HeaderExport.Subrecord(writer, RecordTypes.IDLT))
                {
                    writer.Write(item.TimerSetting);
                }

                using (HeaderExport.Subrecord(writer, RecordTypes.IDLA))
                {
                    foreach (var anim in anims)
                    {
                        Mutagen.Bethesda.Binary.FormLinkBinaryTranslation.Instance.Write(
                            writer: writer,
                            item: anim);
                    }
                }
            }
        }

        public partial class PackageIdlesBinaryOverlay
        {
            public IReadOnlyList<IFormLink<IIdleAnimationGetter>> Animations { get; private set; } = ListExt.Empty<IFormLink<IIdleAnimationGetter>>();

            private float _timerSetting;
            public Single GetTimerSettingCustom() => _timerSetting;

            partial void AnimationsCustomParse(
                BinaryMemoryReadStream stream,
                long finalPos,
                int offset,
                RecordType type,
                int? lastParsed)
            {
                byte? count = null;
                for (int i = 0; i < 3; i++)
                {
                    var subRecord = _package.MetaData.Constants.GetSubrecordFrame(stream);
                    if (subRecord.Header.RecordType == RecordTypes.IDLC)
                    {
                        // Counter start
                        if (subRecord.Content.Length != 1)
                        {
                            throw new ArgumentException("Unexpected counter length");
                        }
                        count = subRecord.Content[0];
                        stream.Position += subRecord.TotalLength;
                    }
                    else if (subRecord.Header.RecordType == RecordTypes.IDLA)
                    {
                        if (count == null)
                        {
                            this.Animations = BinaryOverlayList<IFormLink<IIdleAnimationGetter>>.FactoryByArray(
                                mem: stream.RemainingMemory,
                                package: _package,
                                getter: (s, p) => new FormLink<IIdleAnimationGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))),
                                locs: ParseRecordLocations(
                                    stream: stream,
                                    finalPos: finalPos,
                                    constants: _package.MetaData.Constants.SubConstants,
                                    trigger: type,
                                    skipHeader: true));
                        }
                        else
                        {
                            var subMeta = _package.MetaData.Constants.ReadSubrecord(stream);
                            var subLen = subMeta.ContentLength;
                            this.Animations = BinaryOverlayList<IFormLink<IIdleAnimationGetter>>.FactoryByStartIndex(
                                mem: stream.RemainingMemory.Slice(0, subLen),
                                package: _package,
                                itemLength: 4,
                                getter: (s, p) => new FormLink<IIdleAnimationGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))));
                            stream.Position += subLen;
                        }
                    }
                    else if (subRecord.Header.RecordType == RecordTypes.IDLT)
                    {
                        _timerSetting = SpanExt.GetFloat(subRecord.Content);
                        stream.Position += subRecord.TotalLength;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count.HasValue && count.Value != Animations.Count)
                {
                    throw new ArgumentException("Idle animation counts did not match.");
                }
            }
        }
    }
}
