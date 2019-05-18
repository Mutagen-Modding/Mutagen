using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkType : PrimitiveType
    {
        public enum FormIDTypeEnum
        {
            Normal,
            EDIDChars
        }

        public override string ProtectedProperty => base.Property;
        public override string ProtectedName => base.ProtectedName;
        private FormIDType _rawFormID;
        private LoquiType loquiType;
        public FormIDTypeEnum FormIDType;

        public override bool HasProperty => true;

        public override string TypeName => $"{(this.FormIDType == FormIDTypeEnum.Normal ? "FormID" : "EDID")}{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TypeName}>";

        public override Type Type => typeof(FormID);

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            loquiType = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<LoquiType>();
            _rawFormID = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<FormIDType>();
            this.NotifyingProperty.Set(NotifyingType.ReactiveUI);
            this.ObjectCentralizedProperty.Set(false);
            loquiType.SetObjectGeneration(this.ObjectGen, setDefaults: true);
            await loquiType.Load(node, requireName: false);
            loquiType.Name = this.Name;
            _rawFormID.Name = this.Name;
            this.NotifyingProperty.Forward(loquiType.NotifyingProperty);
            this.NotifyingProperty.Forward(_rawFormID.NotifyingProperty);
            this.ObjectCentralizedProperty.Forward(loquiType.ObjectCentralizedProperty);
            this.ObjectCentralizedProperty.Forward(_rawFormID.ObjectCentralizedProperty);
            this.HasBeenSetProperty.Forward(loquiType.HasBeenSetProperty);
            this.HasBeenSetProperty.Forward(_rawFormID.HasBeenSetProperty);
            this.FormIDType = node.GetAttribute<FormIDTypeEnum>("type", defaultVal: FormIDTypeEnum.Normal);
        }

        public override string GenerateEqualsSnippet(Accessor accessor, Accessor rhsAccessor, bool negate = false)
        {
            return $"{(negate ? "!" : null)}object.Equals({accessor.DirectAccess}, {rhsAccessor.DirectAccess})";
        }

        public override void GenerateForClass(FileGeneration fg)
        {
            string linkString;
            switch (this.FormIDType)
            {
                case FormIDTypeEnum.Normal:
                    linkString = "FormID";
                    break;
                case FormIDTypeEnum.EDIDChars:
                    linkString = "EDID";
                    break;
                default:
                    throw new NotImplementedException();
            }
            fg.AppendLine($"public {TypeName} {this.Property} {{ get; }} = new {linkString}{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TypeName}>();");
            fg.AppendLine($"public {loquiType.TypeName} {this.Name} {{ get => {this.Property}.Item; {(this.ReadOnly ? string.Empty : $"set => {this.Property}.Item = value; ")}}}");
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"{this.TypeName} {this.ObjectGen.Interface(getter: true)}.{this.Property} => this.{this.Property};");
        }

        public override string EqualsMaskAccessor(string accessor) => accessor;

        public override void GenerateForInterface(FileGeneration fg, bool getter, bool internalInterface)
        {
            if (getter)
            {
                fg.AppendLine($"{loquiType.TypeName} {this.Name} {{ get; }}");
                fg.AppendLine($"{TypeName} {this.Property} {{ get; }}");
                fg.AppendLine();
            }
            else
            {
                fg.AppendLine($"new {loquiType.TypeName} {this.Name} {{ get; set; }}");
            }
        }

        public override string SkipCheck(string copyMaskAccessor)
        {
            return _rawFormID.SkipCheck(copyMaskAccessor);
        }

        public override void GenerateForHasBeenSetMaskGetter(FileGeneration fg, Accessor accessor, string retAccessor)
        {
            _rawFormID.GenerateForHasBeenSetMaskGetter(
                fg: fg,
                accessor: accessor,
                retAccessor: retAccessor);
        }

        public override void GenerateForHasBeenSetCheck(FileGeneration fg, Accessor accessor, string checkMaskAccessor)
        {
            _rawFormID.GenerateForHasBeenSetCheck(
                fg: fg,
                accessor: accessor,
                checkMaskAccessor: checkMaskAccessor);
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            _rawFormID.GenerateForEqualsMask(
                fg: fg,
                accessor: accessor,
                rhsAccessor: rhsAccessor,
                retAccessor: retAccessor);
        }

        public override void GenerateForEquals(FileGeneration fg, Accessor accessor, Accessor rhsAccessor)
        {
            fg.AppendLine($"if (!{accessor.PropertyAccess}.Equals({rhsAccessor.PropertyAccess})) return false;");
        }

        public override void GenerateForCopy(FileGeneration fg, Accessor accessor, string rhsAccessorPrefix, string copyMaskAccessor, string defaultFallbackAccessor, bool protectedMembers)
        {
            if (this.HasBeenSet)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{accessor.PropertyAccess}.SetLink"))
                {
                    args.Add($"rhs: {rhsAccessorPrefix}.{this.GetName(false, property: true)}");
                    args.Add($"def: {defaultFallbackAccessor}?.{this.GetName(false, property: true)}");
                }
            }
            else
            {
                using (var args = new ArgsWrapper(fg,
                    $"{accessor.PropertyAccess}.SetLink"))
                {
                    args.Add($"value: {rhsAccessorPrefix}.{this.GetName(false, property: true)}");
                }
            }
        }

        public override void GenerateToString(FileGeneration fg, string name, Accessor accessor, string fgAccessor)
        {
            if (!this.IntegrateField) return;
            fg.AppendLine($"{fgAccessor}.AppendLine($\"{name} => {{{accessor.PropertyOrDirectAccess}}}\");");
        }

        public override void GenerateUnsetNth(FileGeneration fg, Accessor identifier)
        {
            if (!this.IntegrateField) return;
            if (!this.ReadOnly)
            {
                if (this.HasBeenSet)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{identifier.PropertyAccess}.Unset"))
                    {
                    }
                }
                else
                {
                    fg.AppendLine($"{identifier.DirectAccess} = default({loquiType.TypeName});");
                }
            }
            fg.AppendLine("break;");
        }

        public override void GenerateClear(FileGeneration fg, Accessor identifier)
        {
            if (this.ReadOnly || !this.IntegrateField) return;
            if (this.NotifyingType != NotifyingType.None)
            {
                if (this.HasBeenSet)
                {
                    fg.AppendLine($"{identifier.PropertyAccess}.Unset();");
                }
                else
                {
                    fg.AppendLine($"{identifier.DirectAccess} = default({loquiType.TypeName});");
                }
            }
            else
            {
                if (this.HasBeenSet)
                {
                    fg.AppendLine($"{identifier.PropertyAccess}.Unset();");
                }
                else
                {
                    fg.AppendLine($"{identifier.DirectAccess} = default({loquiType.TypeName});");
                }
            }
        }
    }
}
