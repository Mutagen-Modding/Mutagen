using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class ByteBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<byte>
    {
        public ByteBinaryTranslationGeneration() 
            : base(expectedLen: 1)
        {
            CustomRead = (fg, reader, item) => fg.AppendLine($"{item.DirectAccess} = {reader.DirectAccess}.ReadUInt8();");
        }

        public override void GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor, 
            int currentPosition,
            DataType dataType)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            if (data.RecordType.HasValue
                || this.ExpectedLength == null)
            {
                throw new NotImplementedException();
            }
            if (dataType == null)
            {
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {dataAccessor}.Span[{currentPosition}];");
            }
            else
            {
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{dataType.GetFieldData().RecordType}Location.HasValue ? {dataAccessor}.Span[_{dataType.GetFieldData().RecordType}Location.Value + {currentPosition}] : default;");
            }
        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return this.ExpectedLength.Value;
        }
    }
}
