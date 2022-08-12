using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ContainedFormLinksModule : AContainedLinksModule<FormLinkType>
{
    public static ContainedFormLinksModule Instance = new();
    
    public override async IAsyncEnumerable<(LoquiInterfaceType Location, string Interface)> Interfaces(ObjectGeneration obj)
    {
        if (await HasLinks(obj, includeBaseClass: false) != Case.No)
        {
            yield return (LoquiInterfaceType.IGetter, $"{nameof(IFormLinkContainerGetter)}");
            yield return (LoquiInterfaceType.ISetter, $"{nameof(IFormLinkContainer)}");
        }
    }

    public override async Task GenerateInCommon(ObjectGeneration obj, StructuredStringBuilder sb, MaskTypeSet maskTypes)
    {
        if (maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class))
        {
            sb.AppendLine($"public IEnumerable<{nameof(IFormLinkGetter)}> EnumerateFormLinks({obj.Interface(getter: true)} obj)");
            using (sb.CurlyBrace())
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        sb.AppendLine("foreach (var item in base.EnumerateFormLinks(obj))");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("yield return item;");
                        }
                        break;
                    }
                }
                var startCount = sb.Count;
                foreach (var field in obj.IterateFields(nonIntegrated: true))
                {
                    if (field is FormLinkType formLink)
                    {
                        if (field.Nullable)
                        {
                            sb.AppendLine($"if ({nameof(FormLinkInformation)}.{nameof(FormLinkInformation.TryFactory)}(obj.{field.Name}, out var {field.Name}Info))");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine($"yield return {field.Name}Info;");
                            }
                        }
                        else if (formLink.FormIDType == FormLinkType.FormIDTypeEnum.Normal)
                        {
                            sb.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name});");
                        }
                    }
                    else if (field is FormKeyType formKey
                             && obj.Name != "MajorRecord")
                    {
                        if (field.Nullable)
                        {
                            sb.AppendLine($"if (obj.{field.Name} != null)");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name}.AsLink<I{obj.ProtoGen.Protocol.Namespace}MajorRecordGetter>());");
                            }
                        }
                        else
                        {
                            sb.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name}.AsLink<I{obj.ProtoGen.Protocol.Namespace}MajorRecordGetter>());");
                        }
                    }
                    else if (field is LoquiType loqui)
                    {
                        Case subLinkCase;
                        if (loqui.TargetObjectGeneration != null)
                        {
                            subLinkCase = await HasLinks(loqui, includeBaseClass: true);
                        }
                        else
                        {
                            subLinkCase = Case.Maybe;
                        }
                        if (subLinkCase == Case.No) continue;
                        var doBrace = true;
                        var access = $"obj.{field.Name}";
                        if (subLinkCase == Case.Maybe)
                        {
                            sb.AppendLine($"if (obj.{field.Name} is {nameof(IFormLinkContainerGetter)} {field.Name}linkCont)");
                            access = $"{field.Name}linkCont";
                        }
                        else if (loqui.Nullable)
                        {
                            sb.AppendLine($"if (obj.{field.Name} is {{}} {field.Name}Items)");
                            access = $"{field.Name}Items";
                        }
                        else
                        {
                            doBrace = false;
                        }
                        using (sb.CurlyBrace(doIt: doBrace))
                        {
                            sb.AppendLine($"foreach (var item in {access}.{nameof(IFormLinkContainerGetter.EnumerateFormLinks)}())");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine($"yield return item;");
                            }
                        }
                    }
                    else if (field is WrapperType cont)
                    {
                        var access = $"obj.{field.Name}";
                        if (field.Nullable)
                        {
                            access = $"{field.Name}Item";
                        }

                        StructuredStringBuilder subFg = new StructuredStringBuilder();
                        if (cont.SubTypeGeneration is LoquiType contLoqui
                            && await HasLinks(contLoqui, includeBaseClass: true) != Case.No)
                        {
                            string filterNulls = cont is GenderedType && ((GenderedType)cont).ItemNullable ? ".NotNull()" : null;
                            var linktype = await HasLinks(contLoqui, includeBaseClass: true);
                            if (linktype != Case.No)
                            {
                                switch (linktype)
                                {
                                    case Case.Yes:
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.SelectMany(f => f.{nameof(IFormLinkContainerGetter.EnumerateFormLinks)}()))");
                                        break;
                                    case Case.Maybe:
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.WhereCastable<{contLoqui.TypeName(getter: true)}, {nameof(IFormLinkContainerGetter)}>()");
                                        using (subFg.IncreaseDepth())
                                        {
                                            subFg.AppendLine($".SelectMany((f) => f.{nameof(IFormLinkContainerGetter.EnumerateFormLinks)}()))");
                                        }
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                        }
                        else if (cont.SubTypeGeneration is FormLinkType formIDType
                                 && formIDType.FormIDType == FormLinkType.FormIDTypeEnum.Normal)
                        {
                            string filterNulls = cont is GenderedType && ((GenderedType)cont).ItemNullable ? ".NotNull()" : null;
                            subFg.AppendLine($"foreach (var item in {access}{filterNulls})");
                        }
                        else
                        {
                            continue;
                        }

                        if (field.Nullable)
                        {
                            sb.AppendLine($"if (obj.{field.Name} is {{}} {field.Name}Item)");
                        }
                        using (sb.CurlyBrace(doIt: field.Nullable))
                        {
                            sb.AppendLines(subFg);
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(item);");
                            }
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.ValueTypeGen is LoquiType dictLoqui
                            && await HasLinks(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            var valuesAccessor = dict.Mode == DictMode.KeyedValue ? "Items" : "Values";
                            var linktype = await HasLinks(dictLoqui, includeBaseClass: true);
                            switch (linktype)
                            {
                                case Case.Yes:
                                    sb.AppendLine($"foreach (var item in obj.{field.Name}.{valuesAccessor}.SelectMany(f => f.{nameof(IFormLinkContainerGetter.EnumerateFormLinks)}()))");
                                    break;
                                case Case.Maybe:
                                    sb.AppendLine($"foreach (var item in obj.{field.Name}.{valuesAccessor}.WhereCastable<{dictLoqui.TypeName(getter: true)}, {nameof(IFormLinkContainerGetter)}>()");
                                    using (sb.IncreaseDepth())
                                    {
                                        sb.AppendLine($".SelectMany((f) => f.{nameof(IFormLinkContainerGetter.EnumerateFormLinks)}()))");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (dict.ValueTypeGen is FormLinkType formIDType)
                        {
                            sb.AppendLine($"foreach (var item in obj.{field.Name}.Values)");
                        }
                        else
                        {
                            continue;
                        }
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine($"yield return item;");
                        }
                    }
                    else if (field is BreakType breakType)
                    {
                        if (sb.Count > startCount)
                        {
                            sb.AppendLine($"if (obj.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index})) yield break;");
                        }
                    }
                }
                // Remove trailing breaks
                while (sb.Count > startCount)
                {
                    if (sb[sb.Count - 1].AsSpan().TrimStart().StartsWith($"if (obj.{VersioningModule.VersioningFieldName}"))
                    {
                        sb.RemoveAt(sb.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                sb.AppendLine("yield break;");
            }
            sb.AppendLine();
        }

        if (maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class))
        {
            sb.AppendLine($"public void RemapLinks({obj.Interface(getter: false)} obj, IReadOnlyDictionary<FormKey, FormKey> mapping)");
            using (sb.CurlyBrace())
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        sb.AppendLine("base.RemapLinks(obj, mapping);");
                        break;
                    }
                }
                var startCount = sb.Count;
                foreach (var field in obj.IterateFields(nonIntegrated: true))
                {
                    if (field is FormLinkType formLink
                        && formLink.FormIDType == FormLinkType.FormIDTypeEnum.Normal)
                    {
                        sb.AppendLine($"obj.{field.Name}.Relink(mapping);");
                    }
                    else if (field is FormKeyType formKey
                             && obj.Name != "MajorRecord")
                    {
                        sb.AppendLine($"obj.{field.Name} = {nameof(FormLinkRemappingMixIn)}.Remap(obj.{field.Name}, mapping);");
                    }
                    else if (field is LoquiType loqui)
                    {
                        Case subLinkCase;
                        if (loqui.TargetObjectGeneration != null)
                        {
                            subLinkCase = await HasLinks(loqui, includeBaseClass: true);
                        }
                        else
                        {
                            subLinkCase = Case.Maybe;
                        }
                        if (subLinkCase == Case.No) continue;
                        sb.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                    }
                    else if (field is WrapperType cont)
                    {
                        if ((cont.SubTypeGeneration is LoquiType contLoqui
                             && await HasLinks(contLoqui, includeBaseClass: true) != Case.No)
                            || (cont.SubTypeGeneration is FormLinkType formIDType
                                && formIDType.FormIDType == FormLinkType.FormIDTypeEnum.Normal))
                        {
                            sb.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.Mode == DictMode.KeyedValue
                            && dict.ValueTypeGen is LoquiType dictLoqui
                            && await HasLinks(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            sb.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                        }
                        else if (dict.ValueTypeGen is FormLinkType formIDType)
                        {
                            sb.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                        }
                    }
                    else if (field is BreakType breakType)
                    {
                        if (sb.Count > startCount)
                        {
                            sb.AppendLine($"if (obj.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index})) return;");
                        }
                    }
                }
                // Remove trailing breaks
                while (sb.Count > startCount)
                {
                    if (sb[^1].AsSpan().TrimStart().StartsWith($"if (obj.{VersioningModule.VersioningFieldName}"))
                    {
                        sb.RemoveAt(sb.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            sb.AppendLine();
        }
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder fg)
    {
        await base.GenerateInClass(obj, fg);
        if (obj.GetObjectType() != ObjectType.Mod)
        {
            var linkCase = await HasLinks(obj, includeBaseClass: false);
            if (linkCase == Case.No) return;
        }
        await GenerateInterfaceImplementation(obj, fg, getter: false);
    }

    public async Task GenerateInterfaceImplementation(ObjectGeneration obj, StructuredStringBuilder fg, bool getter)
    {
        var shouldAlwaysOverride = obj.IsTopLevelGroup();
        fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}IEnumerable<{nameof(IFormLinkGetter)}> {nameof(IFormLinkContainerGetter.EnumerateFormLinks)}() => {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateFormLinks(this);");

        if (!getter)
        {
            fg.AppendLine($"public{await obj.FunctionOverride(async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}void {nameof(IFormLinkContainer.RemapLinks)}(IReadOnlyDictionary<FormKey, FormKey> mapping) => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.RemapLinks(this, mapping);");
        }
    }
}