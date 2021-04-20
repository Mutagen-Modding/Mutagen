using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Records.Binary.Streams;
using System;
using System.Buffers.Binary;
using System.Diagnostics;

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
                FormKey formKey = FormKey.Factory(masters, form.Raw);
                if (cache.IsOfRecordType<Npc>(formKey))
                {
                    return new NpcOwner()
                    {
                        Npc = new FormLink<INpcGetter>(FormKeyBinaryTranslation.Instance.Parse(span, masters)),
                        Global = new FormLink<IGlobalGetter>(FormKeyBinaryTranslation.Instance.Parse(span.Slice(4), masters))
                    };
                }
                else if (cache.IsOfRecordType<Faction>(formKey))
                {
                    return new FactionOwner()
                    {
                        Faction = new FormLink<IFactionGetter>(FormKeyBinaryTranslation.Instance.Parse(span, masters)),
                        RequiredRank = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4))
                    };
                }
                else
                {
                    return new NoOwner()
                    {
                        RawOwnerData = BinaryPrimitives.ReadUInt32LittleEndian(span),
                        RawVariableData = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4))
                    };
                }
            }

            static partial void FillBinaryOwnerCustom(MutagenFrame frame, IExtraData item)
            {
                item.Owner = GetBinaryOwner(frame.ReadSpan(8), frame.MetaData.RecordInfoCache!, frame.MetaData.MasterReferences!);
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
                        writer.Write(noOwner.RawVariableData);
                        break;
                    case NpcOwner npcOwner:
                        FormKeyBinaryTranslation.Instance.Write(writer, npcOwner.Npc.FormKey);
                        FormKeyBinaryTranslation.Instance.Write(writer, npcOwner.Global.FormKey);
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
            IOwnerTargetGetter GetOwnerCustom(int location) => ExtraDataBinaryCreateTranslation.GetBinaryOwner(_data.Slice(location), _package.MetaData.RecordInfoCache!, _package.MetaData.MasterReferences!);
        }
    }
}
