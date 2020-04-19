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
            int? currentPosition)
        {
            var data = typeGen.GetFieldData();
            switch (data.BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.DoNothing:
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        ref currentPosition);
                    return;
                default:
                    throw new NotImplementedException();
            }
            if (typeGen.HasBeenSet)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                if (typeGen.CanBeNullable(getter: true))
                {
                    dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)}? {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}[0] : default(Byte?);");
                }
                else
                {
                    fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
                    dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}[0] : default(Byte?);");
                }
            }
            else
            {
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {dataAccessor}.Span[0x{currentPosition:X}];");
            }
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            return $"{dataAccessor}[0]";
        }
    }
}
