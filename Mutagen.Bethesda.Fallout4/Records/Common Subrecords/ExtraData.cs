using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Utility;
using System.Buffers.Binary;
using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Fallout4;

public partial class ExtraData
{
    public OwnerTarget Owner { get; set; } = new NoOwner();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IOwnerTargetGetter IExtraDataGetter.Owner => Owner;
}

partial class ExtraDataBinaryCreateTranslation
{
    public static OwnerTarget GetBinaryOwner(ReadOnlySpan<byte> span, RecordTypeInfoCacheReader cache, IReadOnlySeparatedMasterPackage masters)
    {
        FormID form = new FormID(BinaryPrimitives.ReadUInt32LittleEndian(span));
        FormKey formKey = FormKey.Factory(masters, form);
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

    public static partial void FillBinaryOwnerCustom(MutagenFrame frame, IExtraData item)
    {
        item.Owner = GetBinaryOwner(frame.ReadSpan(8), frame.MetaData.RecordInfoCache!, frame.MetaData.MasterReferences);
    }
}

partial class ExtraDataBinaryWriteTranslation
{
    public static partial void WriteBinaryOwnerCustom(MutagenWriter writer, IExtraDataGetter item)
    {
        switch (item.Owner)
        {
            case NoOwner noOwner:
                writer.Write(noOwner.RawOwnerData);
                writer.Write(noOwner.RawVariableData);
                break;
            case NpcOwner npcOwner:
                FormKeyBinaryTranslation.Instance.Write(writer, npcOwner.Npc);
                FormKeyBinaryTranslation.Instance.Write(writer, npcOwner.Global);
                break;
            case FactionOwner factionOwner:
                FormKeyBinaryTranslation.Instance.Write(writer, factionOwner.Faction);
                writer.Write(factionOwner.RequiredRank);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class ExtraDataBinaryOverlay
{
    #region Owner
    public partial IOwnerTargetGetter GetOwnerCustom(int location);
    public IOwnerTargetGetter Owner => GetOwnerCustom(location: 0x0);
    #endregion
    
    public partial IOwnerTargetGetter GetOwnerCustom(int location) => ExtraDataBinaryCreateTranslation.GetBinaryOwner(_structData.Slice(location), _package.MetaData.RecordInfoCache!, _package.MetaData.MasterReferences);
}