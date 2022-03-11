using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using StringType = Microsoft.VisualBasic.CompilerServices.StringType;

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
        FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor,
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
        
        using (var args = new ArgsWrapper(fg,
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
                    using (new BraceWrapper(gen))
                    {
                        var major = loqui != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                        if (major)
                        {
                            gen.AppendLine("try");
                        }
                        using (new BraceWrapper(gen, doIt: major))
                        {
                            await subTransl.GenerateWrite(
                                fg: gen,
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
                            using (new BraceWrapper(gen))
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
        FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor,
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
            fg.AppendLine("frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;");
        }

        WrapSet(fg, itemAccessor, arr2d, (wrapFg) =>
        {
            using (var args = new ArgsWrapper(wrapFg,
                $"{this.NamespacePrefix}Array2dBinaryTranslation<{arr2d.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>.Instance.Parse")
            {
                SemiColon = false,
            })
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
                            fg: subFg,
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
                                    fg: gen,
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
                                gen.AppendLine($"transl: (MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}, [MaybeNullWhen(false)] out {arr2d.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem{(needsRecordConv ? $", {nameof(TypedParseParams)}? translationParams" : null)}) =>");
                                using (new BraceWrapper(gen))
                                {
                                    subGen.GenerateCopyInRet(
                                        fg: gen,
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
                            gen.AppendLine($"transl: (MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}, [MaybeNullWhen(false)] out {arr2d.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem{(needsRecordConv ? $", {nameof(TypedParseParams)}? translationParams" : null)}) =>");
                            using (new BraceWrapper(gen))
                            {
                                gen.AppendLine("switch (header.TypeInt)");
                                using (new BraceWrapper(gen))
                                {
                                    foreach (var item in subGenTypes)
                                    {
                                        LoquiType specificLoqui = item.Value as LoquiType;
                                        if (specificLoqui.TargetObjectGeneration.Abstract) continue;
                                        foreach (var trigger in item.Key)
                                        {
                                            gen.AppendLine($"case 0x{trigger.TypeInt:X}: // {trigger.Type}");
                                        }
                                        LoquiType targetLoqui = arr2d.SubTypeGeneration as LoquiType;
                                        using (new BraceWrapper(gen))
                                        {
                                            subGen.GenerateCopyInRet(
                                                fg: gen,
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
                                    using (new DepthWrapper(gen))
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
    
    public void WrapSet(FileGeneration fg, Accessor accessor, ListType list, Action<FileGeneration> a)
    {
        if (list.Nullable)
        {
            fg.AppendLine($"{accessor} = ");
            using (new DepthWrapper(fg))
            {
                a(fg);
                fg.AppendLine(";");
            }
        }
        else
        {
            using (var args = new ArgsWrapper(fg,
                       $"{accessor}.SetTo"))
            {
                args.Add(subFg => a(subFg));
            }
        }
    }

    public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen,
        Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor,
        Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor, bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrapperFields(
        FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, 
        Accessor dataAccessor, int? passedLength, string passedLengthAccessor, DataType? dataType = null)
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
                    using (var args = new ArgsWrapper(fg,
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
                fg.AppendLine($"public {arr2d.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} {{ get; private set; }}");
            }
            else
            {
                using (var args = new ArgsWrapper(fg,
                           $"public {arr2d.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayArray2d.Factory<{typeName}>"))
                {
                    args.Add($"mem: {dataAccessor}.Slice({passedLength})");
                    args.Add($"package: _package");
                    args.Add($"itemLength: {subLen.Value}");
                    args.Add($"size: new P2Int({arr2d.FixedSize.Value.X}, {arr2d.FixedSize.Value.Y})");
                    args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, arr2d.SubTypeGeneration, "s", "p")}");
                }
            }
        }
    }

    public override async Task GenerateWrapperRecordTypeParse(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen,
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
            fg.AppendLine($"var subMeta = stream.ReadSubrecord();");
            dataAccess = "stream.RemainingMemory.Slice(0, subMeta.ContentLength)";
        }
        else
        {
            dataAccess = "stream.RemainingMemory";
        }
        
        using (var args = new ArgsWrapper(fg,
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