using Loqui;
using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public abstract class BinaryTranslationGeneration : TranslationGeneration
    {
        public BinaryTranslationModule Module;
        public string Namespace => Module.Namespace;
        public virtual bool DoErrorMasks => this.Module.DoErrorMasks;

        public delegate TryGet<string> ParamTest(
            ObjectGeneration objGen,
            TypeGeneration typeGen);
        public List<ParamTest> AdditionalWriteParams = new List<ParamTest>();
        public List<ParamTest> AdditionalCopyInParams = new List<ParamTest>();
        public List<ParamTest> AdditionalCopyInRetParams = new List<ParamTest>();
        public abstract int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen);

        public virtual bool AllowDirectWrite(
            ObjectGeneration objGen,
            TypeGeneration typeGen) => true;
        public virtual bool AllowDirectParse(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            bool squashedRepeatedList) => true;

        public abstract void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor);

        public abstract string GetTranslatorInstance(TypeGeneration typeGen, bool getter);

        public abstract Task GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor);

        public abstract void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor);

        public virtual void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? passedLength)
        {
        }

        public virtual void GenerateWrapperCtor(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen)
        {
        }

        public virtual int? GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            var data = typeGen.GetFieldData();
            if (!data.HasTrigger)
            {
                return this.ExpectedLength(objGen, typeGen) ?? 0;
            }
            return null;
        }

        public virtual async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor locationAccessor,
            Accessor packageAccessor,
            Accessor converterAccessor)
        {
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    fg.AppendLine($"_{typeGen.Name}Location = (ushort){locationAccessor};");
                    break;
                case BinaryGenerationType.Custom:
                    using (var args = new ArgsWrapper(fg,
                        $"{typeGen.Name}CustomParse"))
                    {
                        args.AddPassArg($"stream");
                        args.AddPassArg($"finalPos");
                        args.AddPassArg($"offset");
                    }
                    break;
                case BinaryGenerationType.DoNothing:
                case BinaryGenerationType.NoGeneration:
                default:
                    return;
            }
        }

        public virtual string GenerateForTypicalWrapper(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            throw new NotImplementedException();
        }
    }
}
