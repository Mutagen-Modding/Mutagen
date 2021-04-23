using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Records;
using System;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDType : PrimitiveType
    {
        public override Type Type(bool getter) => typeof(FormID);

        public override void GenerateForEquals(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, Accessor maskAccessor)
        {
            if (!this.IntegrateField) return;
            fg.AppendLine($"if ({this.GetTranslationIfAccessor(maskAccessor)})");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"if (!{accessor}.Equals({rhsAccessor})) return false;");
            }
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            if (!this.IntegrateField) return;
            fg.AppendLine($"{retAccessor} = {accessor}.Equals({rhsAccessor});");
        }
    }
}
