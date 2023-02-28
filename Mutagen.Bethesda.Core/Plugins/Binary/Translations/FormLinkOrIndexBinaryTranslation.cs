using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class FormLinkOrIndexBinaryTranslation
{
    public static FormLinkOrIndexBinaryTranslation Instance = new();
    
    public void Write<T>(
        MutagenWriter writer,
        IFormLinkOrIndexGetter<T> item)
        where T : class, IMajorRecordGetter
    {
        if (item.UsesAlias() || item.UsesPackageData())
        {
            writer.Write(item.Index ?? 0);
        }
        else
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.Link.FormKey);
        }
    }

    public void ParseInto<T>(
        MutagenFrame reader,
        IFormLinkOrIndex<T> item)
        where T : class, IMajorRecordGetter
    {
        item.Index = reader.GetUInt32();
        item.Link.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader));
    }
}