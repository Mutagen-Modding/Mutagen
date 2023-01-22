using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class FormLinkOrAliasBinaryTranslation
{
    public void Write<T>(
        MutagenWriter writer,
        IFormLinkOrAliasGetter<T> item)
        where T : class, IMajorRecordGetter
    {
        if (item.UsesAlias())
        {
            writer.Write(item.Alias);
        }
        else
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.Link.FormKey);
        }
    }
}