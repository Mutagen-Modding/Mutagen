using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim;

partial class NavigationMapInfoBinaryCreateTranslation
{
    public static partial void FillBinaryIslandCustom(MutagenFrame frame, INavigationMapInfo item)
    {
        if (frame.ReadUInt8() > 0)
        {
            item.Island = IslandData.CreateFromBinary(frame);
        }
    }

    public static partial void FillBinaryParentParseLogicCustom(MutagenFrame frame, INavigationMapInfo item)
    {
        if (item.ParentWorldspace.IsNull)
        {
            item.ParentCell.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
        }
        else
        {
            item.ParentWorldspaceCoord = P2Int16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(frame);
        }
    }
}

partial class NavigationMapInfoBinaryWriteTranslation
{
    public static partial void WriteBinaryIslandCustom(MutagenWriter writer, INavigationMapInfoGetter item)
    {
        if (item.Island is {} island)
        {
            writer.Write((byte)1);
            island.WriteToBinary(writer);
        }
        else
        {
            writer.Write(default(byte));
        }
    }

    public static partial void WriteBinaryParentParseLogicCustom(MutagenWriter writer, INavigationMapInfoGetter item)
    {
        if (item.ParentWorldspace.IsNull)
        {
            FormKeyBinaryTranslation.Instance.Write(writer, item.ParentCell.FormKey);
        }
        else
        {
            P2Int16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, item.ParentWorldspaceCoord);
        }
    }
}

partial class NavigationMapInfoBinaryOverlay
{
    IIslandDataGetter? _island;
    public partial IIslandDataGetter? GetIslandCustom(int location) => _island;

    public P2Int16 ParentWorldspaceCoord
    {
        get
        {
            return new P2Int16(
                BinaryPrimitives.ReadInt16LittleEndian(_structData.Span.Slice(IslandEndingPos + 0x8)),
                BinaryPrimitives.ReadInt16LittleEndian(_structData.Span.Slice(IslandEndingPos + 0xA)));
        }
    }

    public IFormLinkGetter<ICellGetter> ParentCell
    {
        get
        {
            return new FormLink<ICellGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_structData.Span.Slice(IslandEndingPos + 0x8, 0x4))));
        }
    }

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        if (_structData[LinkedDoorsEndingPos] > 0)
        {
            using var islandStream = new OverlayStream(_structData.Slice(LinkedDoorsEndingPos + 1), stream.MetaData);
            this._island =  IslandDataBinaryOverlay.IslandDataFactory(
                islandStream,
                _package);
            this.IslandEndingPos = LinkedDoorsEndingPos + 1 + islandStream.Position;
        }
        else
        {
            this._island = null;
            this.IslandEndingPos = LinkedDoorsEndingPos + 1;
        }
    }
}
