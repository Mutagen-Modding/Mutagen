using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
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

    namespace Internals
    {
        public partial class CameraPathBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, ICameraPathInternal item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }

            static partial void FillBinaryZoomCustom(MutagenFrame frame, ICameraPathInternal item)
            {
                var subFrame = frame.ReadSubrecordFrame();
                if (subFrame.Content.Length != 1)
                {
                    throw new ArgumentException();
                }
                var e = subFrame.Content[0];
                item.Zoom = (CameraPath.ZoomType)(e % 128);
                item.ZoomMustHaveCameraShots = e < 128;
            }
        }

        public partial class CameraPathBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, ICameraPathGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }

            static partial void WriteBinaryZoomCustom(MutagenWriter writer, ICameraPathGetter item)
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

        public partial class CameraPathBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }

            int? _zoomLoc;
            public CameraPath.ZoomType GetZoomCustom() => _zoomLoc.HasValue ? (CameraPath.ZoomType)(HeaderTranslation.ExtractSubrecordSpan(_data, _zoomLoc.Value, _package.MetaData.Constants)[0] % 128) : default;

            public bool ZoomMustHaveCameraShots => _zoomLoc.HasValue && HeaderTranslation.ExtractSubrecordSpan(_data, _zoomLoc.Value, _package.MetaData.Constants)[0] < 128;

            partial void ZoomCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _zoomLoc = stream.Position - offset;
            }
        }
    }
}
