using Loqui;
using Loqui.Generation;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
    public class PluginTranslationModule : BinaryTranslationModule
    {
        public override bool DoTranslationInterface(ObjectGeneration obj) => obj.GetObjectType() != ObjectType.Mod;

        public override bool DirectTranslationReference(ObjectGeneration obj) => obj.GetObjectType() == ObjectType.Mod;

        public override string TranslatorReference(ObjectGeneration obj, Accessor item)
        {
            if (obj.GetObjectType() != ObjectType.Mod)
            {
                return base.TranslatorReference(obj, item);
            }
            return $"{this.TranslationWriteClass(obj)}.Instance";
        }

        public PluginTranslationModule(LoquiGenerator gen)
            : base(gen)
        {
            this.DoErrorMasks = false;
            this.TranslationMaskParameter = false;
            this._typeGenerations[typeof(LoquiType)] = new LoquiBinaryTranslationGeneration(TranslationTerm);
            this._typeGenerations[typeof(BoolType)] = new BooleanBinaryTranslationGeneration();
            this._typeGenerations[typeof(CharType)] = new PrimitiveBinaryTranslationGeneration<char>(expectedLen: 1);
            this._typeGenerations[typeof(DateTimeType)] = new PrimitiveBinaryTranslationGeneration<DateTime>(expectedLen: null);
            this._typeGenerations[typeof(DoubleType)] = new PrimitiveBinaryTranslationGeneration<double>(expectedLen: 8);
            this._typeGenerations[typeof(EnumType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(FloatType)] = new FloatBinaryTranslationGeneration();
            this._typeGenerations[typeof(PercentType)] = new PercentBinaryTranslationGeneration();
            this._typeGenerations[typeof(Int8Type)] = new SByteBinaryTranslationGeneration();
            this._typeGenerations[typeof(Int16Type)] = new PrimitiveBinaryTranslationGeneration<short>(expectedLen: 2);
            this._typeGenerations[typeof(Int32Type)] = new PrimitiveBinaryTranslationGeneration<int>(expectedLen: 4);
            this._typeGenerations[typeof(Int64Type)] = new PrimitiveBinaryTranslationGeneration<long>(expectedLen: 8);
            this._typeGenerations[typeof(P3UInt16Type)] = new PointBinaryTranslationGeneration<P3UInt16>(expectedLen: 6);
            this._typeGenerations[typeof(P2FloatType)] = new PointBinaryTranslationGeneration<P2Float>(expectedLen: 8);
            this._typeGenerations[typeof(P3FloatType)] = new PointBinaryTranslationGeneration<P3Float>(expectedLen: 12);
            this._typeGenerations[typeof(P2Int32Type)] = new PointBinaryTranslationGeneration<P2Int>(expectedLen: 8);
            this._typeGenerations[typeof(P3IntType)] = new PointBinaryTranslationGeneration<P3Int>(expectedLen: 12);
            this._typeGenerations[typeof(P2Int16Type)] = new PointBinaryTranslationGeneration<P2Int16>(expectedLen: 4);
            this._typeGenerations[typeof(P3Int16Type)] = new PointBinaryTranslationGeneration<P3Int16>(expectedLen: 6);
            this._typeGenerations[typeof(P2FloatType)] = new PointBinaryTranslationGeneration<P2Float>(expectedLen: 8);
            this._typeGenerations[typeof(StringType)] = new StringBinaryTranslationGeneration()
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(FilePathType)] = new FilePathBinaryTranslationGeneration();
            this._typeGenerations[typeof(UInt8Type)] = new ByteBinaryTranslationGeneration();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>(expectedLen: 2);
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>(expectedLen: 4);
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>(expectedLen: 8);
            this._typeGenerations[typeof(FormIDType)] = new PrimitiveBinaryTranslationGeneration<FormID>(expectedLen: 4)
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(FormKeyType)] = new FormKeyBinaryTranslationGeneration();
            this._typeGenerations[typeof(ModKeyType)] = new ModKeyBinaryTranslationGeneration();
            this._typeGenerations[typeof(RecordTypeType)] = new RecordTypeBinaryTranslationGeneration();
            this._typeGenerations[typeof(FormLinkType)] = new FormLinkBinaryTranslationGeneration();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ArrayType)] = new ArrayBinaryTranslationGeneration();
            this._typeGenerations[typeof(DictType)] = new DictBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new ByteArrayBinaryTranslationGeneration();
            this._typeGenerations[typeof(BufferType)] = new BufferBinaryTranslationGeneration();
            this._typeGenerations[typeof(DataType)] = new DataBinaryTranslationGeneration();
            this._typeGenerations[typeof(ColorType)] = new ColorBinaryTranslationGeneration()
            {
                PreferDirectTranslation = false
            };
            this._typeGenerations[typeof(ZeroType)] = new ZeroBinaryTranslationGeneration();
            this._typeGenerations[typeof(NothingType)] = new NothingBinaryTranslationGeneration();
            this._typeGenerations[typeof(CustomLogic)] = new CustomLogicTranslationGeneration();
            this._typeGenerations[typeof(GenderedType)] = new GenderedTypeBinaryTranslationGeneration();
            this._typeGenerations[typeof(BreakType)] = new BreakBinaryTranslationGeneration();
            this._typeGenerations[typeof(MarkerType)] = new MarkerBinaryTranslationGeneration();
            APILine[] modAPILines = new APILine[]
            {
                new APILine(
                    nicknameKey: "GroupMask",
                    resolutionString: "GroupMask? importMask = null",
                    when: (obj, dir) => obj.GetObjectType() == ObjectType.Mod)
            };
            var modKey = new APILine(
                nicknameKey: "ModKey",
                resolutionString: "ModKey modKey",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var modKeyWriter = new APILine(
                nicknameKey: "ModKeyWriter",
                resolutionString: "ModKey modKey",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Reader) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var writeParamOptional = new APILine(
                nicknameKey: "WriteParamOptional",
                resolutionString: $"{nameof(BinaryWriteParameters)}? param = null",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Reader) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var stringsReadParamOptional = new APILine(
                nicknameKey: "StringsParamsOptional",
                resolutionString: $"{nameof(StringsReadParameters)}? stringsParam = null",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod
                        && obj.GetObjectData().UsesStringFiles;
                });
            var recordInfoCache = new APILine(
                nicknameKey: "RecordInfoCache",
                resolutionString: $"{nameof(RecordInfoCache)} infoCache",
                when: (obj, dir) =>
                {
                    if (dir != TranslationDirection.Reader) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var parallel = new APILine(
                nicknameKey: "Parallel",
                resolutionString: "bool parallel = true",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    return obj.GetObjectType() == ObjectType.Mod;
                });
            var gameRelease = new APILine(
                nicknameKey: "GameRelease",
                resolver: (obj) => $"{ModModule.ReleaseEnumName(obj)} release",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    if (obj.GetObjectType() != ObjectType.Mod) return false;
                    return obj.GetObjectData().GameReleaseOptions != null;
                });
            var recTypeConverter = new APILine(
                "RecordTypeConverter",
                $"{nameof(RecordTypeConverter)}? recordTypeConverter = null",
                when: (obj, dir) =>
                {
                    return obj.GetObjectType() != ObjectType.Mod;
                });
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine("MutagenWriter", "MutagenWriter writer") },
                    optionalAPI: modAPILines,
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(modKeyWriter, "modKey"),
                        CustomMethodAPI.FactoryPublic(recTypeConverter),
                        CustomMethodAPI.FactoryPublic(writeParamOptional),
                    }),
                readerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine("MutagenFrame", "MutagenFrame frame") },
                    optionalAPI: modAPILines,
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(gameRelease),
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(modKeyWriter, "modKey"),
                        CustomMethodAPI.FactoryPublic(recTypeConverter),
                        CustomMethodAPI.FactoryPublic(writeParamOptional),
                    }));
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Path", $"{nameof(ModPath)} path") },
                        customAPI: new CustomMethodAPI[]
                        {
                            CustomMethodAPI.FactoryPublic(gameRelease)
                        },
                        optionalAPI: writeParamOptional
                            .AsEnumerable()
                            .And(modAPILines)
                            .And(stringsReadParamOptional)
                            .And(parallel)
                            .ToArray()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromPathOut,
                        ConvertFromPathIn),
                    When = (o, d) => d == TranslationDirection.Reader && o.GetObjectType() == ObjectType.Mod
                });
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Path", $"string path") },
                        customAPI: new CustomMethodAPI[]
                        {
                            CustomMethodAPI.FactoryPublic(gameRelease)
                        },
                        optionalAPI: writeParamOptional
                            .AsEnumerable()
                            .And(modAPILines)
                            .And(stringsReadParamOptional)
                            .And(parallel)
                            .ToArray()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromPathOut,
                        ConvertFromPathIn),
                    When = (o, d) => d == TranslationDirection.Writer && o.GetObjectType() == ObjectType.Mod
                });
            this.MinorAPIs.Add(
                new TranslationModuleAPI(
                    new MethodAPI(
                        majorAPI: new APILine[] { new APILine("Stream", "Stream stream") },
                        customAPI: new CustomMethodAPI[]
                        {
                            CustomMethodAPI.FactoryPublic(modKey),
                            CustomMethodAPI.FactoryPublic(gameRelease),
                            CustomMethodAPI.FactoryPublic(recordInfoCache),
                            CustomMethodAPI.FactoryPublic(writeParamOptional),
                        },
                        optionalAPI: modAPILines
                            .And(parallel)
                            .ToArray()))
                {
                    Funnel = new TranslationFunnel(
                        this.MainAPI,
                        ConvertFromStreamOut,
                        ConvertFromStreamIn),
                    When = (o, _) => o.GetObjectType() == ObjectType.Mod
                });
            this.CustomLogic = new CustomLogicTranslationGeneration() { Module = this };
        }

        #region Minor API Translations
        private void ConvertFromStreamOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            var objData = obj.GetObjectData();
            string gameReleaseStr;
            if (objData.GameReleaseOptions == null)
            {
                gameReleaseStr = $"{nameof(GameRelease)}.{objData.GameCategory}";
            }
            else
            {
                gameReleaseStr = $"item.{ModModule.ReleaseEnumName(obj)}.ToGameRelease()";
            }
            fg.AppendLine("var modKey = item.ModKey;");
            using (var args = new ArgsWrapper(fg,
                $"using (var writer = new MutagenWriter",
                suffixLine: ")",
                semiColon: false))
            {
                args.AddPassArg("stream");
                args.Add($"new {nameof(WritingBundle)}({gameReleaseStr})");
                args.Add("dispose: false");
            }
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
            }
        }

        private void ConvertFromStreamIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("try");
            using (new BraceWrapper(fg))
            {
                string gameReleaseStr;
                if (obj.GetObjectData().GameReleaseOptions == null)
                {
                    gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    gameReleaseStr = $"release.ToGameRelease()";
                }
                fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(stream, modKey, {gameReleaseStr}))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var frame = new MutagenFrame(reader);");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = infoCache;");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                    internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
                }
            }
            fg.AppendLine("catch (Exception ex)"); 
            using (new BraceWrapper(fg)) 
            { 
                fg.AppendLine("throw RecordException.Enrich(ex, modKey);"); 
            } 
        }

        private void ConvertFromPathOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            var objData = obj.GetObjectData();
            string gameReleaseStr;
            if (objData.GameReleaseOptions == null)
            {
                gameReleaseStr = $"{nameof(GameRelease)}.{objData.GameCategory}";
            }
            else
            {
                gameReleaseStr = $"item.{ModModule.ReleaseEnumName(obj)}.ToGameRelease()";
            }

            fg.AppendLine($"param ??= {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)};");
            using (var args = new ArgsWrapper(fg,
                $"var modKey = param.{nameof(BinaryWriteParameters.RunMasterMatch)}"))
            {
                args.Add("mod: item");
                args.AddPassArg("path");
            }
            if (objData.UsesStringFiles)
            {
                fg.AppendLine("bool disposeStrings = param.StringsWriter == null;");
                fg.AppendLine($"var stringsWriter = param.StringsWriter ?? (EnumExt.HasFlag((int)item.ModHeader.Flags, (int)ModHeaderCommonFlag.Localized) ? new StringsWriter({gameReleaseStr}, modKey, Path.Combine(Path.GetDirectoryName(path)!, \"Strings\")) : null);");
            }
            fg.AppendLine($"var bundle = new {nameof(WritingBundle)}({gameReleaseStr})");
            using (var prop = new PropertyCtorWrapper(fg))
            {
                if (objData.UsesStringFiles)
                {
                    prop.Add($"{nameof(WritingBundle.StringsWriter)} = stringsWriter");
                }
                prop.Add($"{nameof(WritingBundle.CleanNulls)} = param.{nameof(BinaryWriteParameters.CleanNulls)}");
            }
            fg.AppendLine("using var memStream = new MemoryTributary();");
            using (var args = new ArgsWrapper(fg,
                $"using (var writer = new MutagenWriter",
                suffixLine: ")")
            {
                SemiColon = false
            })
            {
                args.Add("memStream");
                args.Add("bundle");
                args.Add("dispose: false");
            }
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
            }
            fg.AppendLine($"using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"memStream.Position = 0;");
                fg.AppendLine($"memStream.CopyTo(fs);");
            }
            if (objData.UsesStringFiles)
            {
                fg.AppendLine("if (disposeStrings)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("param.StringsWriter?.Dispose();");
                }
            }
        }

        private void ConvertFromPathIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("try");
            using (new BraceWrapper(fg))
            {
                string gameReleaseStr;
                if (obj.GetObjectData().GameReleaseOptions == null)
                {
                    gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    fg.AppendLine($"var gameRelease = release.ToGameRelease();");
                    gameReleaseStr = $"gameRelease";
                }
                fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var modKey = path.ModKey;");
                    fg.AppendLine("var frame = new MutagenFrame(reader);");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}));");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                    if (obj.GetObjectData().UsesStringFiles)
                    {
                        fg.AppendLine("if (reader.Remaining < 12)");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                        }
                        fg.AppendLine($"var flags = reader.GetInt32(offset: 8);");
                        fg.AppendLine($"if (EnumExt.HasFlag(flags, (int)ModHeaderCommonFlag.Localized))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"frame.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.StringsLookup)} = StringsFolderLookupOverlay.TypicalFactory({gameReleaseStr}, path.{nameof(ModPath.ModKey)}, Path.GetDirectoryName(path.{nameof(ModPath.Path)})!, stringsParam);");
                        }
                    }
                    internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
                }
            }
            fg.AppendLine("catch (Exception ex)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("throw RecordException.Enrich(ex, path.ModKey);");
            }
        }

        #endregion

        public override bool WantsTryCreateFromBinary(ObjectGeneration obj)
        {
            return base.WantsTryCreateFromBinary(obj) && obj.GetObjectType() != ObjectType.Mod;
        }

        protected override async Task GenerateBinaryOverlayCreates(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            var objData = obj.GetObjectData();
            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"ReadOnlyMemorySlice<byte> bytes");
                if (objData.GameReleaseOptions != null)
                {
                    args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                }
                args.Add($"ModKey modKey");
                if (objData.UsesStringFiles)
                {
                    args.Add($"{nameof(IStringsFolderLookup)}? stringsLookup = null");
                }
            }
            using (new BraceWrapper(fg))
            {
                string gameReleaseStr;
                if (objData.GameReleaseOptions == null)
                {
                    gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    gameReleaseStr = $"release.ToGameRelease()";
                }
                fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({gameReleaseStr}, new {nameof(MasterReferenceReader)}(modKey));");
                fg.AppendLine($"meta.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenMemoryReadStream)}(bytes, meta));");
                if (objData.UsesStringFiles)
                {
                    fg.AppendLine($"meta.{nameof(ParsingBundle.StringsLookup)} = stringsLookup;");
                }
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    args.Add(subFg =>
                    {
                        using (var subArgs = new FunctionWrapper(subFg,
                            $"stream: new {nameof(MutagenMemoryReadStream)}"))
                        {
                            subArgs.Add("data: bytes");
                            subArgs.Add($"metaData: meta");
                        }
                    });
                    if (objData.GameReleaseOptions != null)
                    {
                        args.AddPassArg("release");
                    }
                    args.AddPassArg("modKey");
                    args.Add("shouldDispose: false");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"{nameof(ModPath)} path");
                if (objData.GameReleaseOptions != null)
                {
                    args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                }
                if (objData.UsesStringFiles)
                {
                    args.Add($"{nameof(StringsReadParameters)}? stringsParam = null");
                }
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    args.AddPassArg("path");
                    if (objData.UsesStringFiles)
                    {
                        args.AddPassArg("stringsParam");
                    }
                    if (objData.GameReleaseOptions != null)
                    {
                        args.AddPassArg("release");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"{nameof(Stream)} stream");
                if (objData.GameReleaseOptions != null)
                {
                    args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                }
                args.Add($"ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    string gameReleaseStr;
                    if (obj.GetObjectData().GameReleaseOptions == null)
                    {
                        gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                    }
                    else
                    {
                        gameReleaseStr = $"release.ToGameRelease()";
                    }
                    args.Add($"stream: new {nameof(MutagenBinaryReadStream)}(stream, modKey, {gameReleaseStr})");
                    args.AddPassArg("modKey");
                    if (objData.GameReleaseOptions != null)
                    {
                        args.AddPassArg("release");
                    }
                    args.Add("shouldDispose: false");
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                using (var args = new FunctionWrapper(fg,
                    $"public{obj.NewOverride()}static {await this.ObjectReturn(obj, maskReturn: false)} {CreateFromPrefix}{TranslationTerm}"))
                {
                    foreach (var (API, Public) in this.MainAPI.ReaderAPI.IterateAPI(obj,
                        TranslationDirection.Reader,
                        this.DoErrorMasks ? new APILine(ErrorMaskKey, "ErrorMaskBuilder? errorMask") : null,
                        this.DoErrorMasks ? new APILine(TranslationMaskKey, $"{nameof(TranslationCrystal)}? translationMask", (o, i) => this.TranslationMaskParameter) : null))
                    {
                        args.Add(API.Result);
                    }
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("try");
                    using (new BraceWrapper(fg))
                    {
                        await GenerateNewSnippet(obj, fg);
                        using (var args = new ArgsWrapper(fg,
                            $"{Loqui.Generation.Utility.Await(await AsyncImport(obj))}{obj.CommonClassInstance("ret", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.{CopyInFromPrefix}{TranslationTerm}"))
                        {
                            args.Add("item: ret");
                            foreach (var arg in this.MainAPI.PassArgs(obj, TranslationDirection.Reader))
                            {
                                args.Add(arg);
                            }
                            foreach (var arg in this.MainAPI.InternalPassArgs(obj, TranslationDirection.Reader))
                            {
                                args.Add(arg);
                            }
                            if (this.DoErrorMasks)
                            {
                                args.AddPassArg("errorMask");
                            }
                            if (this.TranslationMaskParameter)
                            {
                                args.AddPassArg("translationMask");
                            }
                        }
                        fg.AppendLine("return ret;");
                    }
                    fg.AppendLine("catch (Exception ex)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("throw RecordException.Enrich(ex, modKey);");
                    }
                }
                fg.AppendLine();
            }
        }

        public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
        {
            await foreach (var u in base.RequiredUsingStatements(obj))
            {
                yield return u;
            }

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                yield return "Mutagen.Bethesda.Strings";
                yield return "Mutagen.Bethesda.Constants";
            }

            yield return "Mutagen.Bethesda.Records.Binary.Overlay";
        }
    }
}
