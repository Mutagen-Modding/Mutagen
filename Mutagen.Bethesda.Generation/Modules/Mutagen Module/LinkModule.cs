using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class LinkModule : GenerationModule
    {
        public enum LinkCase { No, Yes, Maybe }

        public async Task<LinkCase> HasLinks(LoquiType loqui, GenericSpecification specifications = null)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                var subCase = await HasLinks(loqui.TargetObjectGeneration, loqui.GenericSpecification);
                if (subCase == LinkCase.Yes) return LinkCase.Yes;
                return subCase;
            }
            else if (specifications != null)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    return await HasLinks(specObj);
                }
                return LinkCase.Maybe;
            }
            else
            {
                return LinkCase.Maybe;
            }
        }

        public async Task<LinkCase> HasLinks(ObjectGeneration obj, GenericSpecification specifications = null)
        {
            if (obj.Name == "MajorRecord") return LinkCase.Yes;
            if (obj.IterateFields().Any((f) => f is FormIDLinkType)) return LinkCase.Yes;
            LinkCase bestCase = LinkCase.No;
            foreach (var field in obj.IterateFields())
            {
                if (field is LoquiType loqui)
                {
                    var subCase = await HasLinks(loqui, specifications);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                else if (field is ContainerType cont
                    && cont.SubTypeGeneration is LoquiType contLoqui)
                {
                    var subCase = await HasLinks(contLoqui, specifications);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        var subCase = await HasLinks(valLoqui, specifications);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        var subCase = await HasLinks(keyLoqui, specifications);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                }
            }
            return bestCase;
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            var linkCase = await HasLinks(obj);
            if (linkCase == LinkCase.No) return;
            fg.AppendLine($"public{await obj.FunctionOverride(async (o) => (await HasLinks(o)) != LinkCase.No)}IEnumerable<ILink> Links => GetLinks();");

            fg.AppendLine("private IEnumerable<ILink> GetLinks()");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass) != LinkCase.No)
                    {
                        fg.AppendLine("foreach (var item in base.Links)");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return item;");
                        }
                        break;
                    }
                }
                foreach (var field in obj.IterateFields())
                {
                    if (field is FormIDLinkType)
                    {
                        fg.AppendLine($"yield return {field.ProtectedProperty};");
                    }
                    else if (field is LoquiType loqui)
                    {
                        LinkCase subLinkCase;
                        if (loqui.TargetObjectGeneration != null)
                        {
                            subLinkCase = await HasLinks(loqui);
                        }
                        else
                        {
                            subLinkCase = LinkCase.Maybe;
                        }
                        if (subLinkCase == LinkCase.No) continue;
                        var doBrace = true;
                        if (subLinkCase == LinkCase.Maybe)
                        {
                            fg.AppendLine($"if ({field.Name} is {nameof(ILinkContainer)} {field.Name}linkCont)");
                        }
                        else if (loqui.SingletonType == SingletonLevel.None)
                        {
                            fg.AppendLine($"if ({field.Name} != null)");
                        }
                        else
                        {
                            doBrace = false;
                        }
                        using (new BraceWrapper(fg, doIt: doBrace))
                        {
                            fg.AppendLine($"foreach (var item in {(subLinkCase == LinkCase.Maybe ? $"{field.Name}linkCont" : field.Name)}.Links)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return item;");
                            }
                        }
                    }
                    else if (field is ContainerType cont
                        && cont.SubTypeGeneration is LoquiType contLoqui
                        && await HasLinks(contLoqui) != LinkCase.No)
                    {
                        var linktype = await HasLinks(contLoqui);
                        switch (linktype)
                        {
                            case LinkCase.Yes:
                                fg.AppendLine($"foreach (var item in {field.Name}.SelectMany(f => f.Links))");
                                break;
                            case LinkCase.Maybe:
                                fg.AppendLine($"foreach (var item in Items.SelectWhere((f) => TryGet<ILinkContainer>.Create(successful: f is ILinkContainer, val: f as ILinkContainer))");
                                using (new DepthWrapper(fg))
                                {
                                    fg.AppendLine(".SelectMany((f) => f.Links))");
                                }
                                break;
                            default:
                                break;
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"yield return item;");
                        }
                    }
                    else if (field is DictType dict
                        && dict.Mode == DictMode.KeyedValue
                        && dict.ValueTypeGen is LoquiType dictLoqui
                        && await HasLinks(dictLoqui) != LinkCase.No)
                    {
                        var linktype = await HasLinks(dictLoqui);
                        switch (linktype)
                        {
                            case LinkCase.Yes:
                                fg.AppendLine($"foreach (var item in {field.Name}.SelectMany(f => f.Links))");
                                break;
                            case LinkCase.Maybe:
                                fg.AppendLine($"foreach (var item in Items.SelectWhere((f) => TryGet<ILinkContainer>.Create(successful: f is ILinkContainer, val: f as ILinkContainer))");
                                using (new DepthWrapper(fg))
                                {
                                    fg.AppendLine(".SelectMany((f) => f.Links))");
                                }
                                break;
                            default:
                                break;
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"yield return item;");
                        }
                    }
                }
                fg.AppendLine("yield break;");
            }
        }
    }
}
