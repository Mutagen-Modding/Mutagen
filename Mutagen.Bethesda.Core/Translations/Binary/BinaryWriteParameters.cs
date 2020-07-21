using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Parameter object for customizing binary export job instructions
    /// </summary>
    public class BinaryWriteParameters
    {
        public static BinaryWriteParameters Default = new BinaryWriteParameters();

        #region Mod Key Sync
        /// <summary>
        /// Flag to specify what logic to use to keep a mod's ModKey in sync with its path
        /// </summary>
        public enum ModKeyOption
        {
            /// <summary>
            /// Do no check
            /// </summary>
            NoCheck,

            /// <summary>
            /// If a mod's master flag does not match the path being exported to, throw
            /// </summary>
            ThrowIfMisaligned,

            /// <summary>
            /// If a mod's master flag does not match the path being exported to, modify it to match the path
            /// </summary>
            CorrectToPath,
        }

        /// <summary>
        /// Flag to specify what logic to use to keep a mod's ModKey in sync with its path
        /// </summary>
        public ModKeyOption ModKey = ModKeyOption.ThrowIfMisaligned;
        #endregion

        #region Masters List Sync
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

        /// <summary>
        /// Logic to use to keep a mod's master list content in sync<br/>
        /// This setting is just used to sync the contents of the list, not the order
        /// </summary>
        public MastersListContentOption MastersListContent = MastersListContentOption.Iterate;
        #endregion

        #region Record Count Sync
        /// <summary>
        /// Flag to specify what logic to use to keep a mod's record count in sync
        /// </summary>
        public enum RecordCountOption
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

        /// <summary>
        /// Logic to use to keep a mod's record count in sync
        /// </summary>
        public RecordCountOption RecordCount = RecordCountOption.Iterate;
        #endregion

        /// <summary>
        /// Optional StringsWriter override, for mods that are able to localize.
        /// </summary>
        public StringsWriter? StringsWriter;

        /// <summary>
        /// Aligns a mod's ModKey to a path's implied ModKey.
        /// Will adjust its logic based on the MasterFlagSync option:
        ///  - ThrowIfMisaligned:  If the path and mod do not match, throw.
        ///  - CorrectToPath:  If the path and mod do not match, use path's key.
        /// </summary>
        /// <param name="mod">Mod to check and adjust</param>
        /// <param name="path">Path to compare to</param>
        /// <returns>ModKey to use</returns>
        /// <exception cref="ArgumentException">If misaligned and set to ThrowIfMisaligned</exception>
        public ModKey RunMasterMatch(IModGetter mod, string path)
        {
            if (ModKey == ModKeyOption.NoCheck) return mod.ModKey;
            if (!Bethesda.ModKey.TryFactory(Path.GetFileName(path), out var pathModKey))
            {
                throw new ArgumentException($"Could not convert path to a ModKey to compare against: {Path.GetFileName(path)}");
            }
            switch (ModKey)
            {
                case ModKeyOption.ThrowIfMisaligned:
                    if (mod.ModKey != pathModKey)
                    {
                        throw new ArgumentException($"ModKeys were misaligned: {mod.ModKey} != {pathModKey}");
                    }
                    return mod.ModKey;
                case ModKeyOption.CorrectToPath:
                    return pathModKey;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
