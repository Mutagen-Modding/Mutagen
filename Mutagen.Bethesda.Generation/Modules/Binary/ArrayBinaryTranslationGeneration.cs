using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class ArrayBinaryTranslationGeneration : ListBinaryTranslationGeneration
    {
        public override void GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor, 
            int? currentPosition)
        {
            ArrayType arr = typeGen as ArrayType;
            var data = arr.GetFieldData();
            if (data.BinaryOverlayFallback != BinaryGenerationType.Normal)
            {
                base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition);
                return;
            }
            var subGen = this.Module.GetTypeGeneration(arr.SubTypeGeneration.GetType());
            if (arr.FixedSize.HasValue)
            {
                if (arr.SubTypeGeneration is EnumType e)
                {
                    var posStr = currentPosition == null ? null : $"{currentPosition}";
                    fg.AppendLine($"public {arr.Interface(getter: true, internalInterface: true)} {typeGen.Name} => BinaryOverlayArrayHelper.EnumSliceFromFixedSize<{arr.SubTypeGeneration.TypeName(getter: true)}>({dataAccessor}.Slice({posStr}), amount: {arr.FixedSize.Value}, enumLength: {e.ByteLength});");
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            ArrayType arr = typeGen as ArrayType;
            if (arr.FixedSize.HasValue)
            {
                if (arr.HasBeenSet)
                {
                    throw new NotImplementedException();
                }
                else if (arr.SubTypeGeneration is EnumType e)
                {
                    return arr.FixedSize.Value * e.ByteLength;
                }
                else if (arr.SubTypeGeneration is LoquiType loqui
                    && this.Module.TryGetTypeGeneration(loqui.GetType(), out var loquiGen))
                {
                    return arr.FixedSize.Value * loquiGen.GetPassedAmount(objGen, loqui);
                }
                throw new NotImplementedException();
            }
            return null;
        }
    }
}
