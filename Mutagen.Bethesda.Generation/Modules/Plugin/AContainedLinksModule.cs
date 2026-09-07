using Loqui.Generation;
using DictType = Loqui.Generation.DictType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public abstract class AContainedLinksModule<TLinkType> : GenerationModule
    where TLinkType : TypeGeneration
{
    /// <summary>
    /// What a circular field (GetFieldData().Circular == true) contributes to its
    /// containing object's HasLinks determination. We never recurse into a circular
    /// field's referenced type (that's exactly the generation-time infinite recursion
    /// the "circular" marker exists to prevent), so this is a static, per-link-kind
    /// answer to "could this unknown field plausibly contain a link of this kind?" —
    /// not something derivable from the field itself. Default preserves the original
    /// behavior (contributes nothing); override where a concrete circular field in the
    /// XML is known to carry that kind of link (e.g. FormLinks via ScriptEntryStructs.Members).
    /// </summary>
    protected virtual Case CircularFieldCase => Case.No;

    public virtual async Task<Case> HasLinks(LoquiType loqui, bool includeBaseClass, GenericSpecification? specifications = null)
    {
        if (specifications != null)
        {
            foreach (var target in specifications.Specifications.Values)
            {
                if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                if (!loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey.TryGetValue(key, out var specObj)) continue;
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
            if (field.GetFieldData().Circular)
            {
                // Recursing into a circular field's referenced type risks infinite
                // generation-time recursion (the type graph loops back to this object).
                // We can't safely recurse, so subclasses decide what an unknown circular
                // field contributes (see CircularFieldCase) instead of this shared base
                // silently excluding it for every link kind.
                if (CircularFieldCase > bestCase)
                {
                    bestCase = CircularFieldCase;
                }
                continue;
            }
            if (field is LoquiType loqui)
            {
                var subCase = await HasLinks(loqui, includeBaseClass: true, specifications);
                if (subCase > bestCase)
                {
                    bestCase = subCase;
                }
            }
            else if (field is WrapperType cont)
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