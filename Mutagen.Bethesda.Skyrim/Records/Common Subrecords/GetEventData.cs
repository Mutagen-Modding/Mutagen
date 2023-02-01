using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Skyrim;

public partial class GetEventData
{
    Condition.Function IConditionDataGetter.Function => Condition.Function.GetEventData;
}

partial class GetEventDataBinaryCreateTranslation
{
    public static void FillEndingParams(MutagenFrame frame, IConditionData item)
    {
        item.RunOnType = EnumBinaryTranslation<Condition.RunOnType, MutagenFrame, MutagenWriter>.Instance.Parse(reader: frame.SpawnWithLength(4));
        item.Reference.SetTo(
            FormLinkBinaryTranslation.Instance.Parse(
                reader: frame,
                defaultVal: FormKey.Null));
        item.Unknown3 = frame.ReadInt32();
    }

    public static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IGetEventData item)
    {
        FillEndingParams(frame, item);
    }
}

partial class GetEventDataBinaryWriteTranslation
{
    public static void WriteCommonParams(MutagenWriter writer, IConditionDataGetter item)
    {
        EnumBinaryTranslation<Condition.RunOnType, MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            item.RunOnType,
            length: 4);
        FormLinkBinaryTranslation.Instance.Write(
            writer: writer,
            item: item.Reference);
        writer.Write(item.Unknown3);
    }

    public static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IGetEventDataGetter item)
    {
        WriteCommonParams(writer, item);
    }
}

partial class GetEventDataBinaryOverlay
{
    Condition.Function IConditionDataGetter.Function => Condition.Function.GetEventData;
}
