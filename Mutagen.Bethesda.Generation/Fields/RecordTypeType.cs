using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using System;

namespace Mutagen.Bethesda.Generation
{
    public class RecordTypeType : PrimitiveType
    {
        public override Type Type(bool getter) => typeof(RecordType);
        public override bool IsClass => false;

        public override string GetDefault(bool getter)
        {
            if (this.Nullable)
            {
                return $"default(RecordType?)";
            }
            else
            {
                return "RecordType.Null";
            }
        }
    }
}
