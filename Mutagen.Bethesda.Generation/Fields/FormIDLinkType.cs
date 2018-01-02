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
        public override string ProtectedProperty => base.Property;
        public override string ProtectedName => base.ProtectedName;
        private RawFormIDType RawFormID = new RawFormIDType();
        private LoquiType loquiType = new LoquiType();

        public override string TypeName => $"FormID{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TypeName}>";

        public override Type Type => typeof(RawFormID);

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            this.NotifyingProperty.Set(true);
            loquiType.SetObjectGeneration(this.ObjectGen, setDefaults: true);
            loquiType.ParseRefNode(node);
            loquiType.Name = this.Name;
            RawFormID.Name = this.Name;
            this.NotifyingProperty.Forward(loquiType.NotifyingProperty);
            this.NotifyingProperty.Forward(RawFormID.NotifyingProperty);
            this.HasBeenSetProperty.Forward(loquiType.HasBeenSetProperty);
            this.HasBeenSetProperty.Forward(RawFormID.HasBeenSetProperty);
        }

        public override void GenerateForClass(FileGeneration fg)
        {
            fg.AppendLine($"public FormID{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TypeName}> {this.Property} {{ get; }} = new FormID{(this.HasBeenSet ? "Set" : string.Empty)}Link<{loquiType.TargetObjectGeneration.Name}>();");
            fg.AppendLine($"public {loquiType.TypeName} {this.Name} {{ get => {this.Property}.Item; {(this.Protected ? string.Empty : $"set => {this.Property}.Item = value; ")}}}");
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
            return RawFormID.SkipCheck(copyMaskAccessor);
        }

        public override void GenerateForHasBeenSetMaskGetter(FileGeneration fg, string accessor, string retAccessor)
        {
            RawFormID.GenerateForHasBeenSetMaskGetter(
                fg: fg,
                accessor: accessor,
                retAccessor: retAccessor);
        }

        public override void GenerateForHasBeenSetCheck(FileGeneration fg, string accessor, string checkMaskAccessor)
        {
            RawFormID.GenerateForHasBeenSetCheck(
                fg: fg,
                accessor: accessor,
                checkMaskAccessor: checkMaskAccessor);
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            RawFormID.GenerateForEqualsMask(
                fg: fg,
                accessor: accessor,
                rhsAccessor: rhsAccessor,
                retAccessor: retAccessor);
        }

        public override void GenerateForCopy(FileGeneration fg, string accessorPrefix, string rhsAccessorPrefix, string copyMaskAccessor, string defaultFallbackAccessor, string cmdsAccessor, bool protectedMembers)
        {
            RawFormID.GenerateForCopy(
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
