namespace Mutagen.Bethesda.Plugins.Binary.Parameters
{
    /// <summary>
    /// Flag to specify what logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    public enum MastersListContentOption
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