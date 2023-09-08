using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Fallout4.Internals;
using static Mutagen.Bethesda.Fallout4.Subgraph;

namespace Mutagen.Bethesda.Fallout4;

public partial class Subgraph
{
    public enum SubgraphRole
    {
        MT,
        Weapon,
        Furniture,
        Idle,
        Pipboy
    }
}
    
partial class SubgraphBinaryCreateTranslation
{
    public static partial void FillBinaryRoleCustom(MutagenFrame frame, ISubgraph item, PreviousParse lastParsed)
    {
        frame.ReadSubrecordHeader(RecordTypes.SRAF);
        item.Role = (SubgraphRole)frame.ReadUInt16();
        item.Perspective = (Perspective)frame.ReadUInt16();
    }
}

partial class SubgraphBinaryOverlay
{
    partial void RoleCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        stream.ReadSubrecordHeader(RecordTypes.SRAF);
        _role = (SubgraphRole)stream.ReadUInt16();
        Perspective = (Perspective)stream.ReadUInt16();
    }

    private Subgraph.SubgraphRole _role;
    public partial Subgraph.SubgraphRole GetRoleCustom() => _role;
    public Perspective Perspective { get; private set; }
}

partial class SubgraphBinaryWriteTranslation
{
    public static partial void WriteBinaryRoleCustom(MutagenWriter writer, ISubgraphGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.SRAF);
        writer.Write((ushort)item.Role);
        writer.Write((ushort)item.Perspective);
    }
}