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
        private FormIDType _rawFormID = new FormIDType();
        private LoquiType loquiType = new LoquiType();
        public FormIDTypeEnum FormIDType;

        public override string TypeName => $"{(this.FormIDType == FormIDTypeEnum.Normal ? "FormID" : "EDID")}{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TypeName}>";

        public override Type Type => typeof(FormID);

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            this.NotifyingProperty.Set(true);
            loquiType.SetObjectGeneration(this.ObjectGen, setDefaults: true);
            loquiType.ParseRefNode(node);
            loquiType.Name = this.Name;
            _rawFormID.Name = this.Name;
            this.NotifyingProperty.Forward(loquiType.NotifyingProperty);
            this.NotifyingProperty.Forward(_rawFormID.NotifyingProperty);
            this.HasBeenSetProperty.Forward(loquiType.HasBeenSetProperty);
            this.HasBeenSetProperty.Forward(_rawFormID.HasBeenSetProperty);
            this.FormIDType = node.GetAttribute<FormIDTypeEnum>("type", defaultVal: FormIDTypeEnum.Normal);
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
            fg.AppendLine($"public {TypeName} {this.Property} {{ get; }} = new {linkString}{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TargetObjectGeneration.Name}>();");
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"public {loquiType.TypeName} {this.Name} {{ get => {this.Property}.Item; {(this.ReadOnly ? string.Empty : $"set => {this.Property}.Item = value; ")}}}");
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"{this.TypeName} {this.ObjectGen.Getter_InterfaceStr}.{this.Property} => this.{this.Property};");
        }

        public override void GenerateForGetterInterface(FileGeneration fg)
        {
            fg.AppendLine($"{loquiType.TypeName} {this.Name} {{ get; }}");
            fg.AppendLine($"{TypeName} {this.Property} {{ get; }}");
            fg.AppendLine();
        }

        public override string EqualsMaskAccessor(string accessor) => accessor;

        public override void GenerateForInterface(FileGeneration fg)
        {
            fg.AppendLine($"new {loquiType.TypeName} {this.Name} {{ get; set; }}");
        }

        public override string SkipCheck(string copyMaskAccessor)
        {
            return _rawFormID.SkipCheck(copyMaskAccessor);
        }

        public override void GenerateForHasBeenSetMaskGetter(FileGeneration fg, string accessor, string retAccessor)
        {
            _rawFormID.GenerateForHasBeenSetMaskGetter(
                fg: fg,
                accessor: accessor,
                retAccessor: retAccessor);
        }

        public override void GenerateForHasBeenSetCheck(FileGeneration fg, string accessor, string checkMaskAccessor)
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

        public override void GenerateForCopy(FileGeneration fg, string accessorPrefix, string rhsAccessorPrefix, string copyMaskAccessor, string defaultFallbackAccessor, string cmdsAccessor, bool protectedMembers)
        {
            _rawFormID.GenerateForCopy(
                fg: fg,
                accessorPrefix: accessorPrefix,
                rhsAccessorPrefix: rhsAccessorPrefix,
                copyMaskAccessor: copyMaskAccessor,
                defaultFallbackAccessor: defaultFallbackAccessor,
                cmdsAccessor: cmdsAccessor,
                protectedMembers: protectedMembers);
        }
    }
}
