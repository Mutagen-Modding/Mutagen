using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class ArmorAddonBinaryCreateTranslation
        {
            public static bool IsEnabled(byte b) => EnumExt.HasFlag(b, (byte)2);

            static partial void FillBinaryWeightSliderEnabledCustom(MutagenFrame frame, IArmorAddonInternal item)
            {
                item.WeightSliderEnabled = new GenderedItem<bool>(IsEnabled(frame.ReadUInt8()), IsEnabled(frame.ReadUInt8()));
            }

            static partial void FillBinaryBodyTemplateCustom(MutagenFrame frame, IArmorAddonInternal item)
            {
                item.BodyTemplate = BodyTemplateBinaryCreateTranslation.Parse(frame);
            }
        }

        public partial class ArmorAddonBinaryWriteTranslation
        {
            static partial void WriteBinaryWeightSliderEnabledCustom(MutagenWriter writer, IArmorAddonGetter item)
            {
                var weightSlider = item.WeightSliderEnabled;
                writer.Write(weightSlider.Male ? (byte)2 : default(byte));
                writer.Write(weightSlider.Female ? (byte)2 : default(byte));
            }

            static partial void WriteBinaryBodyTemplateCustom(MutagenWriter writer, IArmorAddonGetter item)
            {
                if (item.BodyTemplate.TryGet(out var templ))
                {
                    BodyTemplateBinaryWriteTranslation.Write(writer, templ);
                }
            }
        }

        public partial class ArmorAddonBinaryOverlay
        {
            public IGenderedItemGetter<Boolean> GetWeightSliderEnabledCustom() => new GenderedItem<bool>(
                ArmorAddonBinaryCreateTranslation.IsEnabled(_data.Slice(_DNAMLocation!.Value + 2)[0]),
                ArmorAddonBinaryCreateTranslation.IsEnabled(_data.Slice(_DNAMLocation!.Value + 3)[0]));

            private int? _BodyTemplateLocation;
            public IBodyTemplateGetter? GetBodyTemplateCustom() => _BodyTemplateLocation.HasValue ? BodyTemplateBinaryOverlay.CustomFactory(new OverlayStream(_data.Slice(_BodyTemplateLocation!.Value), _package), _package) : default;
            public bool BodyTemplate_IsSet => _BodyTemplateLocation.HasValue;

            partial void BodyTemplateCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _BodyTemplateLocation = (stream.Position - offset);
            }
        }
    }
}
