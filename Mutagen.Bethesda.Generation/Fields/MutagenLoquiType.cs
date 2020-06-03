using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;
using Loqui;
using Mutagen.Bethesda.Binary;
using System.Reflection.Metadata;

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

        public override void GenerateTypicalMakeCopy(FileGeneration fg, Accessor retAccessor, Accessor rhsAccessor, Accessor copyMaskAccessor, bool deepCopy, bool doTranslationMask)
        {
            if (this.GetObjectType() != ObjectType.Record)
            {
                base.GenerateTypicalMakeCopy(fg, retAccessor, rhsAccessor, copyMaskAccessor, deepCopy, doTranslationMask: doTranslationMask);
                return;
            }
            switch (this.RefType)
            {
                case LoquiRefType.Direct:
                    using (var args = new ArgsWrapper(fg,
                        $"{retAccessor}({this.TargetObjectGeneration.ObjectName}){rhsAccessor}.{(deepCopy ? "Deep" : null)}Copy"))
                    {
                        if (this.RefType == LoquiRefType.Direct)
                        {
                            if (!doTranslationMask)
                            {
                                args.Add($"copyMask: default(TranslationCrystal)");
                            }
                            else if (deepCopy)
                            {
                                args.Add($"copyMask: {copyMaskAccessor}?.GetSubCrystal({this.IndexEnumInt})");
                            }
                            else
                            {
                                args.Add($"copyMask: {copyMaskAccessor}.Specific");
                            }
                        }
                        args.AddPassArg($"errorMask");
                    }
                    break;
                case LoquiRefType.Generic:
                    if (deepCopy)
                    {
                        fg.AppendLine($"{retAccessor}(r.DeepCopy() as {_generic})!;");
                    }
                    else
                    {
                        fg.AppendLine($"{retAccessor}{nameof(LoquiRegistration)}.GetCopyFunc<{_generic}, {_generic}Getter>()({rhsAccessor.DirectAccess}, null);");
                    }
                    break;
                case LoquiRefType.Interface:
                    if (deepCopy)
                    {
                        fg.AppendLine($"{retAccessor}(r.DeepCopy() as {this.TypeNameInternal(getter: false, internalInterface: true)})!;");
                    }
                    else
                    {
                        fg.AppendLine($"{retAccessor}{nameof(LoquiRegistration)}.GetCopyFunc<{this.TypeName()}, {this.TypeName(getter: true)}>(r.GetType(), typeof({this.TypeName(getter: true)}))({rhsAccessor.DirectAccess}, null);");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
