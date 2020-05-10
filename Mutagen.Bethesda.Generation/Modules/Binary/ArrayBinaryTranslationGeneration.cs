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
        public override async Task GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor, 
            int? currentPosition,
            string passedLengthAccessor,
            DataType dataType = null)
        {
            ArrayType arr = typeGen as ArrayType;
            var data = arr.GetFieldData();
            if (data.BinaryOverlayFallback != BinaryGenerationType.Normal)
            {
                await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType);
                return;
            }
            var subGen = this.Module.GetTypeGeneration(arr.SubTypeGeneration.GetType());
            if (arr.FixedSize.HasValue)
            {
                if (arr.SubTypeGeneration is EnumType e)
                {
                    fg.AppendLine($"public {arr.Interface(getter: true, internalInterface: true)} {typeGen.Name} => BinaryOverlayArrayHelper.EnumSliceFromFixedSize<{arr.SubTypeGeneration.TypeName(getter: true)}>({dataAccessor}.Slice({passedLengthAccessor}), amount: {arr.FixedSize.Value}, enumLength: {e.ByteLength});");
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

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
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
                    return arr.FixedSize.Value * await loquiGen.GetPassedAmount(objGen, loqui);
                }
                throw new NotImplementedException();
            }
            return null;
        }
    }
}
