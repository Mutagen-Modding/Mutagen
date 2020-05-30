using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class PlacedObject
    {
        public enum ActionFlag
        {
            UseDefault,
            Activate,
            Open,
            OpenByDefault,
        }
    }

    namespace Internals
    {
        public partial class PlacedObjectBinaryCreateTranslation
        {
            public const byte HasImageSpaceFlag = 0x40;
            public const byte HasLightingTemplateFlag = 0x80;

            static partial void FillBinaryBoundDataCustom(MutagenFrame frame, IPlacedObjectInternal item)
            {
                var header = frame.ReadSubrecordFrame();
                if (header.Content.Length != 4)
                {
                    throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
                }
                var roomCount = header.Content[0];
                var flags = header.Content[1];
                item.Unknown2 = BinaryPrimitives.ReadInt16LittleEndian(header.Content.Slice(2));
                while (frame.Reader.TryReadSubrecord(out var subHeader))
                {
                    switch (subHeader.RecordTypeInt)
                    {
                        case 0x4D414E4C: // LNAM
                            item.LightingTemplate = FormKeyBinaryTranslation.Instance.Parse(frame);
                            break;
                        case 0x4D414E49: // INAM
                            item.ImageSpace = FormKeyBinaryTranslation.Instance.Parse(frame);
                            break;
                        case 0x4D524C58: // XLRM
                            item.LinkedRooms.Add(new FormLink<PlacedObject>(FormKeyBinaryTranslation.Instance.Parse(frame)));
                            break;
                        default:
                            frame.Reader.Position -= subHeader.HeaderLength;
                            goto Finish;
                    }
                }
                Finish:
                
                // Check error conditions
                if (roomCount != (item.LinkedRooms?.Count ?? 0))
                {
                    throw new ArgumentException($"Unexpected room count: {item.LinkedRooms?.Count ?? 0} != {roomCount}");
                }
                if (EnumExt.HasFlag(flags, HasImageSpaceFlag) != (item.ImageSpace.FormKey != null))
                {
                    throw new ArgumentException($"Image space presence did not match flag specification");
                }
                if (EnumExt.HasFlag(flags, HasLightingTemplateFlag) != (item.LightingTemplate.FormKey != null))
                {
                    throw new ArgumentException($"Lighting template presence did not match flag specification");
                }
            }
        }

        public partial class PlacedObjectBinaryWriteTranslation
        {
            static partial void WriteBinaryBoundDataCustom(MutagenWriter writer, IPlacedObjectGetter item)
            {
                var lightingTemplate = item.LightingTemplate;
                var imageSpace = item.ImageSpace;
                var linkedRooms = item.LinkedRooms;
                var unknown2 = item.Unknown2;
                if (lightingTemplate.FormKey == null
                    && imageSpace.FormKey == null
                    && linkedRooms.Count == 0
                    && unknown2 == 0)
                {
                    return;
                }
                using (HeaderExport.ExportSubrecordHeader(writer, PlacedObject_Registration.XRMR_HEADER))
                {
                    writer.Write((byte)item.LinkedRooms.Count);
                    byte flags = 0;
                    if (lightingTemplate.FormKey != null)
                    {
                        flags = EnumExt.SetFlag(flags, PlacedObjectBinaryCreateTranslation.HasLightingTemplateFlag, true);
                    }
                    if (imageSpace.FormKey != null)
                    {
                        flags = EnumExt.SetFlag(flags, PlacedObjectBinaryCreateTranslation.HasImageSpaceFlag, true);
                    }
                    writer.Write(flags);
                    writer.Write(unknown2);
                }
                if (lightingTemplate.FormKey != null)
                {
                    FormKeyBinaryTranslation.Instance.Write(writer, lightingTemplate.FormKey.Value, PlacedObject_Registration.LNAM_HEADER);
                }
                if (imageSpace.FormKey != null)
                {
                    FormKeyBinaryTranslation.Instance.Write(writer, imageSpace.FormKey.Value, PlacedObject_Registration.INAM_HEADER);
                }
                foreach (var room in linkedRooms)
                {
                    FormKeyBinaryTranslation.Instance.Write(writer, room.FormKey, PlacedObject_Registration.XLRM_HEADER);
                }
            }
        }

        public partial class PlacedObjectBinaryOverlay
        {
            public short Unknown2 => throw new NotImplementedException();

            public IReadOnlyList<IFormLinkGetter<IPlacedObjectGetter>> LinkedRooms => throw new NotImplementedException();

            public IFormLinkNullableGetter<ILightGetter> LightingTemplate => throw new NotImplementedException();

            public IFormLinkNullableGetter<IImageSpaceAdapterGetter> ImageSpace => throw new NotImplementedException();

            partial void BoundDataCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                throw new NotImplementedException();
            }
        }
    }
}
