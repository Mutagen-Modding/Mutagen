using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class DataBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
        public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            if (squashedRepeatedList)
            {
                throw new NotImplementedException();
            }
        }

        public override void GenerateWrite(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor writerAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrapperFields(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor, 
            int passedLength,
            DataType _)
        {
            DataType dataType = typeGen as DataType;
            
            fg.AppendLine($"private ushort? _{dataType.GetFieldData().RecordType}Location;");
            fg.AppendLine($"public {objGen.ObjectName}.{dataType.EnumName} {dataType.EnumName}State {{ get; private set; }}");

            var dataPassedLength = 0;
            foreach (var field in dataType.IterateFieldsWithMeta())
            {
                if (!this.Module.TryGetTypeGeneration(field.Field.GetType(), out var subTypeGen)) continue;
                using (new RegionWrapper(fg, field.Field.Name)
                {
                    AppendExtraLine = false,
                    SkipIfOnlyOneLine = true
                })
                {
                    var data = field.Field.GetFieldData();
                    if (data.Binary == BinaryGenerationType.Custom)
                    {
                        throw new NotImplementedException();
                    }
                    switch (data.Binary)
                    {
                        case BinaryGenerationType.DoNothing:
                        case BinaryGenerationType.NoGeneration:
                            continue;
                        default:
                            break;
                    }
                    subTypeGen.GenerateWrapperFields(
                        fg,
                        objGen,
                        field.Field,
                        dataAccessor,
                        dataPassedLength,
                        data: dataType);
                    dataPassedLength += subTypeGen.GetPassedAmount(objGen, field.Field);
                }
            }
        }

        public override async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen, 
            Accessor locationAccessor)
        {
            DataType data = typeGen as DataType;
            fg.AppendLine($"_{data.GetFieldData().RecordType}Location = (ushort){locationAccessor};");
        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;
    }
}
