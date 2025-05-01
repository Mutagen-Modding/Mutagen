using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using RecordTypes = Mutagen.Bethesda.Starfield.Internals.RecordTypes;

namespace Mutagen.Bethesda.Starfield;

internal partial class GameplayOptionsGroupBinaryOverlay
{
    public IAGameplayOptionsNodeGetter Options { get; private set; } = null!;

    public partial ParseResult GroupsCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        var bnam = stream.ReadSubrecord(RecordTypes.BNAM);
        if (bnam.AsInt8() > 0)
        {
            this.Options = GameplayOptionsGroupRootBinaryOverlay.GameplayOptionsGroupRootFactory(stream, _package);
        }
        else
        {
            this.Options = GameplayOptionsGroupLeafBinaryOverlay.GameplayOptionsGroupLeafFactory(stream, _package);
        }
        return (int)GameplayOptionsGroup_FieldIndex.Options;
    }
}

internal partial class GameplayOptionsGroupBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryGroupsCustom(MutagenFrame frame, IGameplayOptionsGroupInternal item, PreviousParse lastParsed)
    {
        var bnam = frame.ReadSubrecord(RecordTypes.BNAM);
        frame = frame.SpawnAll();
        if (bnam.AsInt8() > 0)
        {
            item.Options = GameplayOptionsGroupRoot.CreateFromBinary(frame);
        }
        else
        {
            item.Options = GameplayOptionsGroupLeaf.CreateFromBinary(frame);
        }
        return (int)GameplayOptionsGroup_FieldIndex.Options;
    }
}

public partial class GameplayOptionsGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryGroupsCustom(MutagenWriter writer, IGameplayOptionsGroupGetter item)
    {
        var opt = item.Options;
        var isRoot = opt is IGameplayOptionsGroupRoot;
        using (HeaderExport.Subrecord(writer, RecordTypes.BNAM))
        {
            writer.Write(isRoot ? (byte)1 : (byte)0);
        }
        opt.WriteToBinary(writer);
    }
}