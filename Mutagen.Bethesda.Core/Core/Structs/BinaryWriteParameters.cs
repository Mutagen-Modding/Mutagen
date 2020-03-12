using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda
{
    public class BinaryWriteParameters
    {
        public static BinaryWriteParameters Default = new BinaryWriteParameters();

        /// <summary>
        /// Flag to specify what logic to use to keep a mod's master flag in sync
        /// </summary>
        public enum MasterFlagSyncOption
        {
            /// <summary>
            // Do no check
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
        /// Flag to specify what logic to use to keep a mod's master list in sync
        /// </summary>
        public enum MastersListSyncOption
        {
            /// <summary>
            // Do no check
            /// </summary>
            NoCheck,
            /// <summary>
            /// Iterate source mod before writing to compile the list of masters to use.
            /// </summary>
            Iterate,
        }

        /// <summary>
        /// Logic to use to keep a mod's master flag in sync
        /// </summary>
        public MasterFlagSyncOption MasterFlagSync = MasterFlagSyncOption.ThrowIfMisaligned;

        /// <summary>
        /// Logic to use to keep a mod's master list in sync
        /// </summary>
        public MastersListSyncOption MastersListSync = MastersListSyncOption.Iterate;

        public ModKey RunMasterMatch(IModGetter mod, string path)
        {
            if (MasterFlagSync == MasterFlagSyncOption.NoCheck) return mod.ModKey;
            if (!ModKey.TryFactory(Path.GetFileName(path), out var pathModKey))
            {
                throw new ArgumentException($"Could not convert path to a ModKey to compare against: {Path.GetFileName(path)}");
            }
            switch (MasterFlagSync)
            {
                case MasterFlagSyncOption.ThrowIfMisaligned:
                    if (mod.ModKey != pathModKey)
                    {
                        throw new ArgumentException($"ModKeys were misaligned: {mod.ModKey} != {pathModKey}");
                    }
                    return mod.ModKey;
                case MasterFlagSyncOption.CorrectToPath:
                    return pathModKey;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
