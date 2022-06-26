using Loqui.Generation;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Generation.Fields;

public class FormKeyType : PrimitiveType
{
    public override Type Type(bool getter) => typeof(FormKey);

    public override string GetDefault(bool getter)
    {
        return "FormKey.Null";
    }
}