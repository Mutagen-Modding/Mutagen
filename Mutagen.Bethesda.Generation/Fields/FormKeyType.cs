using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using System;

namespace Mutagen.Bethesda.Generation
{
    public class FormKeyType : PrimitiveType
    {
        public override Type Type(bool getter) => typeof(FormKey);

        public override string GetDefault(bool getter)
        {
            return "FormKey.Null";
        }
    }
}
