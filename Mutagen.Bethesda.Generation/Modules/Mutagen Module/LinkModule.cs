using Loqui;
using Loqui.Generation;
using Noggog;
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

        public override async Task<IEnumerable<(LoquiInterfaceType Location, string Interface)>> Interfaces(ObjectGeneration obj)
        {
            if (await HasLinks(obj, includeBaseClass: false) != LinkCase.No)
            {
                return (LoquiInterfaceType.IGetter, $"{nameof(ILinkContainer)}").Single();
            }
            return Enumerable.Empty<(LoquiInterfaceType Location, string Interface)>();
        }

        public static async Task<LinkCase> HasLinks(LoquiType loqui, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                return await HasLinks(loqui.TargetObjectGeneration, includeBaseClass, loqui.GenericSpecification);
            }
            else if (specifications != null)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    return await HasLinks(specObj, includeBaseClass);
                }
                return LinkCase.Maybe;
            }
            else
            {
                return LinkCase.Maybe;
            }
        }

        public static async Task<LinkCase> HasLinks(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (obj.Name == "MajorRecord") return LinkCase.Yes;
            if (obj.IterateFields(includeBaseClass: includeBaseClass).Any((f) => f is FormLinkType)) return LinkCase.Yes;
            LinkCase bestCase = LinkCase.No;
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    var subCase = await HasLinks(loqui, includeBaseClass, specifications);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType contLoqui)
                    {
                        var subCase = await HasLinks(contLoqui, includeBaseClass, specifications);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                    else if (cont.SubTypeGeneration is FormLinkType)
                    {
                        return LinkCase.Yes;
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        var subCase = await HasLinks(valLoqui, includeBaseClass, specifications);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        var subCase = await HasLinks(keyLoqui, includeBaseClass, specifications);
                        if (subCase > bestCase)
                        {
                            bestCase = subCase;
                        }
                    }
                    if (dict.ValueTypeGen is FormLinkType)
                    {
                        return LinkCase.Yes;
                    }
                }
            }

            // If no, check subclasses
            if (bestCase == LinkCase.No)
            {
                foreach (var inheritingObject in await obj.InheritingObjects())
                {
                    var subCase = await HasLinks(inheritingObject, includeBaseClass: false, specifications: specifications);
                    if (subCase != LinkCase.No) return LinkCase.Maybe;
                }
            }

            return bestCase;
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            if (!maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class)) return;
            fg.AppendLine($"public IEnumerable<{nameof(ILinkGetter)}> GetLinks({obj.Interface(getter: true)} obj)");
            using (new BraceWrapper(fg))
            {
                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (await HasLinks(baseClass, includeBaseClass: true) != LinkCase.No)
                    {
                        fg.AppendLine("foreach (var item in base.GetLinks(obj))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("yield return item;");
                        }
                        break;
                    }
                }
                foreach (var field in obj.IterateFields())
                {
                    if (field is FormLinkType)
                    {
                        fg.AppendLine($"yield return obj.{field.Name};");
                    }
                    else if (field is LoquiType loqui)
                    {
                        LinkCase subLinkCase;
                        if (loqui.TargetObjectGeneration != null)
                        {
                            subLinkCase = await HasLinks(loqui, includeBaseClass: true);
                        }
                        else
                        {
                            subLinkCase = LinkCase.Maybe;
                        }
                        if (subLinkCase == LinkCase.No) continue;
                        var doBrace = true;
                        if (subLinkCase == LinkCase.Maybe)
                        {
                            fg.AppendLine($"if (obj.{field.Name} is {nameof(ILinkContainer)} {field.Name}linkCont)");
                        }
                        else if (loqui.SingletonType == SingletonLevel.None)
                        {
                            fg.AppendLine($"if (obj.{field.Name} != null)");
                        }
                        else
                        {
                            doBrace = false;
                        }
                        using (new BraceWrapper(fg, doIt: doBrace))
                        {
                            fg.AppendLine($"foreach (var item in {(subLinkCase == LinkCase.Maybe ? $"{field.Name}linkCont" : $"obj.{field.Name}")}.Links)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"yield return item;");
                            }
                        }
                    }
                    else if (field is ContainerType cont)
                    {
                        if (cont.SubTypeGeneration is LoquiType contLoqui
                            && await HasLinks(contLoqui, includeBaseClass: true) != LinkCase.No)
                        {
                            var linktype = await HasLinks(contLoqui, includeBaseClass: true);
                            switch (linktype)
                            {
                                case LinkCase.Yes:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.SelectMany(f => f.Links))");
                                    break;
                                case LinkCase.Maybe:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.WhereCastable<{contLoqui.TypeName(getter: true)}, ILinkContainer>()");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine(".SelectMany((f) => f.Links))");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (cont.SubTypeGeneration is FormLinkType formIDType)
                        {
                            fg.AppendLine($"foreach (var item in obj.{field.Name})");
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
                    else if (field is DictType dict)
                    {
                        if (dict.Mode == DictMode.KeyedValue
                            && dict.ValueTypeGen is LoquiType dictLoqui
                            && await HasLinks(dictLoqui, includeBaseClass: true) != LinkCase.No)
                        {
                            var linktype = await HasLinks(dictLoqui, includeBaseClass: true);
                            switch (linktype)
                            {
                                case LinkCase.Yes:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.SelectMany(f => f.Links))");
                                    break;
                                case LinkCase.Maybe:
                                    fg.AppendLine($"foreach (var item in obj.{field.Name}.Items.WhereCastable<{dictLoqui.TypeName(getter: true)}, ILinkContainer>()");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine(".SelectMany((f) => f.Links))");
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
                }
                fg.AppendLine("yield break;");
            }
            fg.AppendLine();
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (obj.GetObjectType() != ObjectType.Mod)
            {
                var linkCase = await HasLinks(obj, includeBaseClass: false);
                if (linkCase == LinkCase.No) return;
            }
            fg.AppendLine($"[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            fg.AppendLine($"public{await obj.FunctionOverride(async (o) => (await HasLinks(o, includeBaseClass: false)) != LinkCase.No)}IEnumerable<{nameof(ILinkGetter)}> Links => {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.GetLinks(this);");
        }
    }
}
