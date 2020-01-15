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
            int? currentPosition,
            DataType dataType = null)
        {
            ArrayType arr = typeGen as ArrayType;
            var data = arr.GetFieldData();
            if (data.BinaryOverlayFallback != BinaryGenerationType.Normal)
            {
                base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, dataType);
                return;
            }
            var subGen = this.Module.GetTypeGeneration(arr.SubTypeGeneration.GetType());
            if (arr.FixedSize.HasValue)
            {
                var posStr = $"_{dataType.GetFieldData().RecordType}Location.Value + {currentPosition}";
                if (arr.SubTypeGeneration is EnumType e)
                {
                    fg.AppendLine($"public {arr.Interface(getter: true, internalInterface: true)} {typeGen.Name} => BinaryOverlayArrayHelper.EnumSliceFromFixedSize<{arr.SubTypeGeneration.TypeName(getter: true)}>(_{dataType.GetFieldData().RecordType}Location.HasValue ? {dataAccessor}.Slice({posStr}) : default, amount: {arr.FixedSize.Value}, enumLength: {e.ByteLength});");
                }
                else if (arr.SubTypeGeneration is LoquiType loqui)
                {
                    string recConverter = "null";
                    if (data?.RecordTypeConverter != null
                        && data.RecordTypeConverter.FromConversions.Count > 0)
                    {
                        recConverter = $"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter";
                    }
                    var gen = this.Module.GetTypeGeneration(loqui.GetType());
                    fg.AppendLine($"public {arr.Interface(getter: true, internalInterface: true)} {typeGen.Name} => BinaryOverlayArrayHelper.LoquiSliceFromFixedSize<{arr.SubTypeGeneration.TypeName(getter: true)}>(_{dataType.GetFieldData().RecordType}Location.HasValue ? {dataAccessor}.Slice({posStr}) : default, amount: {arr.FixedSize.Value}, length: {gen.ExpectedLength(objGen, loqui)}, _package, {recConverter}, {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory);");
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
