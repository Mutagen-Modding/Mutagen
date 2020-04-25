using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ContainerEntryData
    {
        public ContainerOwnerTarget Owner { get; set; } = new ContainerNoOwner();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IContainerOwnerTargetGetter IContainerEntryDataGetter.Owner => Owner;
    }

    namespace Internals
    {
        public partial class ContainerEntryDataBinaryCreateTranslation
        {
            public static ContainerOwnerTarget GetBinaryOwner(ReadOnlySpan<byte> span, RecordInfoCache cache, MasterReferenceReader masters)
            {
                FormID form = new FormID(BinaryPrimitives.ReadUInt32LittleEndian(span));
                if (cache.IsOfRecordType<Npc>(form))
                {
                    var npcOwner = new ContainerNpcOwner();
                    npcOwner.Npc.FormKey = FormKeyBinaryTranslation.Instance.Parse(span, masters);
                    npcOwner.RawVariableData = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4));
                    return npcOwner;
                }
                else if (cache.IsOfRecordType<Faction>(form))
                {
                    var factionOwner = new ContainerFactionOwner();
                    factionOwner.Faction.FormKey = FormKeyBinaryTranslation.Instance.Parse(span, masters);
                    factionOwner.RequiredRank = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4));
                    return factionOwner;
                }
                else
                {
                    var noOwner = new ContainerNoOwner();
                    noOwner.RawOwnerData = BinaryPrimitives.ReadUInt32LittleEndian(span);
                    noOwner.Global.FormKey = FormKeyBinaryTranslation.Instance.Parse(span, masters);
                    return noOwner;
                }
            }

            static partial void FillBinaryOwnerCustom(MutagenFrame frame, IContainerEntryData item)
            {
                item.Owner = GetBinaryOwner(frame.ReadSpan(8), frame.RecordInfoCache!, frame.MasterReferences!);
            }
        }

        public partial class ContainerEntryDataBinaryWriteTranslation
        {
            static partial void WriteBinaryOwnerCustom(MutagenWriter writer, IContainerEntryDataGetter item)
            {
                switch (item.Owner)
                {
                    case ContainerNoOwner noOwner:
                        writer.Write(noOwner.RawOwnerData);
                        FormKeyBinaryTranslation.Instance.Write(writer, noOwner.Global.FormKey);
                        break;
                    case ContainerNpcOwner npcOwner:
                        FormKeyBinaryTranslation.Instance.Write(writer, npcOwner.Npc.FormKey);
                        writer.Write(npcOwner.RawVariableData);
                        break;
                    case ContainerFactionOwner factionOwner:
                        FormKeyBinaryTranslation.Instance.Write(writer, factionOwner.Faction.FormKey);
                        writer.Write(factionOwner.RequiredRank);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class ContainerEntryDataBinaryOverlay
        {
            IContainerOwnerTargetGetter GetOwnerCustom(int location) => ContainerEntryDataBinaryCreateTranslation.GetBinaryOwner(_data.Slice(location), _package.RecordInfoCache, _package.MasterReferences);
        }
    }
}
