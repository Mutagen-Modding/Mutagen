using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ContainedAssetLinksModule : AContainedLinksModule<AssetLinkType>
{
    public static ContainedAssetLinksModule Instance = new();

    public override async Task<Case> HasLinks(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
    {
        if (obj.GetObjectData().HasMetaAssets) return Case.Yes;
        return await base.HasLinks(obj, includeBaseClass, specifications);
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        await foreach (var b in base.RequiredUsingStatements(obj))
        {
            yield return b;
        }

        if (!await ShouldGenerate(obj))
        {
            yield break;
        }
        yield return "Mutagen.Bethesda.Assets";
    }

    public override async Task PostLoad(ObjectGeneration obj)
    {
        await base.PostLoad(obj);
        obj.GetObjectData().HasMetaAssets = obj.Node.GetAttribute("metaAssets", false);
    }

    public override async IAsyncEnumerable<(LoquiInterfaceType Location, string Interface)> Interfaces(
        ObjectGeneration obj)
    {
        if (await ShouldGenerate(obj))
        {
            yield return (LoquiInterfaceType.IGetter, $"{nameof(IAssetLinkContainerGetter)}");
            yield return (LoquiInterfaceType.ISetter, $"{nameof(IAssetLinkContainer)}");
        }
    }

    public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
    {
        if (!await ShouldGenerate(obj)) return;
        if (maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class))
        {
            if (obj.GetObjectData().HasMetaAssets)
            {
                fg.AppendLine($"public static partial IEnumerable<IAssetLink> GetAdditionalAssetLinks({obj.Interface(getter: true)} obj, ILinkCache linkCache);");
            }
            
            fg.AppendLine($"public IEnumerable<{nameof(IAssetLinkGetter)}> EnumerateAssetLinks({obj.Interface(getter: true)} obj, ILinkCache? linkCache, bool includeImplicit)");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine("foreach (var item in base.EnumerateAssetLinks(obj, linkCache, includeImplicit))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return item;");
                        }
                        break;
                    }
                }
                
                if (obj.GetObjectData().HasMetaAssets)
                {
                    fg.AppendLine("if (includeImplicit && linkCache != null)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"foreach (var additional in GetAdditionalAssetLinks(obj, linkCache))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return additional;");
                        }
                    }
                }
                
                var startCount = fg.Count;
                foreach (var field in obj.IterateFields(nonIntegrated: true))
                {
                    if (field is AssetLinkType link)
                    {
                        if (field.Nullable)
                        {
                            fg.AppendLine($"if (obj.{field.Name}.{nameof(IAssetLink.RawPath)}.HasValue)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return obj.{field.Name};");
                            }
                        }
                        else
                        {
                            fg.AppendLine($"yield return obj.{field.Name};");
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
                            fg.AppendLine($"if (obj.{field.Name} is {nameof(IAssetLinkContainerGetter)} {field.Name}linkCont)");
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
                            fg.AppendLine($"foreach (var item in {access}.{nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(linkCache, includeImplicit: includeImplicit))");
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
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.SelectMany(f => f.{nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(linkCache, includeImplicit)))");
                                        break;
                                    case Case.Maybe:
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.WhereCastable<{contLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainerGetter)}>()");
                                        using (new DepthWrapper(subFg))
                                        {
                                            subFg.AppendLine($".SelectMany((f) => f.{nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(linkCache, includeImplicit)))");
                                        }
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                        }
                        else if (cont.SubTypeGeneration is AssetLinkType assetLinkType)
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
                                fg.AppendLine($"yield return item;");
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
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.SelectMany(f => f.{nameof(IAssetLinkContainer.EnumerateAssetLinks)}(linkCache, includeImplicit: includeImplicit))");
                                    break;
                                case Case.Maybe:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainerGetter)}>()");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine($".SelectMany((f) => f.{nameof(IAssetLinkContainer.EnumerateAssetLinks)}(linkCache, includeImplicit: includeImplicit)))");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (dict.ValueTypeGen is AssetLinkType)
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
            fg.AppendLine($"public IEnumerable<{nameof(IAssetLink)}> EnumerateListedAssetLinks({obj.Interface(getter: false)} obj)");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine("foreach (var item in base.EnumerateListedAssetLinks(obj))");
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
                    if (field is AssetLinkType link)
                    {
                        if (field.Nullable)
                        {
                            fg.AppendLine($"if (obj.{field.Name}.{nameof(IAssetLink.RawPath)}.HasValue)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return obj.{field.Name};");
                            }
                        }
                        else
                        {
                            fg.AppendLine($"yield return obj.{field.Name};");
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
                            fg.AppendLine($"if (obj.{field.Name} is {nameof(IAssetLinkContainer)} {field.Name}linkCont)");
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
                            fg.AppendLine($"foreach (var item in {access}.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}())");
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
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.SelectMany(f => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}()))");
                                        break;
                                    case Case.Maybe:
                                        subFg.AppendLine($"foreach (var item in {access}{filterNulls}.WhereCastable<{contLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainer)}>()");
                                        using (new DepthWrapper(subFg))
                                        {
                                            subFg.AppendLine($".SelectMany((f) => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}()))");
                                        }
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                        }
                        else if (cont.SubTypeGeneration is AssetLinkType assetLinkType)
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
                                fg.AppendLine($"yield return item;");
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
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.SelectMany(f => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}())");
                                    break;
                                case Case.Maybe:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainer)}>()");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine($".SelectMany((f) => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}()))");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (dict.ValueTypeGen is AssetLinkType)
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
            fg.AppendLine($"public void {nameof(IAssetLinkContainer.RemapListedAssetLinks)}({obj.Interface(getter: false)} obj, IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping)");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine($"base.{nameof(IAssetLinkContainer.RemapListedAssetLinks)}(obj, mapping);");
                        break;
                    }
                }
                var startCount = fg.Count;
                foreach (var field in obj.IterateFields(nonIntegrated: true))
                {
                    if (field is AssetLinkType)
                    {
                        fg.AppendLine($"obj.{field.Name}.Relink(mapping);");
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
                        fg.AppendLine($"obj.{field.Name}{field.NullChar}.{nameof(IAssetLinkContainer.RemapListedAssetLinks)}(mapping);");
                    }
                    else if (field is WrapperType cont)
                    {
                        if ((cont.SubTypeGeneration is LoquiType contLoqui
                             && await HasLinks(contLoqui, includeBaseClass: true) != Case.No)
                            || cont.SubTypeGeneration is AssetLinkType)
                        {
                            fg.AppendLine($"obj.{field.Name}{field.NullChar}.ForEach(x => x{field.NullChar}.{nameof(IAssetLinkContainer.RemapListedAssetLinks)}(mapping));");
                        }
                    }
                    else if (field is DictType dict)
                    {
                        if (dict.Mode == DictMode.KeyedValue
                            && dict.ValueTypeGen is LoquiType dictLoqui
                            && await HasLinks(dictLoqui, includeBaseClass: true) != Case.No)
                        {
                            fg.AppendLine($"obj.{field.Name}{field.NullChar}.{nameof(IAssetLinkContainer.RemapListedAssetLinks)}(mapping);");
                        }
                        else if (dict.ValueTypeGen is FormLinkType formIDType)
                        {
                            fg.AppendLine($"obj.{field.Name}{field.NullChar}.{nameof(IAssetLinkContainer.RemapListedAssetLinks)}(mapping);");
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
        if (!await ShouldGenerate(obj)) return;
        await GenerateInterfaceImplementation(obj, fg, getter: false);
    }

    public async Task<bool> ShouldGenerate(ObjectGeneration obj)
    {
        if (obj.GetObjectType() != ObjectType.Mod
            && obj.GetObjectType() != ObjectType.Group)
        {
            var linkCase = await HasLinks(obj, includeBaseClass: false);
            if (linkCase == Case.No) return false;
        }

        return true;
    }

    public async Task GenerateInterfaceImplementation(ObjectGeneration obj, FileGeneration fg, bool getter)
    {
        var shouldAlwaysOverride = obj.IsTopLevelGroup();
        fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}IEnumerable<{nameof(IAssetLinkGetter)}> {nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(ILinkCache? linkCache, bool includeImplicit) => {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateAssetLinks(this, linkCache, includeImplicit);");

        if (!getter)
        {
            fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}IEnumerable<{nameof(IAssetLink)}> {nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}() => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}(this);");
            fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}void {nameof(IAssetLinkContainer.RemapListedAssetLinks)}(IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping) => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.RemapListedAssetLinks(this, mapping);");
        }
    }
}