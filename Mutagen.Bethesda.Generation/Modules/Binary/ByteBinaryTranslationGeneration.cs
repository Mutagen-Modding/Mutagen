using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;

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
            var data = typeGen.GetFieldData();
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
            }            
            if (data.RecordType.HasValue)
            {
                if (dataType != null)
                {
                    throw new ArgumentException();
                }
                dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}.Span[_{typeGen.Name}Location] : default;");
            }
            else
            {
                if (dataType == null)
                {
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {dataAccessor}.Span[{currentPosition}];");
                }
                else
                {
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{dataType.GetFieldData().RecordType}Location.HasValue ? {dataAccessor}.Span[_{dataType.GetFieldData().RecordType}Location.Value + {currentPosition}] : default;");
                }
            }
        }
    }
}
