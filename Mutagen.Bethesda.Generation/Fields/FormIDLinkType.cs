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
        public LoquiType LoquiType { get; private set; }
        public FormIDTypeEnum FormIDType;
        public override bool HasProperty => true;
        public override string TypeName(bool getter) =>  $"I{(this.FormIDType == FormIDTypeEnum.Normal ? "FormID" : "EDID")}{(this.HasBeenSet ? "Set" : string.Empty)}Link{(getter ? "Getter" : null)}<{LoquiType.TypeName(getter)}>";
        public override Type Type(bool getter) => typeof(FormID);
        public string DirectTypeName(bool getter)
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
            return $"{linkString}{(this.HasBeenSet ? "Set" : string.Empty)}Link<{LoquiType.TypeName(getter: getter)}>";
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            LoquiType = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<LoquiType>();
            _rawFormID = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<FormIDType>();
            this.NotifyingProperty.OnNext(NotifyingType.ReactiveUI);
            this.ObjectCentralizedProperty.OnNext(false);
            LoquiType.SetObjectGeneration(this.ObjectGen, setDefaults: true);
            await LoquiType.Load(node, requireName: false);
            LoquiType.Name = this.Name;
            LoquiType.GetterInterfaceType = LoquiInterfaceType.IGetter;
            _rawFormID.Name = this.Name;
            this.NotifyingProperty.Subscribe(i => LoquiType.NotifyingProperty.OnNext(i));
            this.NotifyingProperty.Subscribe(i => _rawFormID.NotifyingProperty.OnNext(i));
            this.ObjectCentralizedProperty.Subscribe(i => LoquiType.ObjectCentralizedProperty.OnNext(i));
            this.ObjectCentralizedProperty.Subscribe(i => _rawFormID.ObjectCentralizedProperty.OnNext(i));
            this.HasBeenSetProperty.Subscribe(i => LoquiType.HasBeenSetProperty.OnNext(i));
            this.HasBeenSetProperty.Subscribe(i => _rawFormID.HasBeenSetProperty.OnNext(i));
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
            fg.AppendLine($"public {TypeName(getter: false)} {this.Property} {{ get; }} = new {DirectTypeName(getter: false)}();");
            fg.AppendLine($"public {LoquiType.TypeName(getter: false)} {this.Name} {{ get => {this.Property}.Item; {(this.ReadOnly ? string.Empty : $"set => {this.Property}.Item = value; ")}}}");
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"{this.TypeName(getter: false)} {this.ObjectGen.Interface(getter: false, internalInterface: this.InternalGetInterface)}.{this.Property} => this.{this.Property};");
            fg.AppendLine($"{LoquiType.TypeName(getter: true)} {this.ObjectGen.Interface(getter: true, internalInterface: this.InternalGetInterface)}.{this.Name} => this.{this.Property}.Item;");
            fg.AppendLine($"{this.TypeName(getter: true)} {this.ObjectGen.Interface(getter: true, internalInterface: this.InternalGetInterface)}.{this.Property} => this.{this.Property};");
        }

        public override string EqualsMaskAccessor(string accessor) => accessor;

        public override void GenerateForInterface(FileGeneration fg, bool getter, bool internalInterface)
        {
            if (getter)
            {
                fg.AppendLine($"{LoquiType.TypeName(getter: true)} {this.Name} {{ get; }}");
                fg.AppendLine($"{TypeName(getter: true)} {this.Property} {{ get; }}");
                fg.AppendLine();
            }
            else
            {
                fg.AppendLine($"new {LoquiType.TypeName(getter: false)} {this.Name} {{ get; set; }}");
                fg.AppendLine($"new {TypeName(getter: false)} {this.Property} {{ get; }}");
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
                    fg.AppendLine($"{identifier.DirectAccess} = default({LoquiType.TypeName(getter: false)});");
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
                    fg.AppendLine($"{identifier.DirectAccess} = default({LoquiType.TypeName(getter: false)});");
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
                    fg.AppendLine($"{identifier.DirectAccess} = default({LoquiType.TypeName(getter: false)});");
                }
            }
        }
    }
}
