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
    public class FormIDLinkType : ClassType
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
        public override bool HasProperty => false;
        public override bool IsEnumerable => false;

        public override string TypeName(bool getter) => $"I{(this.FormIDType == FormIDTypeEnum.Normal ? "FormID" : "EDID")}{(this.HasBeenSet ? "Set" : string.Empty)}Link{(getter ? "Getter" : null)}<{LoquiType.TypeName(getter, internalInterface: true)}>";
        public override Type Type(bool getter) => typeof(FormID);
        public string DirectTypeName(bool getter, bool internalInterface = false)
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
            return $"{linkString}{(this.HasBeenSet ? "Set" : string.Empty)}Link<{LoquiType.TypeName(getter: getter, internalInterface: internalInterface)}>";
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            LoquiType = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<LoquiType>();
            _rawFormID = this.ObjectGen.ProtoGen.Gen.GetTypeGeneration<FormIDType>();
            LoquiType.SetObjectGeneration(this.ObjectGen, setDefaults: true);
            await LoquiType.Load(node, requireName: false);
            LoquiType.Name = this.Name;
            LoquiType.GetterInterfaceType = LoquiInterfaceType.IGetter;
            _rawFormID.Name = this.Name;
            _rawFormID.SetObjectGeneration(this.ObjectGen, false);
            this.NotifyingProperty.Subscribe(i => LoquiType.NotifyingProperty.OnNext(i));
            this.NotifyingProperty.Subscribe(i => _rawFormID.NotifyingProperty.OnNext(i));
            this.ObjectCentralizedProperty.Subscribe(i => LoquiType.ObjectCentralizedProperty.OnNext(i));
            this.ObjectCentralizedProperty.Subscribe(i => _rawFormID.ObjectCentralizedProperty.OnNext(i));
            this.HasBeenSetProperty.Subscribe(i => LoquiType.HasBeenSetProperty.OnNext(i));
            this.HasBeenSetProperty.Subscribe(i => _rawFormID.HasBeenSetProperty.OnNext(i));
            this.FormIDType = node.GetAttribute<FormIDTypeEnum>("type", defaultVal: FormIDTypeEnum.Normal);
            this.Singleton = SingletonLevel.Singleton;
            this.SetPermission = PermissionLevel.@private;
        }

        public override string GenerateEqualsSnippet(Accessor accessor, Accessor rhsAccessor, bool negate = false)
        {
            return $"{(negate ? "!" : null)}object.Equals({accessor.DirectAccess}, {rhsAccessor.DirectAccess})";
        }

        public override string EqualsMaskAccessor(string accessor) => accessor;

        public override string SkipCheck(string copyMaskAccessor, bool deepCopy)
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
            fg.AppendLine($"if (!{accessor.PropertyOrDirectAccess}.Equals({rhsAccessor.PropertyOrDirectAccess})) return false;");
        }

        public override void GenerateForCopy(FileGeneration fg, Accessor accessor, string rhsAccessorPrefix, string copyMaskAccessor, bool protectedMembers, bool getter)
        {
            if (this.HasBeenSet)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{accessor.PropertyOrDirectAccess}.{(getter ? "SetToFormKey" : "SetLink")}"))
                {
                    args.Add($"rhs: {rhsAccessorPrefix}.{this.GetName(false, property: false)}");
                }
            }
            else
            {
                if (getter)
                {
                    switch (this.FormIDType)
                    {
                        case FormIDTypeEnum.Normal:
                            fg.AppendLine($"{accessor.PropertyOrDirectAccess}.FormKey = {rhsAccessorPrefix}.{this.GetName(false, property: false)}.FormKey;");
                            break;
                        case FormIDTypeEnum.EDIDChars:
                            fg.AppendLine($"{accessor.PropertyOrDirectAccess}.EDID = {rhsAccessorPrefix}.{this.GetName(false, property: false)}.EDID;");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{accessor.PropertyOrDirectAccess}.SetLink"))
                    {
                        args.Add($"value: {rhsAccessorPrefix}.{this.GetName(false, property: false)}");
                    }
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
                        $"{identifier.PropertyOrDirectAccess}.Unset"))
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
            fg.AppendLine($"{identifier.PropertyOrDirectAccess}.Unset();");
        }

        public override void GenerateCopySetToConverter(FileGeneration fg)
        {
            switch (this.FormIDType)
            {
                case FormIDTypeEnum.Normal:
                    fg.AppendLine($"(r) => new {DirectTypeName(getter: false)}(r.FormKey)");
                    break;
                case FormIDTypeEnum.EDIDChars:
                    fg.AppendLine($"(r) => new {DirectTypeName(getter: false)}(r.EDID)");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateForClass(FileGeneration fg)
        {
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"protected {this.TypeName(getter: false)} _{this.Name} = {GetNewForNonNullable()};");
            fg.AppendLine($"public {this.TypeName(getter: false)} {this.Name} => this._{ this.Name};");
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"{this.TypeName(getter: true)} {this.ObjectGen.Interface(getter: true, this.InternalGetInterface)}.{this.Name} => this.{this.Name};");
        }

        public override void GenerateForInterface(FileGeneration fg, bool getter, bool internalInterface)
        {
            if (getter)
            {
                if (!ApplicableInterfaceField(getter, internalInterface)) return;
                fg.AppendLine($"{TypeName(getter: true)} {this.Name} {{ get; }}");
            }
            else
            {
                if (!ApplicableInterfaceField(getter, internalInterface)) return;
                fg.AppendLine($"new {TypeName(getter: false)} {this.Name} {{ get; }}");
            }
        }

        public override string HasBeenSetAccessor(Accessor accessor = null)
        {
            return $"{(accessor?.DirectAccess ?? $"this.{this.Name}")}.HasBeenSet";
        }
    }
}
