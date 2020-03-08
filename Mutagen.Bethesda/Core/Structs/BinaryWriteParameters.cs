using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda
{
    public class BinaryWriteParameters
    {
        public static BinaryWriteParameters Default = new BinaryWriteParameters();

        public enum MasterFlagMatchOption
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
        /// Parameter to specify what logic to use to keep the mod's master flag in sync
        /// </summary>
        public MasterFlagMatchOption MasterFlagMatch = MasterFlagMatchOption.ThrowIfMisaligned;

        public ModKey RunMasterMatch(IModGetter mod, string path)
        {
            if (MasterFlagMatch == MasterFlagMatchOption.NoCheck) return mod.ModKey;
            if (!ModKey.TryFactory(Path.GetFileName(path), out var pathModKey))
            {
                throw new ArgumentException($"Could not convert path to a ModKey to compare against: {Path.GetFileName(path)}");
            }
            switch (MasterFlagMatch)
            {
                case MasterFlagMatchOption.ThrowIfMisaligned:
                    if (mod.ModKey != pathModKey)
                    {
                        throw new ArgumentException($"ModKeys were misaligned: {mod.ModKey} != {pathModKey}");
                    }
                    return mod.ModKey;
                case MasterFlagMatchOption.CorrectToPath:
                    return pathModKey;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
