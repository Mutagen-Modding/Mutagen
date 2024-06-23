using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class NavigationMapInfoBinaryCreateTranslation
{
    public static partial void FillBinaryIslandCustom(MutagenFrame frame, INavigationMapInfo item)
    {
        if (frame.ReadUInt8() > 0)
        {
            item.Island = IslandData.CreateFromBinary(frame);
        }
    }

    public static partial void FillBinaryParentCustom(MutagenFrame frame, INavigationMapInfo item)
    {
        var worldspace = FormLinkBinaryTranslation.Instance.Parse(reader: frame);
        if (worldspace.IsNull)
        {
            item.Parent = new NavigationMapInfoCellParent()
            {
                Cell = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<ICellGetter>()
            };
        }
        else
        {
            item.Parent = new NavigationMapInfoWorldspaceParent()
            {
                Worldspace = worldspace.ToLink<IWorldspaceGetter>(),
                Coord = P2Int16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(frame),
            };
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

    public static partial void WriteBinaryParentCustom(MutagenWriter writer, INavigationMapInfoGetter item)
    {
        switch (item.Parent)
        {
            case INavigationMapInfoWorldspaceParentGetter wrldSpace:
                FormKeyBinaryTranslation.Instance.Write(writer, wrldSpace.Worldspace);
                P2Int16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, wrldSpace.Coord);
                break;
            case INavigationMapInfoCellParentGetter cellParent:
                FormKeyBinaryTranslation.Instance.Write(writer, FormLinkInformation.Null);
                FormKeyBinaryTranslation.Instance.Write(writer, cellParent.Cell);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class NavigationMapInfoBinaryOverlay
{
    IIslandDataGetter? _island;
    public partial IIslandDataGetter? GetIslandCustom(int location) => _island;

    public partial IANavigationMapInfoParentGetter GetParentCustom(int location)
    {
        var worldspace = FormKey.Factory(_package.MetaData.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(_structData.Span.Slice(IslandEndingPos + 0x4, 0x4)));
        if (worldspace.IsNull)
        {
            return new NavigationMapInfoCellParent()
            {
                Cell = FormKey.Factory(_package.MetaData.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(_structData.Span.Slice(IslandEndingPos + 0x8, 0x4))).ToLink<ICellGetter>()
            };
        }
        else
        {
            return new NavigationMapInfoWorldspaceParent()
            {
                Worldspace = worldspace.ToLink<IWorldspaceGetter>(),
                Coord = new P2Int16(
                    BinaryPrimitives.ReadInt16LittleEndian(_structData.Span.Slice(IslandEndingPos + 0x8)),
                    BinaryPrimitives.ReadInt16LittleEndian(_structData.Span.Slice(IslandEndingPos + 0xA)))
            };
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
