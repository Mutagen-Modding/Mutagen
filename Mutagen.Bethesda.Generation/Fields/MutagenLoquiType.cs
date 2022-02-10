using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class MutagenLoquiType : LoquiType
{
    private ObjectType _interfObjectType;
    public bool RequireInterfaceObject = true;

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        if (this.RefType == LoquiRefType.Interface)
        {
            if (!node.TryGetAttribute<ObjectType>(Mutagen.Bethesda.Generation.Constants.ObjectType, out _interfObjectType)
                && RequireInterfaceObject)
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
                    fg.AppendLine($"{retAccessor}{nameof(LoquiRegistration)}.GetCopyFunc<{_generic}, {_generic}Getter>()({rhsAccessor}, null);");
                }
                break;
            case LoquiRefType.Interface:
                if (deepCopy)
                {
                    fg.AppendLine($"{retAccessor}(r.DeepCopy() as {this.TypeNameInternal(getter: false, internalInterface: true)})!;");
                }
                else
                {
                    fg.AppendLine($"{retAccessor}{nameof(LoquiRegistration)}.GetCopyFunc<{this.TypeName()}, {this.TypeName(getter: true)}>(r.GetType(), typeof({this.TypeName(getter: true)}))({rhsAccessor}, null);");
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }
 
    public override string GetDefault(bool getter) 
    { 
        if (this.Nullable) 
        { 
            return "default"; 
        } 
        else 
        { 
            return $"new {this.TargetObjectGeneration.Name}()"; 
        } 
    } 
}