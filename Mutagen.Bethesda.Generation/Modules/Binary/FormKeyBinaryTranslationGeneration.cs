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
            this.AdditionalWriteParams.Add(AdditionalParam);
            this.AdditionalCopyInParams.Add(AdditionalParam);
            this.AdditionalCopyInRetParams.Add(AdditionalParam);
            this.PreferDirectTranslation = false;
        }

        private static TryGet<string> AdditionalParam(
           ObjectGeneration objGen,
           TypeGeneration typeGen)
        {
            return TryGet<string>.Succeed("masterReferences: masterReferences");
        }

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition,
            DataType dataType)
        {
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            if (data.RecordType.HasValue
                || this.ExpectedLength(objGen, typeGen) == null)
            {
                return;
                throw new NotImplementedException();
            }
            var posStr = dataType == null ? $"{currentPosition}" : $"_{dataType.GetFieldData().RecordType}Location + {currentPosition}";
            fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => FormKeyBinaryTranslation.Instance.Parse({dataAccessor}.Span.Slice({posStr}, {this.ExpectedLength(objGen, typeGen).Value}), this._package.MasterReferences!);");
        }
    }
}
