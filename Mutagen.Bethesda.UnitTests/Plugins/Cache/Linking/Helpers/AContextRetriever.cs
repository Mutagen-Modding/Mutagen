using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;

public abstract class AContextRetriever
{
    public abstract IModContext? ResolveContext<TMajor, TMajorGetter>(
        IFormLinkGetter<TMajorGetter> formLink,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter;
        
    public abstract IModContext? ResolveContext<TMajorGetter, TScopedSetter, TScopedGetter>(
        IFormLinkGetter<TMajorGetter> formLink,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        where TMajorGetter : class, IMajorRecordGetter
        where TScopedSetter : class, TScopedGetter, IMajorRecord
        where TScopedGetter : class, TMajorGetter;
        
    public abstract bool TryResolveContext<TMajor, TMajorGetter>(
        IFormLinkGetter<TMajorGetter> formLink,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
        [MaybeNullWhen(false)] out IModContext context)
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter;
        
    public abstract bool TryResolveContext<TMajorGetter, TScopedSetter, TScopedGetter>(
        IFormLinkGetter<TMajorGetter> formLink,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
        [MaybeNullWhen(false)] out IModContext context)
        where TMajorGetter : class, IMajorRecordGetter
        where TScopedSetter : class, TScopedGetter, IMajorRecord
        where TScopedGetter : class, TMajorGetter;
        
    public abstract IEnumerable<IModContext> ResolveAllContexts<TMajor, TMajorGetter>(
        IFormLinkGetter<TMajorGetter> formLink,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter;
        
    public abstract IEnumerable<IModContext> ResolveAllContexts<TMajorGetter, TScopedSetter, TScopedGetter>(
        IFormLinkGetter<TMajorGetter> formLink,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        where TMajorGetter : class, IMajorRecordGetter
        where TScopedSetter : class, TScopedGetter, IMajorRecord
        where TScopedGetter : class, TMajorGetter;
}

class NormalContextRetriever : AContextRetriever
{
    public override IModContext? ResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, TMajor, TMajorGetter>(cache);
    }

    public override IModContext? ResolveContext<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(cache);
    }

    public override bool TryResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
        [MaybeNullWhen(false)] out IModContext context)
    {
        if (formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, TMajor, TMajorGetter>(cache, out var resolved))
        {
            context = resolved;
            return true;
        }

        context = default;
        return false;
    }

    public override bool TryResolveContext<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
        [MaybeNullWhen(false)] out IModContext context)
    {
        if (formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(cache, out var resolved))
        {
            context = resolved;
            return true;
        }

        context = default;
        return false;
    }

    public override IEnumerable<IModContext> ResolveAllContexts<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, TMajor, TMajorGetter>(cache);
    }

    public override IEnumerable<IModContext> ResolveAllContexts<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(cache);
    }
}

class SimpleContextRetriever : AContextRetriever
{
    public override IModContext? ResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveSimpleContext<TMajorGetter>(cache);
    }

    public override IModContext? ResolveContext<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveSimpleContext<TMajorGetter, TScopedGetter>(cache);
    }

    public override bool TryResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
        [MaybeNullWhen(false)] out IModContext context)
    {
        if (formLink.TryResolveSimpleContext<TMajorGetter>(cache, out var resolved))
        {
            context = resolved;
            return true;
        }

        context = default;
        return false;
    }

    public override bool TryResolveContext<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
        [MaybeNullWhen(false)] out IModContext context)
    {
        if (formLink.TryResolveSimpleContext<TMajorGetter, TScopedGetter>(cache, out var resolved))
        {
            context = resolved;
            return true;
        }

        context = default;
        return false;
    }
        
    public override IEnumerable<IModContext> ResolveAllContexts<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveAllSimpleContexts<TMajorGetter>(cache);
    }

    public override IEnumerable<IModContext> ResolveAllContexts<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
    {
        return formLink.ResolveAllSimpleContexts<TMajorGetter, TScopedGetter>(cache);
    }
}