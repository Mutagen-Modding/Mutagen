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
            CustomRead = (fg, o, t, reader, item) =>
            {
                fg.AppendLine($"{item.DirectAccess} = {reader.DirectAccess}.ReadUInt8();");
                return true;
            };
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor, 
            int? currentPosition,
            string passedLengthAccessor)
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
                    await this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        currentPosition);
                    return;
                default:
                    throw new NotImplementedException();
            }
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                if (typeGen.CanBeNullable(getter: true))
                {
                    dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.HasBeenSet ? "?" : null)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}[0] : default(Byte{(typeGen.HasBeenSet ? "?" : null)});");
                }
                else
                {
                    fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
                    dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}[0] : default(Byte{(typeGen.HasBeenSet ? "?" : null)});");
                }
            }
            else
            {
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {dataAccessor}.Span[{passedLengthAccessor}];");
            }
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            return $"{dataAccessor}[0]";
        }
    }
}
