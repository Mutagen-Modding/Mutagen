using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Terminal
    {
        [Flags]
        public enum MajorFlag
        {
            HasDistantLod = 0x0000_8000,
            RandomAnimStart = 0x0001_0000
        }

        [Flags]
        public enum Flag : uint
        {
            AllowAwakeSound = 0x0040_0000,
            EnterWithWeaponDrawn = 0x0080_0000,
            PlayAnimWhenFull = 0x0100_0000,
            DisablesActivation = 0x0200_0000,
            IsPerch = 0x0400_0000,
            MustExitToTalk = 0x0800_0000,
            UseStaticAvoidNode = 0x1000_0000,
            HasModel = 0x4000_0000,
            IsSleepFurniture = 0x8000_0000
        }
    }

    namespace Internals
    {
        public partial class TerminalBinaryCreateTranslation
        {
            public const uint UpperFlagsMask = 0xFFC0_0000;
            public const uint NumSlots = 22;

            public static partial void FillBinaryLoopingSoundExportCustom(MutagenFrame frame, ITerminalInternal item)
            {
                // Let later custom logic parse
            }

            public static partial void FillBinaryFlagsCustom(MutagenFrame frame, ITerminalInternal item)
            {
                // Clear out old stuff
                // This assumes flags will be parsed first.  Might need to be upgraded to not need that assumption
                item.MarkerParameters = null;
                item.Flags = FillBinaryFlags(frame, (i) => GetNthMarker(item, i));
            }

            public static Terminal.Flag FillBinaryFlags(IMutagenReadStream stream, Func<int, FurnitureMarkerParameters> getter)
            {
                var subFrame = stream.ReadSubrecordFrame();
                uint raw = BinaryPrimitives.ReadUInt32LittleEndian(subFrame.Content);
                var ret = (Terminal.Flag)(raw & UpperFlagsMask);

                // Create marker objects for sit flags
                uint markers = raw & 0x003F_FFFF;
                uint indexToCheck = 1;
                for (int i = 0; i < NumSlots; i++)
                {
                    var has = EnumExt.HasFlag(markers, indexToCheck);
                    indexToCheck <<= 1;
                    if (!has) continue;
                    var marker = getter(i);
                    marker.Enabled = true;
                }
                return ret;
            }

            public static FurnitureMarkerParameters GetNthMarker(ITerminalInternal item, int index)
            {
                if (item.MarkerParameters == null)
                {
                    item.MarkerParameters = new ExtendedList<FurnitureMarkerParameters>();
                }
                if (!item.MarkerParameters.TryGet(index, out var marker))
                {
                    while (item.MarkerParameters.Count <= index)
                    {
                        item.MarkerParameters.Add(new FurnitureMarkerParameters());
                    }
                    marker = item.MarkerParameters[^1];
                }
                return marker;
            }

            public static partial void FillBinaryMarkerParametersCustom(MutagenFrame frame, ITerminalInternal item)
            {
                if (item.Flags != null)
                {
                    FillBinaryMarkers(frame, (i) => GetNthMarker(item, i));
                }
                else
                {
                    frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
                    item.LoopingSound.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader: frame));
                }
            }

            public static void FillBinaryMarkers(MutagenFrame stream, Func<int, FurnitureMarkerParameters> getter)
            {
                var snam = stream.ReadSubrecord(RecordTypes.SNAM);
                stream = stream.SpawnWithLength(snam.ContentLength);
                int i = 0;
                while (stream.Remaining > 0)
                {
                    var marker = getter(i++);
                    marker.CopyInFromBinary(stream, default);
                }
            }
        }

        public partial class TerminalBinaryWriteTranslation
        {
            public static partial void WriteBinaryLoopingSoundExportCustom(MutagenWriter writer, ITerminalGetter item)
            {
                FormLinkBinaryTranslation.Instance.WriteNullable(
                    writer: writer,
                    item: item.LoopingSound);
            }

            public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, ITerminalGetter item)
            {
                var flags = (uint)(item.Flags ?? 0);
                // Trim out lower flags
                var exportFlags = flags & FurnitureBinaryCreateTranslation.UpperFlagsMask;

                var markers = item.MarkerParameters;
                if (markers != null)
                {
                    // Enable appropriate sit markers
                    uint indexToCheck = 1;
                    foreach (var marker in markers)
                    {
                        exportFlags = EnumExt.SetFlag(exportFlags, indexToCheck, marker.Enabled);
                        indexToCheck <<= 1;
                    }
                }

                // Write out mashup of upper flags and sit markers
                using (HeaderExport.Subrecord(writer, RecordTypes.MNAM))
                {
                    writer.Write(exportFlags);
                }
            }

            public static partial void WriteBinaryMarkerParametersCustom(MutagenWriter writer, ITerminalGetter item)
            {
                Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFurnitureMarkerParametersGetter>.Instance.Write(
                    writer: writer,
                    items: item.MarkerParameters,
                    recordType: RecordTypes.SNAM,
                    transl: (MutagenWriter subWriter, IFurnitureMarkerParametersGetter subItem, TypedWriteParams? conv) =>
                    {
                        var Item = subItem;
                        ((FurnitureMarkerParametersBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                            item: Item,
                            writer: subWriter,
                            translationParams: conv);
                    });
            }
        }

        public partial class TerminalBinaryOverlay
        {
            Terminal.Flag? _flags;
            public partial Terminal.Flag? GetFlagsCustom() => _flags;

            private ExtendedList<FurnitureMarkerParameters>? _markers;
            public IReadOnlyList<IFurnitureMarkerParametersGetter>? MarkerParameters => _markers;

            private int? _LoopingSoundLocation;
            public IFormLinkNullableGetter<ISoundDescriptorGetter> LoopingSound => _LoopingSoundLocation.HasValue ? new FormLinkNullable<ISoundDescriptorGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_data, _LoopingSoundLocation.Value, _package.MetaData.Constants)))) : FormLinkNullable<ISoundDescriptorGetter>.Null;

            private FurnitureMarkerParameters GetNthMarker(int index)
            {
                if (this._markers == null)
                {
                    this._markers = new ExtendedList<FurnitureMarkerParameters>();
                }
                if (!this._markers.TryGet(index, out var marker))
                {
                    while (this._markers.Count <= index)
                    {
                        this._markers.Add(new FurnitureMarkerParameters());
                    }
                    marker = this._markers[^1];
                }
                return marker;
            }

            partial void MarkerParametersCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
            {
                if (_flags == null)
                {
                    _LoopingSoundLocation = (stream.Position - offset);
                }
                else
                {
                    TerminalBinaryCreateTranslation.FillBinaryMarkers(new MutagenFrame(stream), GetNthMarker);
                }
            }

            partial void FlagsCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                this._flags = TerminalBinaryCreateTranslation.FillBinaryFlags(
                    stream,
                    this.GetNthMarker);
            }
        }
    }
}
