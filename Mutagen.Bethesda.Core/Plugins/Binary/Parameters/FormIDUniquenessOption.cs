namespace Mutagen.Bethesda.Plugins.Binary.Parameters
{
    /// <summary>
    /// Flag to specify what logic to use to ensure a mod's formIDs are unique
    /// </summary>
    public enum FormIDUniquenessOption
    {
        /// <summary>
        /// Do no check
        /// </summary>
        NoCheck,

        /// <summary>
        /// Iterate source mod
        /// </summary>
        Iterate,
    }
}