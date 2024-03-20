using System.Xml.Linq;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Fields;

public class FormLinkType : ClassType
{
    public enum FormIDTypeEnum
    {
        Normal,
        EDIDChars
    }

    public bool MaxIsNone { get; private set; }

    public override string ProtectedName => base.ProtectedName;
    private FormIDType _rawFormID;
    public MutagenLoquiType LoquiType { get; private set; }
    public FormIDTypeEnum FormIDType;
    public override bool IsEnumerable => false;
    public override bool CanBeNullable(bool getter) => false;
    public string GenericString => LoquiType.TypeNameInternal(getter: true, internalInterface: true);

    public virtual string ClassTypeStringPrefix => this.FormIDType switch
    {
        FormIDTypeEnum.Normal => "Form",
        FormIDTypeEnum.EDIDChars => "EDID",
        _ => throw new NotImplementedException(),
    };

    public virtual string ClassTypeString => $"{ClassTypeStringPrefix}Link";

    public string FormIDTypeString => this.FormIDType switch
    {
        FormIDTypeEnum.Normal when Nullable => "FormKeyNullable",
        FormIDTypeEnum.Normal => "FormKey",
        FormIDTypeEnum.EDIDChars => "EDID",
        _ => throw new NotImplementedException(),
    };

    public override string TypeName(bool getter, bool needsCovariance = false)
    {
        return $"I{DirectNonGenericTypeName()}{(needsCovariance || getter ? "Getter" : null)}<{GenericString}>";
    }

    public override Type Type(bool getter) => typeof(FormID);

    public string DirectTypeName(bool getter, bool internalInterface = false)
    {
        return $"{DirectNonGenericTypeName()}<{GenericString}>";
    }

    public string DirectNonGenericTypeName()
    {
        return $"{ClassTypeString}{(this.Nullable ? "Nullable" : string.Empty)}";
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
        this.NullableProperty.Subscribe(i => LoquiType.NullableProperty.OnNext(i));
        this.NullableProperty.Subscribe(i => _rawFormID.NullableProperty.OnNext(i));
        this.FormIDType = node.GetAttribute<FormIDTypeEnum>("type", defaultVal: FormIDTypeEnum.Normal);
        MaxIsNone = node.GetAttribute("maxIsNone", false);
        this.Singleton = true;
        this.SetPermission = AccessModifier.Private;
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

    public override void GenerateForEqualsMask(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, string retAccessor)
    {
        _rawFormID.GenerateForEqualsMask(
            sb: sb,
            accessor: accessor,
            rhsAccessor: rhsAccessor,
            retAccessor: retAccessor);
    }

    public override string GetNewForNonNullable()
    {
        return $"new {DirectTypeName(getter: false, internalInterface: true)}()";
    }

    public override void GenerateForEquals(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, Accessor maskAccessor)
    {
        sb.AppendLine($"if ({this.GetTranslationIfAccessor(maskAccessor)})");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"if (!{accessor}.Equals({rhsAccessor})) return false;");
        }
    }

    public override void GenerateForCopy(StructuredStringBuilder sb, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
    {
        sb.AppendLine($"if ({(deepCopy ? this.GetTranslationIfAccessor(copyMaskAccessor) : this.SkipCheck(copyMaskAccessor, deepCopy))})");
        using (sb.CurlyBrace())
        {
            if (this.Nullable
                || deepCopy)
            {
                sb.AppendLine($"{accessor}.SetTo({rhs}.{FormIDTypeString});");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public override void GenerateToString(StructuredStringBuilder sb, string name, Accessor accessor, string sbAccessor)
    {
        if (!this.IntegrateField) return;
        sb.AppendLine($"sb.{nameof(StructuredStringBuilder.AppendItem)}({accessor}.{FormIDTypeString}{(string.IsNullOrWhiteSpace(this.Name) ? null : $", \"{this.Name}\"")});");
    }

    public override void GenerateUnsetNth(StructuredStringBuilder sb, Accessor identifier)
    {
        if (!this.IntegrateField) return;
        if (!this.ReadOnly)
        {
            if (this.Nullable)
            {
                using (var args = sb.Call(
                           $"{identifier}.Unset"))
                {
                }
            }
            else
            {
                sb.AppendLine($"{identifier} = default({LoquiType.TypeName(getter: false)});");
            }
        }
        sb.AppendLine("break;");
    }

    public override void GenerateClear(StructuredStringBuilder sb, Accessor identifier)
    {
        if (this.ReadOnly || !this.IntegrateField) return;
        sb.AppendLine($"{identifier}.Clear();");
    }

    public override string ReturnForCopySetToConverter(Accessor itemAccessor)
    {
        return
            $"({TypeName(getter: false, needsCovariance: true)})new {DirectTypeName(getter: false)}({itemAccessor}.{FormIDTypeString})";
    }

    public override async Task GenerateForClass(StructuredStringBuilder sb)
    {
        // Want to intercept any sets and wrap, to make sure it's not sharing a ref with another record
        sb.AppendLine($"private readonly {this.TypeName(getter: false)} _{this.Name} = {GetNewForNonNullable()};");
        sb.AppendLine($"public {this.TypeName(getter: false)} {this.Name}");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"get => _{this.Name};");
            sb.AppendLine($"set => _{this.Name}.SetTo(value);");
        }
        sb.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
        sb.AppendLine($"{this.TypeName(getter: true)} {this.ObjectGen.Interface(getter: true, this.InternalGetInterface)}.{this.Name} => this.{this.Name};");
    }

    public override void GenerateForInterface(StructuredStringBuilder sb, bool getter, bool internalInterface)
    {
        if (getter)
        {
            if (!ApplicableInterfaceField(getter, internalInterface)) return;
            sb.AppendLine($"{TypeName(getter: true, needsCovariance: true)} {this.Name} {{ get; }}");
        }
        else
        {
            if (!ApplicableInterfaceField(getter, internalInterface)) return;
            sb.AppendLine($"new {TypeName(getter: false)} {this.Name} {{ get; set; }}");
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
        if (this.Nullable) return $"FormLinkNullable<{LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>.Null";
        return $"FormLink<{LoquiType.TypeNameInternal(getter: true, internalInterface: true)}>.Null";
    }
}