using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers
{
    public abstract class AContextRetriever
    {
        public abstract IModContext? ResolveContext<TMajor, TMajorGetter>(
            IFormLinkGetter<TMajorGetter> formLink,
            ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;
        
        public abstract bool TryResolveContext<TMajor, TMajorGetter>(
            IFormLinkGetter<TMajorGetter> formLink,
            ILinkCache<ISkyrimMod, ISkyrimModGetter> cache,
            [MaybeNullWhen(false)] out IModContext context)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;
        
        public abstract IEnumerable<IModContext> ResolveAllContexts<TMajor, TMajorGetter>(
            IFormLinkGetter<TMajorGetter> formLink,
            ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;
        
        public abstract IEnumerable<IModContext> ResolveAllContexts<TMajorGetter, TScopedSetter, TScopedGetter>(
            IFormLinkGetter<TMajorGetter> formLink,
            ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
            where TMajorGetter : class, IMajorRecordCommonGetter
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter;
    }

    class NormalContextRetriever : AContextRetriever
    {
        public override IModContext? ResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        {
            return formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, TMajor, TMajorGetter>(cache);
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

        public override IEnumerable<IModContext> ResolveAllContexts<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        {
            return formLink.ResolveAllSimpleContexts<TMajorGetter>(cache);
        }

        public override IEnumerable<IModContext> ResolveAllContexts<TMajorGetter, TScopedSetter, TScopedGetter>(IFormLinkGetter<TMajorGetter> formLink, ILinkCache<ISkyrimMod, ISkyrimModGetter> cache)
        {
            return formLink.ResolveAllSimpleContexts<TMajorGetter, TScopedGetter>(cache);
        }
    }
}