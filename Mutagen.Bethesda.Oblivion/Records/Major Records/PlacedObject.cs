using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

public partial class PlacedObject
{
    [Flags]
    public enum ActionFlag
    {
        UseDefault = 0x001,
        Activate = 0x002,
        Open = 0x004,
        OpenByDefault = 0x008
    }
}

partial class PlacedObjectBinaryCreateTranslation
{
    public static partial void FillBinaryOpenByDefaultCustom(MutagenFrame frame, IPlacedObjectInternal item, PreviousParse lastParsed)
    {
        item.OpenByDefault = true;
        frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
    }
}

partial class PlacedObjectBinaryWriteTranslation
{
    public static partial void WriteBinaryOpenByDefaultCustom(MutagenWriter writer, IPlacedObjectGetter item)
    {
        if (item.OpenByDefault)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.ONAM))
            {
            }
        }
    }
}

partial class PlacedObjectBinaryOverlay
{
    private int? _OpenByDefaultLocation;
    public partial bool GetOpenByDefaultCustom() => _OpenByDefaultLocation.HasValue;
    partial void OpenByDefaultCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _OpenByDefaultLocation = (ushort)(stream.Position - offset);
    }
}