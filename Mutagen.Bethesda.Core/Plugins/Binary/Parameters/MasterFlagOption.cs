namespace Mutagen.Bethesda.Plugins.Binary.Parameters
{
    /// <summary>
    /// Flag to specify what logic to use to ensure a mod's master flag matches the specified ModKey
    /// </summary>
    public enum MasterFlagOption
    {
        /// <summary>
        /// Do no check
        /// </summary>
        NoCheck,

        /// <summary>
        /// Changes master flags to match the ModKey type<br/>
        /// The master flag will be modified to be on if ModKey.Type is Master.<br/>
        /// The light master flag will be modified unless ModKey.Type is Plugin, in which case the flag will be left alone.<br/>
        /// </summary>
        ChangeToMatchModKey,

        /// <summary>
        /// Changes master flags to match the ModKey type<br/>
        /// The master flag will be modified to be on if ModKey.Type is Master.<br/>
        /// The light master flag will be modified unless ModKey.Type is Plugin, in which case the flag will be left alone.<br/>
        /// </summary>
        ExceptionOnMismatch,
    }
}