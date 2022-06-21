using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class Array2dBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        Array2dType arr2d = typeGen as Array2dType;
        if (!Module.TryGetTypeGeneration(arr2d.SubTypeGeneration.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + arr2d.SubTypeGeneration);
        }

        var subLen = await subTransl.ExpectedLength(objGen, arr2d.SubTypeGeneration);
        if (subLen == null) return null;
        return arr2d.FixedSize.Value.X * arr2d.FixedSize.Value.Y * subLen.Value;
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor,
        Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
    {
        var arr2d = typeGen as Array2dType;
        var data = typeGen.GetFieldData();
        if (!this.Module.TryGetTypeGeneration(arr2d.SubTypeGeneration.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + arr2d.SubTypeGeneration);
        }
        var loqui = arr2d.SubTypeGeneration as LoquiType;
        var typeName = arr2d.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        var allowDirectWrite = subTransl.AllowDirectWrite(objGen, arr2d.SubTypeGeneration);
        if (loqui != null)
        {
            typeName = loqui.TypeNameInternal(getter: true, internalInterface: true);
        }
        bool needsMasters = arr2d.SubTypeGeneration is FormLinkType || arr2d.SubTypeGeneration is LoquiType;
        
        using (var args = sb.Call(
            $"{this.NamespacePrefix}Array2dBinaryTranslation<{typeName}>.Instance.Write"))
        {
            args.Add($"writer: {writerAccessor}");
            args.Add($"items: {itemAccessor.Access}");
            if (data.RecordType != null)
            {
                args.Add($"recordType: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
            }
            if (allowDirectWrite)
            {
                args.Add($"transl: {subTransl.GetTranslatorInstance(arr2d.SubTypeGeneration, getter: true)}.Write");
            }
            else
            {
                await args.Add(async (gen) =>
                {
                    var listTranslMask = this.MaskModule.GetMaskModule(arr2d.SubTypeGeneration.GetType()).GetTranslationMaskTypeStr(arr2d.SubTypeGeneration);
                    gen.AppendLine($"transl: ({nameof(MutagenWriter)} subWriter, {typeName} subItem{(needsMasters ? $", {nameof(TypedWriteParams)}? conv" : null)}) =>");
                    using (gen.CurlyBrace())
                    {
                        var major = loqui != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                        if (major)
                        {
                            gen.AppendLine("try");
                        }
                        using (gen.CurlyBrace(doIt: major))
                        {
                            await subTransl.GenerateWrite(
                                sb: gen,
                                objGen: objGen,
                                typeGen: arr2d.SubTypeGeneration,
                                writerAccessor: "subWriter",
                                translationAccessor: "listTranslMask",
                                itemAccessor: new Accessor($"subItem"),
                                errorMaskAccessor: null,
                                converterAccessor: needsMasters ? "conv" : null);
                        }
                        if (major)
                        {
                            gen.AppendLine("catch (Exception ex)");
                            using (gen.CurlyBrace())
                            {
                                gen.AppendLine("throw RecordException.Enrich(ex, subItem);");
                            }
                        }
                    }
                });
            }
        }
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        var arr2d = typeGen as Array2dType;
        if (!Module.TryGetTypeGeneration(arr2d.SubTypeGeneration.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + arr2d.SubTypeGeneration);
        }

        var subMaskStr = subTransl.MaskModule.GetMaskModule(arr2d.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(arr2d.SubTypeGeneration);
        return $"Array2dBinaryTranslation{arr2d.SubTypeGeneration.TypeName(getter, needsCovariance: true)}, {subMaskStr}>.Instance";
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor,
        Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
    {
        var arr2d = typeGen as Array2dType;
        var data = arr2d.GetFieldData();
        var subData = arr2d.SubTypeGeneration.GetFieldData();
        if (!this.Module.TryGetTypeGeneration(arr2d.SubTypeGeneration.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + arr2d.SubTypeGeneration);
        }

        bool needsRecordConv = arr2d.SubTypeGeneration.NeedsRecordConverter();

        if (data.HasTrigger)
        {
            sb.AppendLine("frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;");
        }

        WrapSet(sb, itemAccessor, arr2d, (wrapFg) =>
        {
            using (var args = wrapFg.Call(
                $"{this.NamespacePrefix}Array2dBinaryTranslation<{arr2d.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>.Instance.Parse",
                semiColon: false))
            {
                args.Add($"reader: {Module.ReaderMemberName}");
                args.Add($"size: new P2Int({arr2d.FixedSize.Value.X}, {arr2d.FixedSize.Value.Y})");
                if (needsRecordConv)
                {
                    args.AddPassArg($"translationParams");
                }
                var subGenTypes = subData.GenerationTypes.ToList();
                var subGen = this.Module.GetTypeGeneration(arr2d.SubTypeGeneration.GetType());
                if (subGenTypes.Count <= 1
                    && subTransl.AllowDirectParse(
                        objGen,
                        typeGen: arr2d.SubTypeGeneration,
                        squashedRepeatedList: false))
                {
                    args.Add(subFg =>
                    {
                        subGen.GenerateCopyInRet(
                            sb: subFg,
                            objGen: objGen,
                            targetGen: arr2d.SubTypeGeneration,
                            typeGen: arr2d.SubTypeGeneration,
                            readerAccessor: "r",
                            translationAccessor: "listTranslMask",
                            retAccessor: "transl: ",
                            outItemAccessor: new Accessor("listSubItem"),
                            asyncMode: AsyncMode.Off,
                            errorMaskAccessor: "listErrMask",
                            converterAccessor: "conv",
                            inline: true);
                    });
                }
                else
                {
                    args.Add((gen) =>
                    {
                        if (subGenTypes.All(s => s.Value is LoquiType))
                        {
                            var subGenObjs = subGenTypes.Select(x => ((LoquiType)x.Value).TargetObjectGeneration).ToHashSet();
                            subGenTypes = subGenTypes.Where(s =>
                            {
                                return !((LoquiType)s.Value).TargetObjectGeneration.BaseClassTrail().Any(b =>
                                {
                                    if (!subGenObjs.Contains(b)) return false;
                                    var lookup = subGenTypes.FirstOrDefault(l => ((LoquiType)l.Value).TargetObjectGeneration == b);
                                    return s.Key.SequenceEqual(lookup.Key);
                                });
                            }).ToList();
                        }
                        if (subGenTypes.Count <= 1)
                        {
                            if (subGen.AllowDirectParse(
                                objGen: objGen,
                                typeGen: arr2d.SubTypeGeneration,
                                squashedRepeatedList: false))
                            {
                                subGen.GenerateCopyInRet(
                                    sb: gen,
                                    objGen: objGen,
                                    targetGen: arr2d.SubTypeGeneration,
                                    typeGen: arr2d.SubTypeGeneration,
                                    readerAccessor: "r",
                                    translationAccessor: "listTranslMask",
                                    retAccessor: "transl: ",
                                    outItemAccessor: new Accessor("listSubItem"),
                                    asyncMode: AsyncMode.Off,
                                    errorMaskAccessor: "listErrMask",
                                    converterAccessor: "conv",
                                    inline: true);
                            }
                            else
                            {
                                gen.AppendLine($"transl: (MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}, [MaybeNullWhen(false)] out {arr2d.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem{(needsRecordConv ? $", {nameof(TypedParseParams)} translationParams" : null)}) =>");
                                using (gen.CurlyBrace())
                                {
                                    subGen.GenerateCopyInRet(
                                        sb: gen,
                                        objGen: objGen,
                                        targetGen: arr2d.SubTypeGeneration,
                                        typeGen: arr2d.SubTypeGeneration,
                                        readerAccessor: "r",
                                        translationAccessor: "listTranslMask",
                                        retAccessor: "return ",
                                        outItemAccessor: new Accessor("listSubItem"),
                                        asyncMode: AsyncMode.Off,
                                        errorMaskAccessor: "listErrMask",
                                        converterAccessor: "conv",
                                        inline: false);
                                }
                            }
                        }
                        else
                        {
                            gen.AppendLine($"transl: (MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}, [MaybeNullWhen(false)] out {arr2d.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem{(needsRecordConv ? $", {nameof(TypedParseParams)} translationParams" : null)}) =>");
                            using (gen.CurlyBrace())
                            {
                                gen.AppendLine("switch (header.TypeInt)");
                                using (gen.CurlyBrace())
                                {
                                    foreach (var item in subGenTypes)
                                    {
                                        LoquiType specificLoqui = item.Value as LoquiType;
                                        if (specificLoqui.TargetObjectGeneration.Abstract) continue;
                                        foreach (var trigger in item.Key)
                                        {
                                            gen.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                        }
                                        LoquiType targetLoqui = arr2d.SubTypeGeneration as LoquiType;
                                        using (gen.CurlyBrace())
                                        {
                                            subGen.GenerateCopyInRet(
                                                sb: gen,
                                                objGen: objGen,
                                                targetGen: arr2d.SubTypeGeneration,
                                                typeGen: item.Value,
                                                readerAccessor: "r",
                                                translationAccessor: "listTranslMask",
                                                retAccessor: "return ",
                                                outItemAccessor: new Accessor("listSubItem"),
                                                asyncMode: AsyncMode.Async,
                                                errorMaskAccessor: $"listErrMask",
                                                converterAccessor: "translationParams",
                                                inline: false);
                                        }
                                    }
                                    gen.AppendLine("default:");
                                    using (gen.IncreaseDepth())
                                    {
                                        gen.AppendLine("throw new NotImplementedException();");
                                    }
                                }
                            }
                        }
                    });
                }
            }
        });
    }
    
    public void WrapSet(StructuredStringBuilder sb, Accessor accessor, ListType list, Action<StructuredStringBuilder> a)
    {
        if (list.Nullable)
        {
            sb.AppendLine($"{accessor} = ");
            using (sb.IncreaseDepth())
            {
                a(sb);
                sb.AppendLine(";");
            }
        }
        else
        {
            using (var args = sb.Call(
                       $"{accessor}.SetTo"))
            {
                args.Add(subFg => a(subFg));
            }
        }
    }

    public override void GenerateCopyInRet(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen,
        Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor,
        Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor, bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        Accessor structDataAccessor, 
        Accessor recordDataAccessor, 
        int? passedLength,
        string passedLengthAccessor,
        DataType? dataType = null)
    {
        Array2dType arr2d = typeGen as Array2dType;
        var data = arr2d.GetFieldData();
        switch (data.BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
                return;
            case BinaryGenerationType.Custom:
                if (typeGen.GetFieldData().HasTrigger)
                {
                    using (var args = sb.Call(
                        $"partial void {typeGen.Name}CustomParse"))
                    {
                        args.Add($"{nameof(OverlayStream)} stream");
                        args.Add($"long finalPos");
                        args.Add($"int offset");
                        args.Add($"{nameof(RecordType)} type");
                        args.Add($"{nameof(PreviousParse)} lastParsed");
                    }
                }
                return;
            default:
                throw new NotImplementedException();
        }
        var subGen = this.Module.GetTypeGeneration(arr2d.SubTypeGeneration.GetType());
        var subData = arr2d.SubTypeGeneration.GetFieldData();
        var subLen = await subGen.ExpectedLength(objGen, arr2d.SubTypeGeneration);
        string typeName;
        LoquiType loqui = arr2d.SubTypeGeneration as LoquiType;
        if (loqui != null)
        {
            typeName = this.Module.BinaryOverlayClassName(loqui);
        }
        else
        {
            typeName = arr2d.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        }
        
        if (arr2d.SubTypeGeneration is LoquiType)
        {
            throw new NotImplementedException();
        }
        else
        {
            if (typeGen.Nullable)
            {
                sb.AppendLine($"public {arr2d.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} {{ get; private set; }}");
            }
            else
            {
                using (var args = sb.Call(
                           $"public {arr2d.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayArray2d.Factory<{typeName}>"))
                {
                    args.Add($"mem: {structDataAccessor}.Slice({passedLength})");
                    args.Add($"package: _package");
                    args.Add($"itemLength: {subLen.Value}");
                    args.Add($"size: new P2Int({arr2d.FixedSize.Value.X}, {arr2d.FixedSize.Value.Y})");
                    args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, arr2d.SubTypeGeneration, "s", "p")}");
                }
            }
        }
    }

    public override async Task GenerateWrapperRecordTypeParse(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen,
        Accessor locationAccessor, Accessor packageAccessor, Accessor converterAccessor)
    {
        Array2dType arr2d = typeGen as Array2dType;
        var subGen = this.Module.GetTypeGeneration(arr2d.SubTypeGeneration.GetType());
        var subLen = await subGen.ExpectedLength(objGen, arr2d.SubTypeGeneration);
        if (subLen == null)
        {
            throw new ArgumentException();
        }
        string typeName;
        LoquiType loqui = arr2d.SubTypeGeneration as LoquiType;
        if (loqui != null)
        {
            typeName = this.Module.BinaryOverlayClassName(loqui);
        }
        else
        {
            typeName = arr2d.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        }

        string dataAccess;
        if (arr2d.GetFieldData().HasTrigger)
        {
            sb.AppendLine($"var subMeta = stream.ReadSubrecordHeader();");
            dataAccess = "stream.RemainingMemory.Slice(0, subMeta.ContentLength)";
        }
        else
        {
            dataAccess = "stream.RemainingMemory";
        }
        
        using (var args = sb.Call(
                   $"this.{typeGen.Name} = BinaryOverlayArray2d.Factory<{typeName}>"))
        {
            args.Add($"mem: {dataAccess}");
            args.Add($"package: _package");
            args.Add($"itemLength: {subLen.Value}");
            args.Add($"size: new P2Int({arr2d.FixedSize.Value.X}, {arr2d.FixedSize.Value.Y})");
            args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, arr2d.SubTypeGeneration, "s", "p")}");
        }
    }
}