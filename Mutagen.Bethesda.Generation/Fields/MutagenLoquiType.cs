using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;
using Loqui;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenLoquiType : LoquiType
    {
        private ObjectType _interfObjectType;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            if (this.RefType == LoquiRefType.Interface)
            {
                if (!node.TryGetAttribute<ObjectType>(Mutagen.Bethesda.Generation.Constants.ObjectType, out _interfObjectType))
                {
                    throw new ArgumentException("Interface Ref Type was specified without supplying object type");
                }
            }
        }

        public ObjectType? GetObjectType()
        {
            if (this.RefType == LoquiRefType.Interface) return _interfObjectType;
            return this.TargetObjectGeneration?.GetObjectType();
        }

        public override void GenerateTypicalMakeCopy(FileGeneration fg, string retAccessor, Accessor rhsAccessor, Accessor defAccessor, string copyMaskAccessor)
        {
            if (this.GetObjectType() != ObjectType.Record)
            {
                base.GenerateTypicalMakeCopy(fg, retAccessor, rhsAccessor, defAccessor, copyMaskAccessor);
                return;
            }
            switch (this.RefType)
            {
                case LoquiRefType.Direct:
                    fg.AppendLine($"var copyRet = new {this.TargetObjectGeneration.ObjectName}({rhsAccessor}.FormKey);");
                    using (var args2 = new ArgsWrapper(fg,
                        $"copyRet.CopyFieldsFrom"))
                    {
                        args2.Add($"rhs: {rhsAccessor}");
                        if (this.RefType == LoquiRefType.Direct)
                        {
                            args2.Add($"copyMask: {copyMaskAccessor}?.Specific");
                        }
                        args2.Add($"def: {defAccessor.DirectAccess}");
                    }
                    fg.AppendLine($"{retAccessor}copyRet;");
                    break;
                case LoquiRefType.Generic:
                    fg.AppendLine($"{retAccessor}{nameof(LoquiRegistration)}.GetCopyFunc<{_generic}>()({rhsAccessor.DirectAccess}, null, {defAccessor.DirectAccess});");
                    break;
                case LoquiRefType.Interface:
                    fg.AppendLine($"{retAccessor}{nameof(LoquiRegistration)}.GetCopyFunc<{this.TypeName()}>(r.GetType())({rhsAccessor.DirectAccess}, null, {defAccessor.DirectAccess});");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
