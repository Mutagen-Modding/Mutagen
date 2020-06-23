using DynamicData;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class FootstepSetBinaryCreateTranslation
        {
            static partial void FillBinaryCountCustom(MutagenFrame frame, IFootstepSetInternal item)
            {
                var counts = GetListCounts(frame);

                IEnumerable<IFormLink<Footstep>> ReadIn(int count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        yield return new FormLink<Footstep>(FormKeyBinaryTranslation.Instance.Parse(frame));
                    }
                }

                item.WalkForwardFootsteps.SetTo(ReadIn(counts[0]));
                item.RunForwardFootsteps.SetTo(ReadIn(counts[1]));
                item.WalkForwardAlternateFootsteps.SetTo(ReadIn(counts[2]));
                item.RunForwardAlternateFootsteps.SetTo(ReadIn(counts[3]));
                item.WalkForwardAlternateFootsteps2.SetTo(ReadIn(counts[4]));
            }

            public static int[] GetListCounts(IMutagenReadStream frame)
            {
                var subFrame = frame.ReadSubrecord(RecordTypes.XCNT);
                if (subFrame.ContentLength != 20)
                {
                    throw new ArgumentException($"XCNT record had unexpected length {subFrame.ContentLength} != 20");
                }

                int[] ret = new int[5];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = checked((int)frame.ReadUInt32());
                }

                var formIDCount = ret.Sum();

                var dataHeader = frame.ReadSubrecord(RecordTypes.DATA);
                var expectedLen = formIDCount * 4;
                if (dataHeader.ContentLength != expectedLen)
                {
                    throw new ArgumentException($"DATA record had unexpected length that did not match previous counts {dataHeader.ContentLength} != {expectedLen}");
                }
                return ret;
            }
        }

        public partial class FootstepSetBinaryWriteTranslation
        {
            static partial void WriteBinaryCountCustom(MutagenWriter writer, IFootstepSetGetter item)
            {
                var walkForwardFootsteps = item.WalkForwardFootsteps;

                using (HeaderExport.Subrecord(writer, RecordTypes.XCNT))
                {
                    writer.Write(walkForwardFootsteps.Count);
                    writer.Write(item.RunForwardFootsteps.Count);
                    writer.Write(item.WalkForwardAlternateFootsteps.Count);
                    writer.Write(item.RunForwardAlternateFootsteps.Count);
                    writer.Write(item.WalkForwardAlternateFootsteps2.Count);
                }

                void WriteListOfFormKeys(IReadOnlyList<IFormLink<IFootstepGetter>> formKeys)
                {
                    foreach (var link in formKeys)
                    {
                        FormLinkBinaryTranslation.Instance.Write(writer, link);
                    }
                }

                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    WriteListOfFormKeys(walkForwardFootsteps);
                    WriteListOfFormKeys(item.RunForwardFootsteps);
                    WriteListOfFormKeys(item.WalkForwardAlternateFootsteps);
                    WriteListOfFormKeys(item.RunForwardAlternateFootsteps);
                    WriteListOfFormKeys(item.WalkForwardAlternateFootsteps2);
                }
            }
        }

        public partial class FootstepSetBinaryOverlay
        {
            public IReadOnlyList<IFormLink<IFootstepGetter>> WalkForwardFootsteps { get; private set; } = ListExt.Empty<IFormLink<IFootstepGetter>>();

            public IReadOnlyList<IFormLink<IFootstepGetter>> RunForwardFootsteps { get; private set; } = ListExt.Empty<IFormLink<IFootstepGetter>>();

            public IReadOnlyList<IFormLink<IFootstepGetter>> WalkForwardAlternateFootsteps { get; private set; } = ListExt.Empty<IFormLink<IFootstepGetter>>();

            public IReadOnlyList<IFormLink<IFootstepGetter>> RunForwardAlternateFootsteps { get; private set; } = ListExt.Empty<IFormLink<IFootstepGetter>>();

            public IReadOnlyList<IFormLink<IFootstepGetter>> WalkForwardAlternateFootsteps2 { get; private set; } = ListExt.Empty<IFormLink<IFootstepGetter>>();

            partial void CountCustomParse(OverlayStream stream, int offset)
            {
                int[] counts = FootstepSetBinaryCreateTranslation.GetListCounts(stream);

                IReadOnlyList<IFormLink<IFootstepGetter>> Get(int index)
                {
                    var ret =  BinaryOverlayList<IFormLink<IFootstepGetter>>.FactoryByCount(
                        _data.Slice(stream.Position - offset, 4 * counts[index]),
                        _package,
                        itemLength: 4,
                        count: checked((uint)counts[index]),
                        getter: (s, p) => new FormLink<IFootstepGetter>(FormKeyBinaryTranslation.Instance.Parse(s, p.MetaData.MasterReferences!)));
                    stream.Position += 4 * counts[index];
                    return ret;
                }

                WalkForwardFootsteps = Get(0);
                RunForwardFootsteps = Get(1);
                WalkForwardAlternateFootsteps = Get(2);
                RunForwardAlternateFootsteps = Get(3);
                WalkForwardAlternateFootsteps2 = Get(4);
            }
        }
    }
}
