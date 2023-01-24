using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class FormLinkOrAliasBinaryTranslation
{
    public static FormLinkOrAliasBinaryTranslation Instance = new();
    
    public void Write<T>(
        MutagenWriter writer,
        IFormLinkOrAliasGetter<T> item)
        where T : class, IMajorRecordGetter
    {
        if (item.UsesAlias())
        {
            writer.Write(item.Alias ?? 0);
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
        IFormLinkOrAlias<T> item)
        where T : class, IMajorRecordGetter
    {
        item.Alias = reader.GetUInt32();
        item.Link.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader));
    }
}