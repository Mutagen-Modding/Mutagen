using Loqui;
using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public abstract class BinaryTranslationGeneration : TranslationGeneration
{
    public BinaryTranslationModule Module;
    public virtual string Namespace => "Mutagen.Bethesda.Plugins.Binary.Translations.";
    public virtual bool NeedsNamespacePrefix => true;
    public string NamespacePrefix => NeedsNamespacePrefix ? Namespace : string.Empty;
    public virtual bool DoErrorMasks => this.Module.DoErrorMasks;

    public delegate TryGet<string> ParamTest(
        ObjectGeneration objGen,
        TypeGeneration typeGen);
    public List<ParamTest> AdditionalWriteParams = new List<ParamTest>();
    public List<ParamTest> AdditionalCopyInParams = new List<ParamTest>();
    public List<ParamTest> AdditionalCopyInRetParams = new List<ParamTest>();
    public abstract Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen);

    public virtual bool AllowDirectWrite(
        ObjectGeneration objGen,
        TypeGeneration typeGen) => true;
    public virtual bool AllowDirectParse(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        bool squashedRepeatedList) => true;

    public abstract Task GenerateWrite(
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
        Accessor converterAccessor,
        bool inline);

    public virtual async Task GenerateWrapperFields(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        int? passedLength,
        string passedLengthAccessor,
        DataType data = null)
    {
    }

    public virtual async Task GenerateWrapperUnknownLengthParse(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        int? passedLength,
        string passedLengthAccessor)
    {
    }

    public virtual async Task GenerateWrapperCtor(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen)
    {
    }

    public virtual async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        var data = typeGen.GetFieldData();
        if (!data.HasTrigger)
        {
            return await this.ExpectedLength(objGen, typeGen);
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
                fg.AppendLine($"_{typeGen.Name}Location = {locationAccessor};");
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