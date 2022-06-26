using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Generation.Fields;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;

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

    public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
    {
        if (await HasLinks(obj, includeBaseClass: false) == Case.No) return;
        if (maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class))
        {
            fg.AppendLine($"public IEnumerable<{nameof(IFormLinkGetter)}> GetContainedFormLinks({obj.Interface(getter: true)} obj)");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine("foreach (var item in base.GetContainedFormLinks(obj))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return item;");
                        }
                        break;
                    }
                }
                var startCount = fg.Count;
                foreach (var field in obj.IterateFields(nonIntegrated: true))
                {
                    if (field is FormLinkType formLink)
                    {
                        if (field.Nullable)
                        {
                            fg.AppendLine($"if (obj.{field.Name}.{formLink.FormIDTypeString}.HasValue)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name});");
                            }
                        }
                        else if (formLink.FormIDType == FormLinkType.FormIDTypeEnum.Normal)
                        {
                            fg.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name});");
                        }
                    }
                    else if (field is FormKeyType formKey
                             && obj.Name != "MajorRecord")
                    {
                        if (field.Nullable)
                        {
                            fg.AppendLine($"if (obj.{field.Name} != null)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name}.AsLink<I{obj.ProtoGen.Protocol.Namespace}MajorRecordGetter>());");
                            }
                        }
                        else
                        {
                            fg.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(obj.{field.Name}.AsLink<I{obj.ProtoGen.Protocol.Namespace}MajorRecordGetter>());");
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
                            fg.AppendLine($"if (obj.{field.Name} is {nameof(IFormLinkContainerGetter)} {field.Name}linkCont)");
                            access = $"{field.Name}linkCont";
                        }
                        else if (loqui.Nullable)
                        {
                            fg.AppendLine($"if (obj.{field.Name} is {{}} {field.Name}Items)");
                            access = $"{field.Name}Items";
                        }
                        else
                        {
                            doBrace = false;
                        }
                        using (new BraceWrapper(fg, doIt: doBrace))
                        {
                            fg.AppendLine($"foreach (var item in {access}.{nameof(IFormLinkContainerGetter.ContainedFormLinks)})");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return item;");
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

                        FileGeneration subFg = new FileGeneration();
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
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.SelectMany(f => f.{nameof(IFormLinkContainerGetter.ContainedFormLinks)}))");
                                        break;
                                    case Case.Maybe:
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.WhereCastable<{contLoqui.TypeName(getter: true)}, {nameof(IFormLinkContainerGetter)}>()");
                                        using (new DepthWrapper(subFg))
                                        {
                                            subFg.AppendLine($".SelectMany((f) => f.{nameof(IFormLinkContainerGetter.ContainedFormLinks)}))");
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
                            fg.AppendLine($"if (obj.{field.Name} is {{}} {field.Name}Item)");
                        }
                        using (new BraceWrapper(fg, doIt: field.Nullable))
                        {
                            fg.AppendLines(subFg);
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return {nameof(FormLinkInformation)}.{nameof(FormLinkInformation.Factory)}(item);");
                            }
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.Mode == DictMode.KeyedValue
                            && dict.ValueTypeGen is LoquiType dictLoqui
                            && await HasLinks(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            var linktype = await HasLinks(dictLoqui, includeBaseClass: true);
                            switch (linktype)
                            {
                                case Case.Yes:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.SelectMany(f => f.{nameof(IFormLinkContainerGetter.ContainedFormLinks)}))");
                                    break;
                                case Case.Maybe:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: true)}, {nameof(IFormLinkContainerGetter)}>()");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine($".SelectMany((f) => f.{nameof(IFormLinkContainerGetter.ContainedFormLinks)}))");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (dict.ValueTypeGen is FormLinkType formIDType)
                        {
                            fg.AppendLine($"foreach (var item in obj.{field.Name}.Values)");
                        }
                        else
                        {
                            continue;
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"yield return item;");
                        }
                    }
                    else if (field is BreakType breakType)
                    {
                        if (fg.Count > startCount)
                        {
                            fg.AppendLine($"if (obj.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index})) yield break;");
                        }
                    }
                }
                // Remove trailing breaks
                while (fg.Count > startCount)
                {
                    if (fg[fg.Count - 1].AsSpan().TrimStart().StartsWith($"if (obj.{VersioningModule.VersioningFieldName}"))
                    {
                        fg.RemoveAt(fg.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                fg.AppendLine("yield break;");
            }
            fg.AppendLine();
        }

        if (maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class))
        {
            fg.AppendLine($"public void RemapLinks({obj.Interface(getter: false)} obj, IReadOnlyDictionary<FormKey, FormKey> mapping)");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine("base.RemapLinks(obj, mapping);");
                        break;
                    }
                }
                var startCount = fg.Count;
                foreach (var field in obj.IterateFields(nonIntegrated: true))
                {
                    if (field is FormLinkType formLink
                        && formLink.FormIDType == FormLinkType.FormIDTypeEnum.Normal)
                    {
                        fg.AppendLine($"obj.{field.Name}.Relink(mapping);");
                    }
                    else if (field is FormKeyType formKey
                             && obj.Name != "MajorRecord")
                    {
                        fg.AppendLine($"obj.{field.Name} = {nameof(FormLinkRemappingMixIn)}.Remap(obj.{field.Name}, mapping);");
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
                        fg.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                    }
                    else if (field is WrapperType cont)
                    {
                        if ((cont.SubTypeGeneration is LoquiType contLoqui
                             && await HasLinks(contLoqui, includeBaseClass: true) != Case.No)
                            || (cont.SubTypeGeneration is FormLinkType formIDType
                                && formIDType.FormIDType == FormLinkType.FormIDTypeEnum.Normal))
                        {
                            fg.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.Mode == DictMode.KeyedValue
                            && dict.ValueTypeGen is LoquiType dictLoqui
                            && await HasLinks(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            fg.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                        }
                        else if (dict.ValueTypeGen is FormLinkType formIDType)
                        {
                            fg.AppendLine($"obj.{field.Name}{(field.Nullable ? "?" : null)}.RemapLinks(mapping);");
                        }
                    }
                    else if (field is BreakType breakType)
                    {
                        if (fg.Count > startCount)
                        {
                            fg.AppendLine($"if (obj.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index})) return;");
                        }
                    }
                }
                // Remove trailing breaks
                while (fg.Count > startCount)
                {
                    if (fg[^1].AsSpan().TrimStart().StartsWith($"if (obj.{VersioningModule.VersioningFieldName}"))
                    {
                        fg.RemoveAt(fg.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            fg.AppendLine();
        }
    }

    public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
    {
        await base.GenerateInClass(obj, fg);
        if (obj.GetObjectType() != ObjectType.Mod)
        {
            var linkCase = await HasLinks(obj, includeBaseClass: false);
            if (linkCase == Case.No) return;
        }
        await GenerateInterfaceImplementation(obj, fg, getter: false);
    }

    public async Task GenerateInterfaceImplementation(ObjectGeneration obj, FileGeneration fg, bool getter)
    {
        var shouldAlwaysOverride = obj.IsTopLevelGroup();
        fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}IEnumerable<{nameof(IFormLinkGetter)}> {nameof(IFormLinkContainerGetter.ContainedFormLinks)} => {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.GetContainedFormLinks(this);");

        if (!getter)
        {
            fg.AppendLine($"public{await obj.FunctionOverride(async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}void {nameof(IFormLinkContainer.RemapLinks)}(IReadOnlyDictionary<FormKey, FormKey> mapping) => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.RemapLinks(this, mapping);");
        }
    }
}