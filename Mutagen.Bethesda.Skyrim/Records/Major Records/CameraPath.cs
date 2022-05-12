using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class CameraPath
{
    [Flags]
    public enum ZoomType
    {
        Default,
        Disable,
        ShotList
    }
}

partial class CameraPathBinaryCreateTranslation
{
    public static partial void FillBinaryZoomCustom(MutagenFrame frame, ICameraPathInternal item)
    {
        var subFrame = frame.ReadSubrecord();
        if (subFrame.Content.Length != 1)
        {
            throw new ArgumentException();
        }
        var e = subFrame.Content[0];
        item.Zoom = (CameraPath.ZoomType)(e % 128);
        item.ZoomMustHaveCameraShots = e < 128;
    }
}

partial class CameraPathBinaryWriteTranslation
{
    public static partial void WriteBinaryZoomCustom(MutagenWriter writer, ICameraPathGetter item)
    {
        byte e = (byte)item.Zoom;
        if (!item.ZoomMustHaveCameraShots)
        {
            e += 128;
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            writer.Write(e);
        }
    }
}

partial class CameraPathBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }

    int? _zoomLoc;
    public partial CameraPath.ZoomType GetZoomCustom() => _zoomLoc.HasValue ? (CameraPath.ZoomType)(HeaderTranslation.ExtractSubrecordMemory(_data, _zoomLoc.Value, _package.MetaData.Constants)[0] % 128) : default;

    public bool ZoomMustHaveCameraShots => _zoomLoc.HasValue && HeaderTranslation.ExtractSubrecordMemory(_data, _zoomLoc.Value, _package.MetaData.Constants)[0] < 128;

    partial void ZoomCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _zoomLoc = stream.Position - offset;
    }
}