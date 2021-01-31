using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class IdleMarker
    {
        [Flags]
        public enum MajorFlag
        {
            ChildCanUse = 0x2000_0000,
        }

        [Flags]
        public enum Flag
        {
            RunInSequence = 0x0001,
            DoOnce = 0x0004,
            IgnoredBySandbox = 0x0010
        }
    }

    namespace Internals
    {
        public partial class IdleMarkerBinaryCreateTranslation
        {
            static partial void FillBinaryAnimationCountCustom(MutagenFrame frame, IIdleMarkerInternal item)
            {
                // Skip. Don't care
                frame.ReadSubrecordFrame();
            }

            public static ExtendedList<IFormLink<IIdleAnimationGetter>> ParseAnimations(IMutagenReadStream stream)
            {
                var subFrame = stream.ReadSubrecordFrame();
                var ret = new ExtendedList<IFormLink<IIdleAnimationGetter>>();
                int pos = 0;
                while (pos < subFrame.Content.Length)
                {
                    ret.Add(
                        new FormLink<IIdleAnimationGetter>(
                            FormKeyBinaryTranslation.Instance.Parse(subFrame.Content.Slice(pos), stream.MetaData.MasterReferences!)));
                    pos += 4;
                }
                return ret;
            }

            static partial void FillBinaryAnimationsCustom(MutagenFrame frame, IIdleMarkerInternal item)
            {
                item.Animations = ParseAnimations(frame);
            }
        }

        public partial class IdleMarkerBinaryWriteTranslation
        {
            static partial void WriteBinaryAnimationCountCustom(MutagenWriter writer, IIdleMarkerGetter item)
            {
                if (!item.Animations.TryGet(out var anims)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.IDLC))
                {
                    writer.Write(checked((byte)anims.Count));
                }
            }

            static partial void WriteBinaryAnimationsCustom(MutagenWriter writer, IIdleMarkerGetter item)
            {
                if (!item.Animations.TryGet(out var anims)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.IDLA))
                {
                    foreach (var anim in anims)
                    {
                        FormKeyBinaryTranslation.Instance.Write(writer, anim.FormKey);
                    }
                }
            }
        }

        public partial class IdleMarkerBinaryOverlay
        {
            partial void AnimationCountCustomParse(OverlayStream stream, int offset)
            {
                // Skip. Don't care
                stream.ReadSubrecordFrame();
            }

            public IReadOnlyList<IFormLink<IIdleAnimationGetter>>? Animations { get; private set; }

            partial void AnimationsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                var subHeader = stream.ReadSubrecord();
                Animations = BinaryOverlayList.FactoryByStartIndex<IFormLink<IIdleAnimationGetter>>(
                    mem: stream.RemainingMemory.Slice(0, subHeader.ContentLength),
                    package: _package,
                    itemLength: 4,
                    getter: (s, p) => new FormLink<IIdleAnimationGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))));
            }
        }
    }
}
