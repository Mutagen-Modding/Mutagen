using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Fields;

public class GuidType : PrimitiveType
{
    public override Type Type(bool getter)
    {
        return typeof(Guid);
    }
}