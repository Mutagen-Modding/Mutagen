using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDType : PrimitiveType
    {
        public override Type Type(bool getter) => typeof(FormID);

        public override void GenerateForEquals(FileGeneration fg, Accessor accessor, Accessor rhsAccessor)
        {
            if (!this.IntegrateField) return;
            fg.AppendLine($"if ({accessor}.FormKey != {rhsAccessor}.FormKey) return false;");
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            if (!this.IntegrateField) return;
            fg.AppendLine($"{retAccessor} = object.Equals({accessor}, {rhsAccessor});");
        }
    }
}
