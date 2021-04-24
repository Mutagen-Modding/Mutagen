using Mutagen.Bethesda.LoadOrders;
using Mutagen.Bethesda.Records.Binary.Utility;
using Mutagen.Bethesda.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Records.Binary
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

        #region Masters List Ordering
        /// <summary>
        /// Flag to specify what logic to use to keep a mod's master list keys in order<br/>
        /// This setting is just used to sync the order of the list, not the content
        /// </summary>
        public enum MastersListOrderingOption
        {
            /// <summary>
            /// Do no check
            /// </summary>
            NoCheck,

            /// <summary>
            /// Simply confirms masters come before other mod types
            /// </summary>
            MastersFirst,
        }

        /// <summary>
        /// Logic to use to keep a mod's master list ordering in sync<br/>
        /// This setting is just used to sync the order of the list, not the content
        /// </summary>
        public AMastersListOrderingOption MastersListOrdering { get; set; } = new MastersListOrderingEnumOption();

        /// <summary>
        /// An abstract class representing a logic choice for ordering masters
        /// </summary>
        public abstract class AMastersListOrderingOption
        {
            public static implicit operator AMastersListOrderingOption(MastersListOrderingOption option)
            {
                return new MastersListOrderingEnumOption()
                {
                    Option = option
                };
            }

            public static implicit operator AMastersListOrderingOption(MasterReferenceReader reader)
            {
                return new MastersListOrderingByLoadOrder(reader.Masters.Select(m => m.Master));
            }
        }

        /// <summary>
        /// A class that instructs the export to make masters listings match a given load order.
        /// </summary>
        public class MastersListOrderingByLoadOrder : AMastersListOrderingOption
        {
            private readonly List<ModKey> _modKeys;

            public IReadOnlyList<ModKey> LoadOrder => _modKeys;

            /// <summary>
            /// Whether to throw an exception if an unknown mod during export that is not on the given load order
            /// </summary>
            public bool Strict { get; set; }

            public MastersListOrderingByLoadOrder(IEnumerable<ModKey> modKeys)
            {
                _modKeys = modKeys.ToList();
            }

            public static MastersListOrderingByLoadOrder Factory(IEnumerable<ModKey> modKeys) => new MastersListOrderingByLoadOrder(modKeys);
            
            public static MastersListOrderingByLoadOrder Factory<T>(LoadOrder<T> loadOrder)
                where T : IModKeyed
            {
                return Factory(loadOrder.Select(listing => listing.Key));
            }
        }

        public class MastersListOrderingEnumOption : AMastersListOrderingOption
        {
            public MastersListOrderingOption Option { get; set; } = MastersListOrderingOption.MastersFirst;
        }
        #endregion

        #region Next Form ID
        /// <summary>
        /// Flag to specify what logic to use to keep a mod header's next formID field in sync
        /// </summary>
        public enum NextFormIDOption
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
        /// Logic to use to keep a mod header's next formID field in sync
        /// </summary>
        public NextFormIDOption NextFormID { get; set; } = NextFormIDOption.Iterate;
        #endregion

        #region Form ID Uniqueness
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

        /// <summary>
        /// Logic to use to ensure a mod's formIDs are unique.<br/>
        /// If there is a collision, an ArgumentException will be thrown.
        /// </summary>
        public FormIDUniquenessOption FormIDUniqueness { get; set; } = FormIDUniquenessOption.Iterate;
        #endregion

        #region Master Flag
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

        /// <summary>
        /// Logic to use to ensure a mod's master flag matches the specified ModKey
        /// </summary>
        public MasterFlagOption MasterFlag { get; set; } = MasterFlagOption.ChangeToMatchModKey;
        #endregion

        #region ESL FormID Limit
        /// <summary>
        /// Flag to specify what logic to use to ensure a light master mod does not go over its FormID count
        /// </summary>
        public enum LightMasterLimitOption
        {
            /// <summary>
            /// Do no check
            /// </summary>
            NoCheck,
            /// <summary>
            /// Will throw an exception if a light master 
            /// </summary>
            ExceptionOnOverflow,
        }

        /// <summary>
        /// Logic to use to ensure a light master mod does not go over its FormID count
        /// </summary>
        public LightMasterLimitOption LightMasterLimit { get; set; } = LightMasterLimitOption.ExceptionOnOverflow;
        #endregion

        /// <summary>
        /// Optional StringsWriter override, for mods that are able to localize.
        /// </summary>
        public StringsWriter? StringsWriter { get; set; }

        /// <summary>
        /// If a FormID has all zeros for the ID, but a non-zero mod index, then set mod index to zero as well.
        /// </summary>
        public bool CleanNulls { get; set; } = true;

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
            if (!Bethesda.ModKey.TryFromNameAndExtension(Path.GetFileName(path), out var pathModKey))
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
