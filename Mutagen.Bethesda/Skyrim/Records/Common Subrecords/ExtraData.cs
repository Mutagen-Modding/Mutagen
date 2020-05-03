using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ExtraData
    {
        public OwnerTarget Owner { get; set; } = new NoOwner();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IOwnerTargetGetter IExtraDataGetter.Owner => Owner;
    }

    namespace Internals
    {
        public partial class ExtraDataBinaryCreateTranslation
        {
            public static OwnerTarget GetBinaryOwner(ReadOnlySpan<byte> span, RecordInfoCache cache, MasterReferenceReader masters)
            {
                FormID form = new FormID(BinaryPrimitives.ReadUInt32LittleEndian(span));
                if (cache.IsOfRecordType<Npc>(form))
                {
                    var npcOwner = new NpcOwner();
                    npcOwner.Npc.FormKey = FormKeyBinaryTranslation.Instance.Parse(span, masters);
                    npcOwner.RawVariableData = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4));
                    return npcOwner;
                }
                else if (cache.IsOfRecordType<Faction>(form))
                {
                    var factionOwner = new FactionOwner();
                    factionOwner.Faction.FormKey = FormKeyBinaryTranslation.Instance.Parse(span, masters);
                    factionOwner.RequiredRank = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4));
                    return factionOwner;
                }
                else
                {
                    var noOwner = new NoOwner();
                    noOwner.RawOwnerData = BinaryPrimitives.ReadUInt32LittleEndian(span);
                    noOwner.Global.FormKey = FormKeyBinaryTranslation.Instance.Parse(span, masters);
                    return noOwner;
                }
            }

            static partial void FillBinaryOwnerCustom(MutagenFrame frame, IExtraData item)
            {
                item.Owner = GetBinaryOwner(frame.ReadSpan(8), frame.RecordInfoCache!, frame.MasterReferences!);
            }
        }

        public partial class ExtraDataBinaryWriteTranslation
        {
            static partial void WriteBinaryOwnerCustom(MutagenWriter writer, IExtraDataGetter item)
            {
                switch (item.Owner)
                {
                    case NoOwner noOwner:
                        writer.Write(noOwner.RawOwnerData);
                        FormKeyBinaryTranslation.Instance.Write(writer, noOwner.Global.FormKey);
                        break;
                    case NpcOwner npcOwner:
                        FormKeyBinaryTranslation.Instance.Write(writer, npcOwner.Npc.FormKey);
                        writer.Write(npcOwner.RawVariableData);
                        break;
                    case FactionOwner factionOwner:
                        FormKeyBinaryTranslation.Instance.Write(writer, factionOwner.Faction.FormKey);
                        writer.Write(factionOwner.RequiredRank);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class ExtraDataBinaryOverlay
        {
            IOwnerTargetGetter GetOwnerCustom(int location) => ExtraDataBinaryCreateTranslation.GetBinaryOwner(_data.Slice(location), _package.RecordInfoCache, _package.MasterReferences);
        }
    }
}
