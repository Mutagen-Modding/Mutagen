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
                if (roomCount != item.LinkedRooms.Count)
                {
                    throw new ArgumentException($"Unexpected room count: {item.LinkedRooms.Count} != {roomCount}");
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
            int? _boundDataLoc;

            public short Unknown2 => _boundDataLoc.HasValue ? BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(_boundDataLoc.Value + 8)) : default(short);

            public IReadOnlyList<IFormLink<IPlacedObjectGetter>> LinkedRooms { get; private set; } = ListExt.Empty<IFormLink<IPlacedObjectGetter>>();

            int? _lightingTemplateLoc;
            public IFormLinkNullable<ILightGetter> LightingTemplate => _lightingTemplateLoc.HasValue ? new FormLinkNullable<ILightGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordSpan(_data, _lightingTemplateLoc.Value, _package.MetaData.Constants)))) : FormLinkNullable<ILightGetter>.Null;

            int? _imageSpaceLoc;
            public IFormLinkNullable<IImageSpaceAdapterGetter> ImageSpace => _imageSpaceLoc.HasValue ? new FormLinkNullable<IImageSpaceAdapterGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordSpan(_data, _imageSpaceLoc.Value, _package.MetaData.Constants)))) : FormLinkNullable<IImageSpaceAdapterGetter>.Null;

            partial void BoundDataCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                _boundDataLoc = stream.Position - offset;
                var header = _package.MetaData.Constants.ReadSubrecordFrame(stream);
                if (header.Content.Length != 4)
                {
                    throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
                }
                var roomCount = header.Content[0];
                var flags = header.Content[1];
                while (_package.MetaData.Constants.TryGetSubrecord(stream, out var subHeader))
                {
                    switch (subHeader.RecordTypeInt)
                    {
                        case 0x4D414E4C: // LNAM
                            _lightingTemplateLoc = stream.Position - offset;
                            if (!EnumExt.HasFlag(flags, PlacedObjectBinaryCreateTranslation.HasLightingTemplateFlag))
                            {
                                throw new ArgumentException($"Lighting template presence did not match flag specification");
                            }
                            stream.Position += subHeader.TotalLength;
                            break;
                        case 0x4D414E49: // INAM
                            _imageSpaceLoc = stream.Position - offset;
                            if (!EnumExt.HasFlag(flags, PlacedObjectBinaryCreateTranslation.HasImageSpaceFlag))
                            {
                                throw new ArgumentException($"Image space presence did not match flag specification");
                            }
                            stream.Position += subHeader.TotalLength;
                            break;
                        case 0x4D524C58: // XLRM
                            LinkedRooms = BinaryOverlayList<IFormLink<IPlacedObjectGetter>>.FactoryByArray(
                                stream.RemainingMemory,
                                _package,
                                (s, p) => new FormLink<IPlacedObjectGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))),
                                locs: ParseRecordLocations(
                                    stream: stream,
                                    finalPos: stream.Length,
                                    constants: _package.MetaData.Constants.SubConstants,
                                    trigger: subHeader.RecordType,
                                    skipHeader: true));
                            if (roomCount != LinkedRooms.Count)
                            {
                                throw new ArgumentException($"Unexpected room count: {LinkedRooms.Count} != {roomCount}");
                            }
                            break;
                        default:
                            return;
                    }
                }
            }
        }
    }
}
