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
    public class FormKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
        public FormKeyBinaryTranslationGeneration()
            : base(expectedLen: 4)
        {
            this.PreferDirectTranslation = false;
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
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            if (data.RecordType.HasValue
                || await this.ExpectedLength(objGen, typeGen) == null)
            {
                return;
                throw new NotImplementedException();
            }
            var posStr = dataType == null ? $"{passedLengthAccessor}" : $"_{dataType.GetFieldData().RecordType}Location + {passedLengthAccessor}";
            fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => FormKeyBinaryTranslation.Instance.Parse({dataAccessor}.Span.Slice({posStr}, {(await this.ExpectedLength(objGen, typeGen)).Value}), this._package.MasterReferences!);");
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor.DirectAccess);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                args.Add($"translationMask: {translationMaskAccessor}");
            }
        }
    }
}
