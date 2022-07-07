using Loqui.Generation;
using DictType = Loqui.Generation.DictType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public abstract class AContainedLinksModule<TLinkType> : GenerationModule
    where TLinkType : TypeGeneration
{
    public virtual async Task<Case> HasLinks(LoquiType loqui, bool includeBaseClass, GenericSpecification? specifications = null)
    {
        if (specifications != null)
        {
            foreach (var target in specifications.Specifications.Values)
            {
                if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                return await HasLinks(specObj, includeBaseClass);
            }
        }
        if (loqui.TargetObjectGeneration != null)
        {
            return await HasLinks(loqui.TargetObjectGeneration, includeBaseClass, loqui.GenericSpecification);
        }
        else
        {
            return Case.Maybe;
        }
    }
    
    public virtual async Task<Case> HasLinks(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
    {
        if (obj.Name == "MajorRecord") return Case.Yes;
        if (obj.IterateFields(includeBaseClass: includeBaseClass).Any((f) => f is TLinkType)) return Case.Yes;
        Case bestCase = Case.No;
        foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
        {
            if (field is LoquiType loqui)
            {
                var subCase = await HasLinks(loqui, includeBaseClass: true, specifications);
                if (subCase > bestCase)
                {
                    bestCase = subCase;
                }
            }
            else if (field is ContainerType cont)
            {
                if (cont.SubTypeGeneration is LoquiType contLoqui)
                {
                    var subCase = await HasLinks(contLoqui, includeBaseClass: true, specifications);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                else if (cont.SubTypeGeneration is TLinkType)
                {
                    return Case.Yes;
                }
            }
            else if (field is DictType dict)
            {
                if (dict.ValueTypeGen is LoquiType valLoqui)
                {
                    var subCase = await HasLinks(valLoqui, includeBaseClass: true, specifications);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                if (dict.KeyTypeGen is LoquiType keyLoqui)
                {
                    var subCase = await HasLinks(keyLoqui, includeBaseClass: true, specifications);
                    if (subCase > bestCase)
                    {
                        bestCase = subCase;
                    }
                }
                if (dict.ValueTypeGen is TLinkType)
                {
                    return Case.Yes;
                }
            }
        }

        // If no, check subclasses
        if (bestCase == Case.No)
        {
            foreach (var inheritingObject in await obj.InheritingObjects())
            {
                var subCase = await HasLinks(inheritingObject, includeBaseClass: false, specifications: specifications);
                if (subCase != Case.No) return Case.Maybe;
            }
        }

        return bestCase;
    }

}