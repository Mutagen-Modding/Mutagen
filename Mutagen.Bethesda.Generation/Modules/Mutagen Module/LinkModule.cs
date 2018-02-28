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

        public async Task<LinkCase> HasLinks(ObjectGeneration obj)
        {
            if (obj.Name == "MajorRecord") return LinkCase.Yes;
            if (obj.IterateFields().Any((f) => f is FormIDLinkType)) return LinkCase.Yes;
            LinkCase bestCase = LinkCase.No;
            foreach (var field in obj.IterateFields())
            {
                if (field is LoquiType loqui)
                {
                    var subCase = await AnalyzeLoqui(loqui);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                else if (field is ContainerType cont
                    && cont.SubTypeGeneration is LoquiType contLoqui)
                {
                    var subCase = await AnalyzeLoqui(contLoqui);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        var subCase = await AnalyzeLoqui(valLoqui);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        var subCase = await AnalyzeLoqui(keyLoqui);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                }
            }
            return bestCase;
        }

        private async Task<LinkCase> AnalyzeLoqui(LoquiType loqui)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                var subCase = await HasLinks(loqui.TargetObjectGeneration);
                if (subCase == LinkCase.Yes) return LinkCase.Yes;
                return subCase;
            }
            else
            {
                return LinkCase.Maybe;
            }
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
                            subLinkCase = await HasLinks(loqui.TargetObjectGeneration);
                        }
                        else
                        {
                            subLinkCase = LinkCase.Maybe;
                        }
                        if (subLinkCase == LinkCase.No) continue;
                        if (subLinkCase == LinkCase.Maybe)
                        {
                            fg.AppendLine($"if ({field.Name} is {nameof(ILinkContainer)} linkCont)");
                        }
                        using (new BraceWrapper(fg, doIt: subLinkCase == LinkCase.Maybe))
                        {
                            fg.AppendLine($"foreach (var item in {(subLinkCase == LinkCase.Maybe ? "linkCont" : field.Name)}.Links)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return item;");
                            }
                        }
                    }
                    else if (field is ContainerType cont
                        && cont.SubTypeGeneration is LoquiType contLoqui
                        && await AnalyzeLoqui(contLoqui) != LinkCase.No)
                    {
                        fg.AppendLine($"foreach (var item in {field.Name}.SelectMany(f => f.Links))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"yield return item;");
                        }
                    }
                    else if (field is DictType dict
                        && dict.Mode == DictMode.KeyedValue
                        && dict.ValueTypeGen is LoquiType dictLoqui
                        && await AnalyzeLoqui(dictLoqui) != LinkCase.No)
                    {
                        fg.AppendLine($"foreach (var item in {field.Name}.Values.SelectMany(f => f.Links))");
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
