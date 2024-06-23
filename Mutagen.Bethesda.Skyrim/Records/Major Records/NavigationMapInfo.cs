using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
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
        item.Parent = GetParent(frame);
    }

    public static ANavigationMapInfoParent GetParent<TStream>(TStream frame)
        where TStream : IMutagenReadStream
    {
        var workspaceFk = FormKeyBinaryTranslation.Instance.Parse(frame.GetSpan(4), frame.MetaData.MasterReferences.Raw);
        if (workspaceFk.IsNull)
        {
            return new NavigationMapInfoCellParent()
            {
                Unused = frame.ReadInt32(),
                ParentCell = FormKeyBinaryTranslation.Instance.Parse(frame.ReadSpan(4), frame.MetaData.MasterReferences.Raw).ToLink<ICellGetter>()
            };
        }
        else
        {
            return new NavigationMapInfoWorldParent()
            {
                ParentWorldspace = FormKeyBinaryTranslation.Instance.Parse(frame.ReadSpan(4), frame.MetaData.MasterReferences.Raw).ToLink<IWorldspaceGetter>(),
                ParentWorldspaceCoord = new P2Int16(
                    frame.ReadInt16(),
                    frame.ReadInt16())
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

    public static partial void WriteBinaryParentParseLogicCustom(MutagenWriter writer, INavigationMapInfoGetter item)
    {
        item.Parent.WriteToBinary(writer);
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

    public IFormLinkGetter<ICellGetter> ParentCell =>
        FormLinkBinaryTranslation.Instance.OverlayFactory<ICellGetter>(_package,
            _structData.Span.Slice(IslandEndingPos + 0x8, 0x4));

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        if (_structData[LinkedDoorsEndingPos] > 0)
        {
            using var islandStream = new OverlayStream(_structData.Slice(LinkedDoorsEndingPos + 1), stream.MetaData);
            this._island = IslandDataBinaryOverlay.IslandDataFactory(
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

    public IANavigationMapInfoParentGetter Parent
    {
        get
        {
            using var islandStream = new OverlayStream(_structData.Slice(IslandEndingPos + 4), _package.MetaData);
            return NavigationMapInfoBinaryCreateTranslation.GetParent(islandStream);
        }
    }
}
