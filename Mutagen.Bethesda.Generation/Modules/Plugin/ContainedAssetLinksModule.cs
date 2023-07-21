using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ContainedAssetLinksModule : AContainedLinksModule<AssetLinkType>
{
    public static ContainedAssetLinksModule Instance = new();

    public override async Task<Case> HasLinks(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
    {
        if (obj.GetObjectData().HasInferredAssets) return Case.Yes;
        if (obj.GetObjectData().HasResolvedAssets) return Case.Yes;
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
        yield return "Mutagen.Bethesda.Plugins.Assets";
        yield return "Mutagen.Bethesda.Plugins.Cache";
    }

    public override async Task PostLoad(ObjectGeneration obj)
    {
        await base.PostLoad(obj);
        obj.GetObjectData().HasInferredAssets = obj.Node.GetAttribute("inferredAssets", false);
        obj.GetObjectData().HasResolvedAssets = obj.Node.GetAttribute("resolvedAssets", false);
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

    public override async Task GenerateInCommon(ObjectGeneration obj, StructuredStringBuilder fg, MaskTypeSet maskTypes)
    {
        if (!await ShouldGenerate(obj)) return;
        if (maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class))
        {
            if (obj.GetObjectData().HasInferredAssets)
            {
                fg.AppendLine($"public static partial IEnumerable<{nameof(IAssetLinkGetter)}> GetInferredAssetLinks({obj.Interface(getter: true)} obj, Type? assetType);");
            }
            if (obj.GetObjectData().HasResolvedAssets)
            {
                fg.AppendLine($"public static partial IEnumerable<{nameof(IAssetLinkGetter)}> GetResolvedAssetLinks({obj.Interface(getter: true)} obj, {nameof(IAssetLinkCache)} linkCache, Type? assetType);");
            }
            
            await GenerateEnumerateAssetLinks(obj, fg);
        }

        if (maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class))
        {
            await GenerateEnumerateListedAssetLinks(obj, fg);
        }

        if (maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class))
        {
            await GenerateRemapAssetLinks(obj, fg);
        }
    }

    private async Task GenerateEnumerateAssetLinks(ObjectGeneration obj, StructuredStringBuilder fg)
    {
        fg.AppendLine(
            $"public IEnumerable<{nameof(IAssetLinkGetter)}> EnumerateAssetLinks({obj.Interface(getter: true)} obj, {nameof(AssetLinkQuery)} queryCategories, {nameof(IAssetLinkCache)}? linkCache, Type? assetType)");
        using (fg.CurlyBrace())
        {
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                {
                    fg.AppendLine(
                        "foreach (var item in base.EnumerateAssetLinks(obj, queryCategories, linkCache, assetType))");
                    using (fg.CurlyBrace())
                    {
                        fg.AppendLine("yield return item;");
                    }

                    break;
                }
            }

            if (obj.GetObjectData().HasInferredAssets)
            {
                fg.AppendLine($"if (queryCategories.HasFlag({nameof(AssetLinkQuery)}.{nameof(AssetLinkQuery.Inferred)}))");
                using (fg.CurlyBrace())
                {
                    fg.AppendLine($"foreach (var additional in GetInferredAssetLinks(obj, assetType))");
                    using (fg.CurlyBrace())
                    {
                        fg.AppendLine("yield return additional;");
                    }
                }
            }

            if (obj.GetObjectData().HasResolvedAssets)
            {
                fg.AppendLine($"if (queryCategories.HasFlag({nameof(AssetLinkQuery)}.{nameof(AssetLinkQuery.Resolved)}))");
                using (fg.CurlyBrace())
                {
                    fg.AppendLine(
                        $"if (linkCache == null) throw new ArgumentNullException(\"No link cache was given on a query interested in resolved assets\");");
                    fg.AppendLine($"foreach (var additional in GetResolvedAssetLinks(obj, linkCache, assetType))");
                    using (fg.CurlyBrace())
                    {
                        fg.AppendLine("yield return additional;");
                    }
                }
            }

            var subFg = new StructuredStringBuilder();
            await YieldReturnListedAssets(obj, subFg);
            if (subFg.Count > 0)
            {
                fg.AppendLine($"if (queryCategories.HasFlag({nameof(AssetLinkQuery)}.{nameof(AssetLinkQuery.Listed)}))");
                using (fg.CurlyBrace())
                {
                    fg.AppendLines(subFg);
                }
            }

            fg.AppendLine("yield break;");
        }

        fg.AppendLine();
    }

    private async Task GenerateEnumerateListedAssetLinks(ObjectGeneration obj, StructuredStringBuilder fg)
    {
        fg.AppendLine(
            $"public IEnumerable<{nameof(IAssetLink)}> EnumerateListedAssetLinks({obj.Interface(getter: false)} obj)");
        using (fg.CurlyBrace())
        {
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                {
                    fg.AppendLine("foreach (var item in base.EnumerateListedAssetLinks(obj))");
                    using (fg.CurlyBrace())
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
                        fg.AppendLine($"if (obj.{field.Name} != null)");
                        using (fg.CurlyBrace())
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

                    using (fg.CurlyBrace())
                    {
                        fg.AppendLine(
                            $"foreach (var item in {access}.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}())");
                        using (fg.CurlyBrace())
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

                    var subFg = new StructuredStringBuilder();
                    if (cont.SubTypeGeneration is LoquiType contLoqui
                        && await HasLinks(contLoqui, includeBaseClass: true) != Case.No)
                    {
                        string filterNulls =
                            cont is GenderedType && ((GenderedType)cont).ItemNullable ? ".NotNull()" : null;
                        var linktype = await HasLinks(contLoqui, includeBaseClass: true);
                        if (linktype != Case.No)
                        {
                            switch (linktype)
                            {
                                case Case.Yes:
                                    subFg.AppendLine(
                                        $"foreach (var item in {access}{filterNulls}.SelectMany(f => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}()))");
                                    break;
                                case Case.Maybe:
                                    subFg.AppendLine(
                                        $"foreach (var item in {access}{filterNulls}.WhereCastable<{contLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainer)}>()");
                                    using (subFg.IncreaseDepth())
                                    {
                                        subFg.AppendLine(
                                            $".SelectMany((f) => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}()))");
                                    }

                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    }
                    else if (cont.SubTypeGeneration is AssetLinkType assetLinkType)
                    {
                        string filterNulls =
                            cont is GenderedType && ((GenderedType)cont).ItemNullable ? ".NotNull()" : null;
                        subFg.AppendLine(
                            $"foreach (var item in {access}{filterNulls}{(assetLinkType.Nullable ? ".NotNull()" : null)})");
                    }
                    else
                    {
                        continue;
                    }

                    if (field.Nullable)
                    {
                        fg.AppendLine($"if (obj.{field.Name} is {{}} {field.Name}Item)");
                    }

                    using (fg.CurlyBrace(doIt: field.Nullable))
                    {
                        fg.AppendLines(subFg);
                        using (fg.CurlyBrace())
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
                                fg.AppendLine(
                                    $"foreach (var item in obj.{field.Name}.Items.SelectMany(f => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}())");
                                break;
                            case Case.Maybe:
                                fg.AppendLine(
                                    $"foreach (var item in obj.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainer)}>()");
                                using (fg.IncreaseDepth())
                                {
                                    fg.AppendLine(
                                        $".SelectMany((f) => f.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}()))");
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

                    using (fg.CurlyBrace())
                    {
                        fg.AppendLine($"yield return item;");
                    }
                }
                else if (field is BreakType breakType)
                {
                    if (fg.Count > startCount)
                    {
                        fg.AppendLine(
                            $"if (obj.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index})) yield break;");
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

    private async Task GenerateRemapAssetLinks(ObjectGeneration obj, StructuredStringBuilder fg)
    {
        if (obj.GetObjectData().HasInferredAssets)
        {
            using (var f = fg.Function(
                       $"public static partial IEnumerable<{nameof(IAssetLinkGetter)}> RemapInferredAssetLinks",
                       semiColon: true))
            {
                f.Add($"{obj.Interface(getter: false)} obj");
                f.Add($"IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping");
                f.Add($"{nameof(IAssetLinkCache)}? linkCache");
                f.Add($"{nameof(AssetLinkQuery)} queryCategories");
            }
            fg.AppendLine();
        }
        
        if (obj.GetObjectData().HasResolvedAssets)
        {
            using (var f = fg.Function(
                       $"public static partial IEnumerable<{nameof(IAssetLinkGetter)}> RemapResolvedAssetLinks",
                       semiColon: true))
            {
                f.Add($"{obj.Interface(getter: false)} obj");
                f.Add($"IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping");
                f.Add($"{nameof(IAssetLinkCache)}? linkCache");
                f.Add($"{nameof(AssetLinkQuery)} queryCategories");
            }
            fg.AppendLine();
        }
        
        using (var f = fg.Function(
                   $"public void {nameof(IAssetLinkContainer.RemapAssetLinks)}"))
        {
            f.Add($"{obj.Interface(getter: false)} obj");
            f.Add($"IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping");
            f.Add($"{nameof(IAssetLinkCache)}? linkCache");
            f.Add($"{nameof(AssetLinkQuery)} queryCategories");
        }
        using (fg.CurlyBrace())
        {
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasLinks(baseClass, includeBaseClass: true) != Case.No)
                {
                    fg.AppendLine($"base.{nameof(IAssetLinkContainer.RemapAssetLinks)}(obj, mapping, linkCache, queryCategories);");
                    break;
                }
            }
            
            if (obj.GetObjectData().HasInferredAssets)
            {
                fg.AppendLine("RemapInferredAssetLinks(obj, mapping, linkCache, queryCategories);");
            }
            
            if (obj.GetObjectData().HasResolvedAssets)
            {
                fg.AppendLine("RemapResolvedAssetLinks(obj, mapping, linkCache, queryCategories);");
            }
            
            var subFg = new StructuredStringBuilder();
            foreach (var field in obj.IterateFields(nonIntegrated: true))
            {
                if (field is AssetLinkType)
                {
                    subFg.AppendLine($"obj.{field.Name}{field.NullChar}.Relink(mapping);");
                }
                else if (field is WrapperType cont)
                {
                    if ((cont.SubTypeGeneration is LoquiType contLoqui
                         && await HasLinks(contLoqui, includeBaseClass: true) != Case.No))
                    {
                        fg.AppendLine(
                            $"obj.{field.Name}{field.NullChar}.ForEach(x => x{contLoqui.NullChar}.{nameof(IAssetLinkContainer.RemapAssetLinks)}(mapping, queryCategories));");
                    }
                    else if (cont.SubTypeGeneration is AssetLinkType subAsset)
                    {
                        fg.AppendLine(
                            $"obj.{field.Name}{field.NullChar}.ForEach(x => x{subAsset.NullChar}.Relink(mapping));");
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.Mode == DictMode.KeyedValue
                        && dict.ValueTypeGen is LoquiType dictLoqui
                        && await HasLinks(dictLoqui, includeBaseClass: true) != Case.No)
                    {
                        fg.AppendLine(
                            $"obj.{field.Name}{field.NullChar}.{nameof(IAssetLinkContainer.RemapAssetLinks)}(mapping, queryCategories);");
                    }
                    else if (dict.ValueTypeGen is FormLinkType formIDType)
                    {
                        fg.AppendLine(
                            $"obj.{field.Name}{field.NullChar}.{nameof(IAssetLinkContainer.RemapAssetLinks)}(mapping, queryCategories);");
                    }
                }
            }

            if (subFg.Count > 0)
            {
                fg.AppendLine($"if (query.HasFlag({nameof(AssetLinkQuery)}.{nameof(AssetLinkQuery.Listed)}))");
                using (fg.CurlyBrace())
                {
                    fg.AppendLines(subFg);
                }
            }
            
            foreach (var field in obj.IterateFields(nonIntegrated: true))
            {
                if (field is LoquiType loqui)
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
                    fg.AppendLine(
                        $"obj.{field.Name}{field.NullChar}.{nameof(IAssetLinkContainer.RemapAssetLinks)}(mapping, queryCategories, linkCache);");
                }
            }
        }

        fg.AppendLine();
    }


    private async Task YieldReturnListedAssets(ObjectGeneration obj, StructuredStringBuilder fg)
    {
        var startCount = fg.Count;
        foreach (var field in obj.IterateFields(nonIntegrated: true))
        {
            if (field is AssetLinkType link)
            {
                if (field.Nullable)
                {
                    fg.AppendLine($"if (obj.{field.Name} != null)");
                    using (fg.CurlyBrace())
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

                using (fg.CurlyBrace(doIt: doBrace))
                {
                    fg.AppendLine(
                        $"foreach (var item in {access}.{nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(queryCategories: queryCategories, linkCache: linkCache, assetType: assetType))");
                    using (fg.CurlyBrace())
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

                var subFg = new StructuredStringBuilder();
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
                                subFg.AppendLine(
                                    $"foreach (var item in {access}{filterNulls}.SelectMany(f => f.{nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(queryCategories: queryCategories, linkCache: linkCache, assetType: assetType)))");
                                break;
                            case Case.Maybe:
                                subFg.AppendLine(
                                    $"foreach (var item in {access}{filterNulls}.WhereCastable<{contLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainerGetter)}>()");
                                using (subFg.IncreaseDepth())
                                {
                                    subFg.AppendLine(
                                        $".SelectMany((f) => f.{nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(queryCategories: queryCategories, linkCache: linkCache, assetType: assetType)))");
                                }

                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
                else if (cont.SubTypeGeneration is AssetLinkType assetLinkType)
                {
                    string filterNulls =
                        assetLinkType.IsNullable || (cont is GenderedType && ((GenderedType)cont).ItemNullable)
                            ? ".NotNull()"
                            : null;
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

                using (fg.CurlyBrace(doIt: field.Nullable))
                {
                    fg.AppendLines(subFg);
                    using (fg.CurlyBrace())
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
                            fg.AppendLine(
                                $"foreach (var item in obj.{field.Name}.Items.SelectMany(f => f.{nameof(IAssetLinkContainer.EnumerateAssetLinks)}(linkCache, includeImplicit: includeImplicit))");
                            break;
                        case Case.Maybe:
                            fg.AppendLine(
                                $"foreach (var item in obj.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: true)}, {nameof(IAssetLinkContainerGetter)}>()");
                            using (fg.IncreaseDepth())
                            {
                                fg.AppendLine(
                                    $".SelectMany((f) => f.{nameof(IAssetLinkContainer.EnumerateAssetLinks)}(queryCategories: queryCategories, linkCache: linkCache, assetType: assetType)))");
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

                using (fg.CurlyBrace())
                {
                    fg.AppendLine($"yield return item;");
                }
            }
            else if (field is BreakType breakType)
            {
                if (fg.Count > startCount)
                {
                    fg.AppendLine(
                        $"if (obj.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index})) yield break;");
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
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder fg)
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

    public async Task GenerateInterfaceImplementation(ObjectGeneration obj, StructuredStringBuilder fg, bool getter)
    {
        var shouldAlwaysOverride = obj.IsTopLevelGroup() || obj.IsTopLevelListGroup();
        fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}IEnumerable<{nameof(IAssetLinkGetter)}> {nameof(IAssetLinkContainerGetter.EnumerateAssetLinks)}(AssetLinkQuery queryCategories, {nameof(IAssetLinkCache)}? linkCache, Type? assetType) => {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);");

        if (!getter)
        {
            fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}IEnumerable<{nameof(IAssetLink)}> {nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}() => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.{nameof(IAssetLinkContainer.EnumerateListedAssetLinks)}(this);");
            fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}void {nameof(IAssetLinkContainer.RemapAssetLinks)}(IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping, {nameof(AssetLinkQuery)} queryCategories, IAssetLinkCache? linkCache) => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.RemapAssetLinks(this, mapping, linkCache, queryCategories);");
            fg.AppendLine($"public{await obj.FunctionOverride(shouldAlwaysOverride, async (o) => await HasLinks(o, includeBaseClass: false) != Case.No)}void {nameof(IAssetLinkContainer.RemapListedAssetLinks)}(IReadOnlyDictionary<{nameof(IAssetLinkGetter)}, string> mapping) => {obj.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.Instance.RemapAssetLinks(this, mapping, null, {nameof(AssetLinkQuery)}.{nameof(AssetLinkQuery.Listed)});");
        }
    }
}