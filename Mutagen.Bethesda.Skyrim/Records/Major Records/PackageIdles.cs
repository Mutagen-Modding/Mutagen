using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

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
                    if (subRecord.RecordType == RecordTypes.IDLC)
                    {
                        // Counter start
                        if (subRecord.Content.Length != 1)
                        {
                            throw new ArgumentException("Unexpected counter length");
                        }
                        count = subRecord.Content[0];
                        frame.Position += subRecord.TotalLength;
                    }
                    else if (subRecord.RecordType == RecordTypes.IDLA)
                    {
                        if (count == null)
                        {
                            item.Animations.SetTo(
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IFormLinkGetter<IdleAnimation>>.Instance.Parse(
                                    frame: frame,
                                    triggeringRecord: RecordTypes.IDLA,
                                    transl: FormLinkBinaryTranslation.Instance.Parse));
                        }
                        else
                        {
                            item.Animations.SetTo(
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IFormLinkGetter<IdleAnimation>>.Instance.Parse(
                                    frame: frame,
                                    amount: count.Value,
                                    triggeringRecord: RecordTypes.IDLA,
                                    transl: FormLinkBinaryTranslation.Instance.Parse));
                        }
                    }
                    else if (subRecord.RecordType == RecordTypes.IDLT)
                    {
                        item.TimerSetting = subRecord.Content.Float();
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
            public IReadOnlyList<IFormLinkGetter<IIdleAnimationGetter>> Animations { get; private set; } = ListExt.Empty<IFormLinkGetter<IIdleAnimationGetter>>();

            private float _timerSetting;
            public Single GetTimerSettingCustom() => _timerSetting;

            partial void AnimationsCustomParse(
                OverlayStream stream,
                long finalPos,
                int offset,
                RecordType type,
                int? lastParsed)
            {
                byte? count = null;
                for (int i = 0; i < 3; i++)
                {
                    var subRecord = stream.GetSubrecordFrame();
                    if (subRecord.RecordType == RecordTypes.IDLC)
                    {
                        // Counter start
                        if (subRecord.Content.Length != 1)
                        {
                            throw new ArgumentException("Unexpected counter length");
                        }
                        count = subRecord.Content[0];
                        stream.Position += subRecord.TotalLength;
                    }
                    else if (subRecord.RecordType == RecordTypes.IDLA)
                    {
                        if (count == null)
                        {
                            this.Animations = BinaryOverlayList.FactoryByArray<IFormLinkGetter<IIdleAnimationGetter>>(
                                mem: stream.RemainingMemory,
                                package: _package,
                                getter: (s, p) => new FormLink<IIdleAnimationGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))),
                                locs: ParseRecordLocations(
                                    stream: stream,
                                    constants: _package.MetaData.Constants.SubConstants,
                                    trigger: type,
                                    skipHeader: true));
                        }
                        else
                        {
                            var subMeta = stream.ReadSubrecord();
                            var subLen = subMeta.ContentLength;
                            this.Animations = BinaryOverlayList.FactoryByStartIndex<IFormLinkGetter<IIdleAnimationGetter>>(
                                mem: stream.RemainingMemory.Slice(0, subLen),
                                package: _package,
                                itemLength: 4,
                                getter: (s, p) => new FormLink<IIdleAnimationGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))));
                            stream.Position += subLen;
                        }
                    }
                    else if (subRecord.RecordType == RecordTypes.IDLT)
                    {
                        _timerSetting = subRecord.Content.Float();
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
