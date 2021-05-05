using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Pex
{
    public class PexListBinaryTranslationGeneration : ListBinaryTranslationGeneration
    {
        public override string Namespace => "Mutagen.Bethesda.Translations.Binary.";

        public override async Task GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor)
        {
            var list = typeGen as ListType;
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var data = typeGen.GetFieldData();
            if (data.MarkerType.HasValue)
            {
                fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {objGen.RecordTypeHeaderName(data.MarkerType.Value)})) {{ }}");
            }

            var subData = list.SubTypeGeneration.GetFieldData();

            var allowDirectWrite = subTransl.AllowDirectWrite(objGen, list.SubTypeGeneration);
            var loqui = list.SubTypeGeneration as LoquiType;

            var typeName = $"{list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}{list.SubTypeGeneration.NullChar}";
            if (loqui != null)
            {
                typeName = loqui.TypeNameInternal(getter: true, internalInterface: true);
            }

            string suffix = string.Empty;

            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{Module.WriterClass}, {Module.ReaderClass}, {typeName}>.Instance.Write{suffix}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"items: {GetWriteAccessor(itemAccessor)}");
                args.Add("countLengthLength: 2");
                if (this.Module.TranslationMaskParameter)
                {
                    args.Add($"translationMask: {translationMaskAccessor}");
                }
                if (allowDirectWrite)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(list.SubTypeGeneration, getter: true)}.Write");
                }
                else
                {
                    await args.Add(async (gen) =>
                    {
                        var listTranslMask = this.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetTranslationMaskTypeStr(list.SubTypeGeneration);
                        gen.AppendLine($"transl: ({this.Module.WriterClass} subWriter, {typeName} subItem) =>");
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
                                    typeGen: list.SubTypeGeneration,
                                    writerAccessor: "subWriter",
                                    translationAccessor: "listTranslMask",
                                    itemAccessor: new Accessor($"subItem"),
                                    errorMaskAccessor: null,
                                    converterAccessor: null);
                            }
                            if (major)
                            {
                                gen.AppendLine("catch (Exception ex)");
                                using (new BraceWrapper(gen))
                                {
                                    gen.AppendLine("throw RecordException.Factory(ex, subItem);");
                                }
                            }
                        }
                    });
                }
            }
        }

        public override async Task GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var list = typeGen as ListType;
            var data = list.GetFieldData();
            var subData = list.SubTypeGeneration.GetFieldData();
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var isAsync = subTransl.IsAsync(list.SubTypeGeneration, read: true);

            WrapSet(fg, itemAccessor, list, (wrapFg) =>
            {
                using (var args = new ArgsWrapper(wrapFg,
                    $"{(isAsync ? "(" : null)}{Loqui.Generation.Utility.Await(isAsync)}{this.Namespace}List{(isAsync ? "Async" : null)}BinaryTranslation<{Module.WriterClass}, {Module.ReaderClass}, {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>.Instance.Parse",
                    suffixLine: $"{Loqui.Generation.Utility.ConfigAwait(isAsync)}{(isAsync ? ")" : null)}")
                {
                    SemiColon = false,
                })
                {
                    args.Add($"reader: {Module.ReaderMemberName}");
                    if (list is ArrayType arr
                        && arr.FixedSize.HasValue)
                    {
                        args.Add($"amount: {arr.FixedSize.Value}");
                    }
                    else
                    {
                        args.Add($"amount: {Module.ReaderMemberName}.ReadUInt16()");
                    }
                    var subGenTypes = subData.GenerationTypes.ToList();
                    var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                    if (subGenTypes.Count <= 1
                        && subTransl.AllowDirectParse(
                            objGen,
                            typeGen: list.SubTypeGeneration,
                            squashedRepeatedList: false))
                    {
                        args.Add(subFg =>
                        {
                            subGen.GenerateCopyInRet(
                                fg: subFg,
                                objGen: objGen,
                                targetGen: list.SubTypeGeneration,
                                typeGen: list.SubTypeGeneration,
                                readerAccessor: "r",
                                translationAccessor: "listTranslMask",
                                retAccessor: "transl: ",
                                outItemAccessor: new Accessor("listSubItem"),
                                asyncMode: isAsync ? AsyncMode.Async : AsyncMode.Off,
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
                                    typeGen: list.SubTypeGeneration,
                                    squashedRepeatedList: false))
                                {
                                    subGen.GenerateCopyInRet(
                                        fg: gen,
                                        objGen: objGen,
                                        targetGen: list.SubTypeGeneration,
                                        typeGen: list.SubTypeGeneration,
                                        readerAccessor: "r",
                                        translationAccessor: "listTranslMask",
                                        retAccessor: "transl: ",
                                        outItemAccessor: new Accessor("listSubItem"),
                                        asyncMode: isAsync ? AsyncMode.Async : AsyncMode.Off,
                                        errorMaskAccessor: "listErrMask",
                                        converterAccessor: "conv",
                                        inline: true);
                                }
                                else
                                {
                                    gen.AppendLine($"transl: {Loqui.Generation.Utility.Async(isAsync)}(MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}{(isAsync ? null : $", out {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem")}) =>");
                                    using (new BraceWrapper(gen))
                                    {
                                        subGen.GenerateCopyInRet(
                                            fg: gen,
                                            objGen: objGen,
                                            targetGen: list.SubTypeGeneration,
                                            typeGen: list.SubTypeGeneration,
                                            readerAccessor: "r",
                                            translationAccessor: "listTranslMask",
                                            retAccessor: "return ",
                                            outItemAccessor: new Accessor("listSubItem"),
                                            asyncMode: isAsync ? AsyncMode.Async : AsyncMode.Off,
                                            errorMaskAccessor: "listErrMask",
                                            converterAccessor: "conv",
                                            inline: false);
                                    }
                                }
                            }
                            else
                            {
                                gen.AppendLine($"transl: {Loqui.Generation.Utility.Async(isAsync)}(MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}{(isAsync ? null : $", out {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem")}) =>");
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
                                            LoquiType targetLoqui = list.SubTypeGeneration as LoquiType;
                                            using (new BraceWrapper(gen))
                                            {
                                                subGen.GenerateCopyInRet(
                                                    fg: gen,
                                                    objGen: objGen,
                                                    targetGen: list.SubTypeGeneration,
                                                    typeGen: item.Value,
                                                    readerAccessor: "r",
                                                    translationAccessor: "listTranslMask",
                                                    retAccessor: "return ",
                                                    outItemAccessor: new Accessor("listSubItem"),
                                                    asyncMode: AsyncMode.Async,
                                                    errorMaskAccessor: $"listErrMask",
                                                    converterAccessor: "conv",
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
                    fg.AppendLine($".CastExtendedList<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>();");
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
    }
}
