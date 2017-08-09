using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;

namespace Mutagen.Generation
{
    public abstract class BinaryTranslationModule : TranslationModule<BinaryTranslationGeneration>
    {
        public override string Namespace => "Mutagen.Binary.";

        public BinaryTranslationModule(LoquiGenerator gen)
            : base(gen)
        {
            this._typeGenerations[typeof(LoquiType)] = new LoquiBinaryTranslationGeneration(ModuleNickname);
            this._typeGenerations[typeof(BoolNullType)] = new PrimitiveBinaryTranslationGeneration<bool?>();
            this._typeGenerations[typeof(BoolType)] = new PrimitiveBinaryTranslationGeneration<bool>();
            this._typeGenerations[typeof(CharNullType)] = new PrimitiveBinaryTranslationGeneration<char?>();
            this._typeGenerations[typeof(CharType)] = new PrimitiveBinaryTranslationGeneration<char>();
            this._typeGenerations[typeof(DateTimeNullType)] = new PrimitiveBinaryTranslationGeneration<DateTime?>();
            this._typeGenerations[typeof(DateTimeType)] = new PrimitiveBinaryTranslationGeneration<DateTime>();
            this._typeGenerations[typeof(DoubleNullType)] = new PrimitiveBinaryTranslationGeneration<double?>();
            this._typeGenerations[typeof(DoubleType)] = new PrimitiveBinaryTranslationGeneration<double>();
            this._typeGenerations[typeof(EnumType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(EnumNullType)] = new EnumBinaryTranslationGeneration();
            this._typeGenerations[typeof(FloatNullType)] = new PrimitiveBinaryTranslationGeneration<float?>("Float");
            this._typeGenerations[typeof(FloatType)] = new PrimitiveBinaryTranslationGeneration<float>("Float");
            this._typeGenerations[typeof(Int8NullType)] = new PrimitiveBinaryTranslationGeneration<sbyte?>("Int8");
            this._typeGenerations[typeof(Int8Type)] = new PrimitiveBinaryTranslationGeneration<sbyte>("Int8");
            this._typeGenerations[typeof(Int16NullType)] = new PrimitiveBinaryTranslationGeneration<short?>();
            this._typeGenerations[typeof(Int16Type)] = new PrimitiveBinaryTranslationGeneration<short>();
            this._typeGenerations[typeof(Int32NullType)] = new PrimitiveBinaryTranslationGeneration<int?>();
            this._typeGenerations[typeof(Int32Type)] = new PrimitiveBinaryTranslationGeneration<int>();
            this._typeGenerations[typeof(Int64NullType)] = new PrimitiveBinaryTranslationGeneration<long?>();
            this._typeGenerations[typeof(Int64Type)] = new PrimitiveBinaryTranslationGeneration<long>();
            this._typeGenerations[typeof(StringType)] = new PrimitiveBinaryTranslationGeneration<string>(nullable: true) { CanBeNotNullable = false };
            this._typeGenerations[typeof(UInt8NullType)] = new PrimitiveBinaryTranslationGeneration<byte?>();
            this._typeGenerations[typeof(UInt8Type)] = new PrimitiveBinaryTranslationGeneration<byte>();
            this._typeGenerations[typeof(UInt16NullType)] = new PrimitiveBinaryTranslationGeneration<ushort?>();
            this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>();
            this._typeGenerations[typeof(UInt32NullType)] = new PrimitiveBinaryTranslationGeneration<uint?>();
            this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>();
            this._typeGenerations[typeof(UInt64NullType)] = new PrimitiveBinaryTranslationGeneration<ulong?>();
            this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>();
            this._typeGenerations[typeof(ListType)] = new ListBinaryTranslationGeneration();
            this._typeGenerations[typeof(ByteArrayType)] = new PrimitiveBinaryTranslationGeneration<byte[]>(typeName: "ByteArray", nullable: true);
        }

        public override void Load()
        {
            foreach (var gen in _typeGenerations.Values)
            {
                gen.Module = this;
                gen.MaskModule = this.Gen.MaskModule;
            }
        }

        public override IEnumerable<string> RequiredUsingStatements()
        {
            yield break;
        }

        public override IEnumerable<string> Interfaces(ObjectGeneration obj)
        {
            yield break;
        }

        public override IEnumerable<string> GetWriterInterfaces(ObjectGeneration obj)
        {
            yield break;
        }

        public override IEnumerable<string> GetReaderInterfaces(ObjectGeneration obj)
        {
            yield break;
        }

        public override void Modify(ObjectGeneration obj)
        {
        }

        public override void Modify(LoquiGenerator gen)
        {
        }

        public override void GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateRead(obj, fg);
            if (obj.IsTopClass)
            {
                using (var args = new FunctionWrapper(fg,
                    $"public void Write_{ModuleNickname}"))
                {
                    args.Add($"Stream stream");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{obj.ExtCommonName}.Write_{this.ModuleNickname}"))
                    {
                        args.Add("this");
                        args.Add("stream");
                    }
                }
                fg.AppendLine();
            }

            using (var args = new FunctionWrapper(fg,
                $"public void Write_{ModuleNickname}"))
            {
                args.Add($"Stream stream");
                args.Add($"out {obj.ErrorMask} errorMask");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{obj.ExtCommonName}.Write_{this.ModuleNickname}"))
                {
                    args.Add("this");
                    args.Add("stream");
                    args.Add("out errorMask");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public void Write_{ModuleNickname}"))
            {
                args.Add($"BinaryWriter writer");
                args.Add($"out {obj.ErrorMask} errorMask");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{obj.ExtCommonName}.Write_{this.ModuleNickname}"))
                {
                    args.Add($"writer: writer");
                    args.Add($"item: this");
                    args.Add($"doMasks: true");
                    args.Add($"errorMask: out errorMask");
                }
            }
            fg.AppendLine();

            if (obj.Abstract)
            {
                if (!obj.BaseClass?.Abstract ?? true)
                {
                    fg.AppendLine($"public abstract void Write_{ModuleNickname}(BinaryWriter writer);");
                    fg.AppendLine();
                }
            }
            else if (obj.IsTopClass
                || (!obj.Abstract && (obj.BaseClass?.Abstract ?? true)))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public{obj.FunctionOverride}void Write_{ModuleNickname}"))
                {
                    args.Add($"BinaryWriter writer");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{obj.ExtCommonName}.Write_{ModuleNickname}"))
                    {
                        args.Add($"writer: writer");
                        args.Add($"item: this");
                        args.Add($"doMasks: false");
                        args.Add($"errorMask: out {obj.ErrorMask} errorMask");
                    }
                }
                fg.AppendLine();
            }
        }

        public override void GenerateInCommonExt(ObjectGeneration obj, FileGeneration fg)
        {
            using (new RegionWrapper(fg, $"{ModuleNickname} Write"))
            {
                CommonWrite(obj, fg);
            }
        }

        private void CommonWrite(ObjectGeneration obj, FileGeneration fg)
        {
            using (var args = new FunctionWrapper(fg,
                $"public static void Write_{ModuleNickname}{obj.GenericTypes}",
                obj.GenerateWhereClauses().ToArray()))
            {
                args.Add($"{obj.Getter_InterfaceStr} item");
                args.Add($"Stream stream");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("using (var writer = new BinaryWriter(stream))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"Write_{this.ModuleNickname}(");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine($"writer: writer,");
                        fg.AppendLine($"item: item,");
                        fg.AppendLine($"doMasks: false,");
                        fg.AppendLine($"errorMask: out {obj.ErrorMask} errorMask);");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static void Write_{ModuleNickname}{obj.GenericTypes}",
                obj.GenerateWhereClauses().ToArray()))
            {
                args.Add($"{obj.Getter_InterfaceStr} item");
                args.Add($"Stream stream");
                args.Add($"out {obj.ErrorMask} errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("using (var writer = new BinaryWriter(stream))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"Write_{this.ModuleNickname}(");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine($"writer: writer,");
                        fg.AppendLine($"item: item,");
                        fg.AppendLine($"doMasks: true,");
                        fg.AppendLine($"errorMask: out errorMask);");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static void Write_{ModuleNickname}{obj.GenericTypes}",
                obj.GenerateWhereClauses().ToArray()))
            {
                args.Add($"BinaryWriter writer");
                args.Add($"{obj.Getter_InterfaceStr} item");
                args.Add($"bool doMasks");
                args.Add($"out {obj.ErrorMask} errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{obj.ErrorMask} errMaskRet = null;");
                using (var args = new ArgsWrapper(fg,
                    $"Write_{this.ModuleNickname}_Internal"))
                {
                    args.Add("writer: writer");
                    args.Add("item: item");
                    args.Add("doMasks: doMasks");
                    args.Add($"errorMask: doMasks ? () => errMaskRet ?? (errMaskRet = new {obj.ErrorMask}()) : default(Func<{obj.ErrorMask}>)");
                }
                fg.AppendLine($"errorMask = errMaskRet;");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"private static void Write_{ModuleNickname}_Internal{obj.GenericTypes}",
                obj.GenerateWhereClauses().ToArray()))
            {
                args.Add($"BinaryWriter writer");
                args.Add($"{obj.Getter_InterfaceStr} item");
                args.Add($"bool doMasks");
                args.Add($"Func<{obj.ErrorMask}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("try");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (field.Field.Derivative) continue;

                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }

                        if (field.Field.Notifying != NotifyingOption.None)
                        {
                            fg.AppendLine($"if (item.{field.Field.HasBeenSetAccessor})");
                        }
                        using (new BraceWrapper(fg))
                        {
                            var maskType = this.Gen.MaskModule.GetMaskModule(field.Field.GetType()).GetErrorMaskTypeStr(field.Field);
                            fg.AppendLine($"{maskType} subMask;");
                            generator.GenerateWrite(
                                fg: fg,
                                typeGen: field.Field,
                                writerAccessor: "writer",
                                itemAccessor: $"item.{field.Field.Name}",
                                doMaskAccessor: "doMasks",
                                maskAccessor: $"subMask");
                            fg.AppendLine("if (subMask != null)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"errorMask().{field.Field.Name} = subMask;");
                            }
                        }
                    }
                }
                fg.AppendLine("catch (Exception ex)");
                fg.AppendLine("when (doMasks)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("errorMask().Overall = ex;");
                }
            }
        }

        private void GenerateRead(ObjectGeneration obj, FileGeneration fg)
        {
            if (!obj.Abstract)
            {
                GenerateCreate(obj, fg);
            }

            if (obj is StructGeneration) return;
            fg.AppendLine($"public{obj.FunctionOverride}void CopyIn_{ModuleNickname}(BinaryReader reader, NotifyingFireParameters? cmds = null)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{this.Namespace}LoquiBinaryTranslation<{obj.ObjectName}, {obj.ErrorMask}>.Instance.CopyIn(");
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine($"reader: reader,");
                    fg.AppendLine($"item: this,");
                    fg.AppendLine($"skipProtected: true,");
                    fg.AppendLine($"doMasks: false,");
                    fg.AppendLine($"mask: out {obj.ErrorMask} errorMask,");
                    fg.AppendLine($"cmds: cmds);");
                }
            }
            fg.AppendLine();

            fg.AppendLine($"public virtual void CopyIn_{ModuleNickname}(BinaryReader reader, out {obj.ErrorMask} errorMask, NotifyingFireParameters? cmds = null)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{this.Namespace}LoquiBinaryTranslation<{obj.ObjectName}, {obj.ErrorMask}>.Instance.CopyIn(");
                using (new DepthWrapper(fg))
                {
                    fg.AppendLine($"reader: reader,");
                    fg.AppendLine($"item: this,");
                    fg.AppendLine($"skipProtected: true,");
                    fg.AppendLine($"doMasks: true,");
                    fg.AppendLine($"mask: out errorMask,");
                    fg.AppendLine($"cmds: cmds);");
                }
            }
            fg.AppendLine();

            foreach (var baseClass in obj.BaseClassTrail())
            {
                fg.AppendLine($"public override void CopyIn_{ModuleNickname}(BinaryReader reader, out {baseClass.ErrorMask} errorMask, NotifyingFireParameters? cmds = null)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"CopyIn_{this.ModuleNickname}(reader, out {obj.ErrorMask} errMask, cmds: cmds);");
                    fg.AppendLine("errorMask = errMask;");
                }
                fg.AppendLine();
            }
        }

        private void GenerateCreate(ObjectGeneration obj, FileGeneration fg)
        {
            fg.AppendLine($"public{obj.NewOverride}static {obj.ObjectName} Create_{ModuleNickname}(Stream stream)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"using (var reader = new BinaryReader(stream))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"return Create_{this.ModuleNickname}(reader);");
                }
            }
            fg.AppendLine();

            fg.AppendLine($"public{obj.NewOverride}static {obj.ObjectName} Create_{ModuleNickname}(BinaryReader reader)");
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return Create_{this.ModuleNickname}"))
                {
                    args.Add("reader: reader");
                    args.Add("doMasks: false");
                    args.Add("errorMask: out var errorMask");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static {obj.ObjectName} Create_{ModuleNickname}"))
            {
                args.Add("BinaryReader reader");
                args.Add($"out {obj.ErrorMask} errorMask");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return Create_{this.ModuleNickname}"))
                {
                    args.Add("reader: reader");
                    args.Add("doMasks: true");
                    args.Add("errorMask: out errorMask");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static {obj.ObjectName} Create_{ModuleNickname}"))
            {
                args.Add("BinaryReader reader");
                args.Add("bool doMasks");
                args.Add($"out {obj.ErrorMask} errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{obj.ErrorMask} errMaskRet = null;");
                using (var args = new ArgsWrapper(fg,
                    $"var ret = Create_{this.ModuleNickname}_Internal"))
                {
                    args.Add("reader: reader");
                    args.Add("doMasks: doMasks");
                    args.Add($"errorMask: doMasks ? () => errMaskRet ?? (errMaskRet = new {obj.ErrorMask}()) : default(Func<{obj.ErrorMask}>)");
                }
                fg.AppendLine($"errorMask = errMaskRet;");
                fg.AppendLine($"return ret;");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"private static {obj.ObjectName} Create_{ModuleNickname}_Internal"))
            {
                args.Add("BinaryReader reader");
                args.Add("bool doMasks");
                args.Add($"Func<{obj.ErrorMask}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var ret = new {obj.Name}{obj.GenericTypes}();");
                fg.AppendLine("try");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"foreach (var elem in EnumExt<{obj.FieldIndexName}>.Values)");
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"Fill_{this.ModuleNickname}_Internal"))
                        {
                            args.Add("item: ret");
                            args.Add("reader: reader");
                            args.Add("elem: elem");
                            args.Add("doMasks: doMasks");
                            args.Add("errorMask: errorMask");
                        }
                    }
                }
                fg.AppendLine("catch (Exception ex)");
                fg.AppendLine("when (doMasks)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("errorMask().Overall = ex;");
                }
                fg.AppendLine("return ret;");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"protected static void Fill_{ModuleNickname}_Internal"))
            {
                args.Add($"{obj.ObjectName} item");
                args.Add("BinaryReader reader");
                args.Add($"{obj.FieldIndexName} elem");
                args.Add("bool doMasks");
                args.Add($"Func<{obj.ErrorMask}> errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("switch (elem)");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }

                        fg.AppendLine($"case {obj.FieldIndexName}.{field.Field.Name}:");
                        using (new DepthWrapper(fg))
                        {
                            if (generator.ShouldGenerateCopyIn(field.Field))
                            {
                                using (new BraceWrapper(fg))
                                {
                                    var maskType = this.Gen.MaskModule.GetMaskModule(field.Field.GetType()).GetErrorMaskTypeStr(field.Field);
                                    fg.AppendLine($"{maskType} subMask;");
                                    generator.GenerateCopyIn(
                                        fg: fg,
                                        typeGen: field.Field,
                                        readerAccessor: "reader",
                                        itemAccessor: $"item.{field.Field.ProtectedName}",
                                        doMaskAccessor: "doMasks",
                                        maskAccessor: $"subMask");
                                    fg.AppendLine("if (subMask != null)");
                                    using (new BraceWrapper(fg))
                                    {
                                        fg.AppendLine($"errorMask().{field.Field.Name} = subMask;");
                                    }
                                }
                            }
                            fg.AppendLine("break;");
                        }
                    }

                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        if (obj.HasBaseObject)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{obj.BaseClassName}.Fill_{this.ModuleNickname}_Internal"))
                            {
                                args.Add("item: item");
                                args.Add("reader: reader");
                                args.Add("doMasks: doMasks");
                                args.Add("errorMask: errorMask");
                            }
                        }
                        fg.AppendLine("break;");
                    }
                }
            }
            fg.AppendLine();
        }

        public override void Generate(ObjectGeneration obj, FileGeneration fg)
        {
        }

        public override void GenerateInInterfaceGetter(ObjectGeneration obj, FileGeneration fg)
        {
        }
    }
}
