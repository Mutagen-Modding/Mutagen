using Mutagen.Bethesda.Plugins.Binary.Parameters;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    [Obsolete("Use ModFactory instead")]
    public static class ModInstantiator
    {
        /// <summary>
        /// Imports a mod as a read-only overlay
        /// </summary>
        [Obsolete("Use ModFactory.ImportGetter instead")]
        public static IModDisposeGetter Importer(ModPath path, GameRelease release, BinaryReadParameters? param = null)
        {
            return ModFactory.ImportGetter(path, release, param);
        }

        /// <summary>
        /// Imports a mod as a read-only overlay
        /// </summary>
        [Obsolete("Use ModFactory.ImportGetter instead")]
        public static IModDisposeGetter ImportGetter(ModPath path, GameRelease release, BinaryReadParameters? param = null)
        {
            return ModFactory.ImportGetter(path, release, param);
        }

        /// <summary>
        /// Imports a mod as a mutable mod
        /// </summary>
        [Obsolete("Use ModFactory.ImportSetter instead")]
        public static IMod ImportSetter(ModPath path, GameRelease release, BinaryReadParameters? param = null)
        {
            return ModFactory.ImportSetter(path, release, param);
        }

        /// <summary>
        /// Creates a new mod
        /// </summary>
        [Obsolete("Use ModFactory.Activator instead")]
        public static IMod Activator(ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false)
        {
            return ModFactory.Activator(modKey, release, headerVersion, forceUseLowerFormIDRanges);
        }
    }

    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    /// <typeparam name="TMod">
    /// Type of Mod to instantiate.  Can be the direct class, or one of its interfaces.
    /// </typeparam>
    [Obsolete("Use ModFactory<TMod> instead")]
    public static class ModInstantiator<TMod>
        where TMod : IModGetter
    {
        public delegate TMod ActivatorDelegate(ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false);
        public delegate TMod ImporterDelegate(ModPath modKey, GameRelease release, BinaryReadParameters? param = null);

        /// <summary>
        /// Function to call to retrieve a new Mod of type T
        /// </summary>
        [Obsolete("Use ModFactory<TMod>.Activator instead")]
        public static ActivatorDelegate Activator => (modKey, release, headerVersion, forceUseLowerFormIDRanges)
            => ModFactory<TMod>.Activator(modKey, release, headerVersion, forceUseLowerFormIDRanges);

        /// <summary>
        /// Function to call to import a new Mod of type T
        /// </summary>
        [Obsolete("Use ModFactory<TMod>.Importer instead")]
        public static ImporterDelegate Importer => (modKey, release, param)
            => ModFactory<TMod>.Importer(modKey, release, param);
    }
}
