using System;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Generation.Fields;

public class ModKeyType : PrimitiveType
{
    public override Type Type(bool getter) => typeof(ModKey);
    public override bool IsClass => true;

    public override string GetDefault(bool getter)
    {
        return "ModKey.Null";
    }
}