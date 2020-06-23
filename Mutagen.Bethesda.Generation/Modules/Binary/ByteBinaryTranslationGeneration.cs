using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;

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
            string passedLengthAccessor,
            DataType dataType)
        {
            var data = typeGen.GetFieldData();
            switch (data.BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    await this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        currentPosition,
                        passedLengthAccessor,
                        dataType);
                    return;
                default:
                    throw new NotImplementedException();
            }
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                if (typeGen.CanBeNullable(getter: true))
                {
                    dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})";
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.HasBeenSet ? "?" : null)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}[0] : default(Byte{(typeGen.HasBeenSet ? "?" : null)});");
                }
                else
                {
                    fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
                    if (dataType != null) throw new ArgumentException();
                    dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})";
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {dataAccessor}[0] : default(Byte{(typeGen.HasBeenSet ? "?" : null)});");
                }
            }
            else
            {
                if (dataType == null)
                {
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {dataAccessor}.Span[{passedLengthAccessor ?? "0x0"}];");
                }
                else
                {
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, passedLengthAccessor);
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {dataAccessor}.Span[_{typeGen.Name}Location] : default;");
                }
            }
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            return $"{dataAccessor}[0]";
        }
    }
}
