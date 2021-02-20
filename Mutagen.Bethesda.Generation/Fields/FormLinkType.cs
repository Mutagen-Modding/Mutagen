using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;
using System.Reactive.Subjects;

namespace Mutagen.Bethesda.Generation
{
    public class FormLinkType : ClassType
    {
        public enum FormIDTypeEnum
        {
            Normal,
            EDIDChars
        }

        public override string ProtectedName => base.ProtectedName;
        private FormIDType _rawFormID;
        public MutagenLoquiType LoquiType { get; private set; }
        public FormIDTypeEnum FormIDType;
        public override bool IsEnumerable => false;
        public override bool CanBeNullable(bool getter) => false;
        public bool NeedCovariance => this.LoquiType.RefType == Loqui.Generation.LoquiType.LoquiRefType.Generic;

        public string ClassTypeStringPrefix => this.FormIDType switch
        {
            FormIDTypeEnum.Normal => "Form",
            FormIDTypeEnum.EDIDChars => "EDID",
            _ => throw new NotImplementedException(),
        };

        public string FormIDTypeString => this.FormIDType switch
        {
            FormIDTypeEnum.Normal when Nullable => "FormKeyNullable",
            FormIDTypeEnum.Normal => "FormKey",
            FormIDTypeEnum.EDIDChars => "EDID",
            _ => throw new NotImplementedException(),
        };

        public override string TypeName(bool getter, bool needsCovariance = false)
        {
            var doInterface = needsCovariance || (getter && this.LoquiType.RefType == Loqui.Generation.LoquiType.LoquiRefType.Generic);
            return $"{(doInterface ? "I" : null)}{DirectNonGenericTypeName()}{(doInterface && (needsCovariance || getter) ? "Getter" : null)}<{LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>";
        }

        public override Type Type(bool getter) => typeof(FormID);

        public string DirectTypeName(bool getter, bool internalInterface = false)
        {
            return $"{DirectNonGenericTypeName()}<{LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>";
        }

        public string DirectNonGenericTypeName()
        {
            return $"{ClassTypeStringPrefix}Link{(this.Nullable ? "Nullable" : string.Empty)}";
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            LoquiType = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<MutagenLoquiType>();
            _rawFormID = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<FormIDType>();
            LoquiType.SetObjectGeneration(this.ObjectGen, setDefaults: true);
            LoquiType.RequireInterfaceObject = false;
            await LoquiType.Load(node, requireName: false);
            LoquiType.Name = this.Name;
            LoquiType.GetterInterfaceType = LoquiInterfaceType.IGetter;
            _rawFormID.Name = this.Name;
            _rawFormID.SetObjectGeneration(this.ObjectGen, false);
            this.NotifyingProperty.Subscribe(i => LoquiType.NotifyingProperty.OnNext(i));
            this.NotifyingProperty.Subscribe(i => _rawFormID.NotifyingProperty.OnNext(i));
            this.NullableProperty.Subscribe(i => LoquiType.NullableProperty.OnNext(i));
            this.NullableProperty.Subscribe(i => _rawFormID.NullableProperty.OnNext(i));
            this.FormIDType = node.GetAttribute<FormIDTypeEnum>("type", defaultVal: FormIDTypeEnum.Normal);
            this.Singleton = true;
            this.SetPermission = PermissionLevel.@private;
        }

        public override string GenerateEqualsSnippet(Accessor accessor, Accessor rhsAccessor, bool negate = false)
        {
            return $"{(negate ? "!" : null)}object.Equals({accessor}, {rhsAccessor})";
        }

        public override string EqualsMaskAccessor(string accessor) => accessor;

        public override string SkipCheck(Accessor copyMaskAccessor, bool deepCopy)
        {
            return _rawFormID.SkipCheck(copyMaskAccessor, deepCopy);
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            _rawFormID.GenerateForEqualsMask(
                fg: fg,
                accessor: accessor,
                rhsAccessor: rhsAccessor,
                retAccessor: retAccessor);
        }

        public override string GetNewForNonNullable()
        {
            return $"new {DirectTypeName(getter: false, internalInterface: true)}()";
        }

        public override void GenerateForEquals(FileGeneration fg, Accessor accessor, Accessor rhsAccessor)
        {
            fg.AppendLine($"if (!{accessor}.Equals({rhsAccessor})) return false;");
        }

        public override void GenerateForCopy(FileGeneration fg, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
        {
            fg.AppendLine($"if ({(deepCopy ? this.GetTranslationIfAccessor(copyMaskAccessor) : this.SkipCheck(copyMaskAccessor, deepCopy))})");
            using (new BraceWrapper(fg))
            {
                if (this.Nullable
                    || deepCopy)
                {
                    fg.AppendLine($"{accessor} = new {DirectTypeName(getter: false)}({rhs}.{FormIDTypeString});");
                }
                else
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{accessor}.SetLink"))
                    {
                        args.Add($"value: {rhs}");
                    }
                }
            }
        }

        public override void GenerateToString(FileGeneration fg, string name, Accessor accessor, string fgAccessor)
        {
            if (!this.IntegrateField) return;
            fg.AppendLine($"fg.{nameof(FileGeneration.AppendItem)}({accessor}.{FormIDTypeString}{(string.IsNullOrWhiteSpace(this.Name) ? null : $", \"{this.Name}\"")});");
        }

        public override void GenerateUnsetNth(FileGeneration fg, Accessor identifier)
        {
            if (!this.IntegrateField) return;
            if (!this.ReadOnly)
            {
                if (this.Nullable)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{identifier}.Unset"))
                    {
                    }
                }
                else
                {
                    fg.AppendLine($"{identifier} = default({LoquiType.TypeName(getter: false)});");
                }
            }
            fg.AppendLine("break;");
        }

        public override void GenerateClear(FileGeneration fg, Accessor identifier)
        {
            if (this.ReadOnly || !this.IntegrateField) return;
            fg.AppendLine($"{identifier} = {DirectTypeName(getter: false)}.Null;");
        }

        public override void GenerateCopySetToConverter(FileGeneration fg)
        {
            fg.AppendLine($".Select(r => ({TypeName(getter: false, needsCovariance: true)})new {DirectTypeName(getter: false)}(r.{FormIDTypeString}))");
        }

        public override void GenerateForClass(FileGeneration fg)
        {
            fg.AppendLine($"public {this.TypeName(getter: false)} {this.Name} {{ get; set; }} = {GetNewForNonNullable()};");
            if (NeedCovariance)
            {
                fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                fg.AppendLine($"{this.TypeName(getter: true)} {this.ObjectGen.Interface(getter: true, this.InternalGetInterface)}.{this.Name} => this.{this.Name};");
            }
        }

        public override void GenerateForInterface(FileGeneration fg, bool getter, bool internalInterface)
        {
            if (getter)
            {
                if (!ApplicableInterfaceField(getter, internalInterface)) return;
                fg.AppendLine($"{TypeName(getter: true, needsCovariance: NeedCovariance)} {this.Name} {{ get; }}");
            }
            else
            {
                if (!ApplicableInterfaceField(getter, internalInterface)) return;
                fg.AppendLine($"new {TypeName(getter: false)} {this.Name} {{ get; set; }}");
            }
        }

        public override string NullableAccessor(bool getter, Accessor accessor = null)
        {
            return $"({accessor?.Access ?? $"this.{this.Name}"}.{FormIDTypeString} != null)";
        }

        public override string GetDuplicate(Accessor accessor)
        {
            return $"new {this.DirectTypeName(getter: false, internalInterface: true)}({accessor}.{FormIDTypeString})";
        }

        public override string GetDefault(bool getter)
        {
            if (this.Nullable) return $"FormLinkNullable<{LoquiType.TypeNameInternal(getter, internalInterface: true)}>.Null";
            return $"FormLink<{LoquiType.TypeNameInternal(getter, internalInterface: true)}>.Null";
        }
    }
}
