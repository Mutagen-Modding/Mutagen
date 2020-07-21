using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// ModKey represents a unique identifier for a mod.  
    /// 
    /// The proper factory format is: [ModName].es[p/m], depending on whether it is a master file or not.
    /// 
    /// A correct ModKey is very important if a mod's contents will ever be added to another mod (as an override).
    /// Otherwise, records will become mis-linked.  The ModKey should typically be the name that the mod intends to be exported
    /// to disk with.  If a mod is not going to be exported, then any unique name is sufficient.
    /// 
    /// General practice is to use ModKey.TryFactory on a mod's file name when at all possible
    /// </summary>
    public struct ModKey : IEquatable<ModKey>
    {
        /// <summary>
        /// A static readonly singleton representing a null ModKey
        /// </summary>
        public static readonly ModKey Null = new ModKey(null!, master: false);

        private readonly string? name_;

        /// <summary>
        /// Mod name
        /// </summary>
        public string Name => name_ ?? string.Empty;
        
        /// <summary>
        /// Master flag
        /// </summary>
        public bool Master { get; private set; }
        
        /// <summary>
        /// Convenience accessor to get the appropriate file name
        /// </summary>
        public string FileName => this.ToString();
        
        private readonly int _hash;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of mod</param>
        /// <param name="master">True if mod is a master</param>
        public ModKey(
            string name,
            bool master)
        {
            this.name_ = name == null ? null : string.Intern(name);
            this.Master = master;

            // Cache the hash on construction, as ModKeys are typically created rarely, but hashed often.
            var nameHash = (name_?.Equals(string.Empty) ?? true) ? 0 : name_.GetHashCode(StringComparison.OrdinalIgnoreCase);
            if (nameHash != 0)
            {
                HashCode hash = new HashCode();
                hash.Add(nameHash);
                hash.Add(Master);
                this._hash = hash.ToHashCode();
            }
            else
            {
                this._hash = 0;
            }
        }

        /// <summary>
        /// ModKey equality operator
        /// Name is compared ignoring case
        /// </summary>
        /// <param name="other">ModKey to compare to</param>
        /// <returns>True equal Name and Master value</returns>
        public bool Equals(ModKey other)
        {
            return this.Master == other.Master
                && string.Equals(this.Name, other.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Default equality operator
        /// Name is compared ignoring case
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>True if ModKey with equal Name and Master value</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ModKey key)) return false;
            return Equals(key);
        }

        /// <summary>
        /// Hashcode retrieved from upper case Name and Master values.
        /// </summary>
        /// <returns>Hashcode retrieved from upper case Name and Master values.</returns>
        public override int GetHashCode()
        {
            return _hash;
        }
        
        /// <summary>
        /// Converts to a string: MyMod.esp
        /// </summary>
        /// <returns>String representation of ModKey</returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Null" : $"{Name}.{(this.Master ? Constants.Esm : Constants.Esp)}";
        }

        /// <summary>
        /// Attempts to construct a ModKey from a string:
        ///   ModName.esp
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="modKey">ModKey if successfully converted</param>
        /// <returns>True if conversion successful</returns>
        public static bool TryFactory(ReadOnlySpan<char> str, [MaybeNullWhen(false)]out ModKey modKey)
        {
            if (str.Length == 0 || str.IsWhiteSpace())
            {
                modKey = default!;
                return false;
            }
            var index = str.LastIndexOf('.');
            if (index == -1
                || index != str.Length - 4)
            {
                modKey = default!;
                return false;
            }
            var endSpan = str.Slice(index + 1);
            bool master;
            if (endSpan.Equals(Constants.Esm.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                master = true;
            }
            else if (endSpan.Equals(Constants.Esp.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                master = false;
            }
            else
            {
                modKey = default!;
                return false;
            }
            var modString = str.Slice(0, index).ToString();
            modKey = new ModKey(
                name: modString,
                master: master);
            return true;
        }

        /// <summary>
        /// Constructs a ModKey from a string:
        ///   ModName.esp
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>Converted ModKey</returns>
        /// <exception cref="ArgumentException">If string malformed</exception>
        public static ModKey Factory(ReadOnlySpan<char> str)
        {
            if (TryFactory(str, out var key))
            {
                return key;
            }
            if (TryFactory(Path.GetFileName(str), out key))
            {
                return key;
            }
            throw new ArgumentException("Could not construct ModKey.");
        }

        public static bool operator ==(ModKey? a, ModKey? b)
        {
            return EqualityComparer<ModKey?>.Default.Equals(a, b);
        }

        public static bool operator !=(ModKey? a, ModKey? b)
        {
            return !EqualityComparer<ModKey?>.Default.Equals(a, b);
        }

        #region Comparers
        private class AlphabeticalMastersFirstComparer : Comparer<ModKey>
        {
            public readonly static AlphabeticalMastersFirstComparer Instance = new AlphabeticalMastersFirstComparer();
            public override int Compare(ModKey x, ModKey y)
            {
                if (x.Master == y.Master)
                {
                    return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
                }
                return x.Master ? -1 : 1;
            }
        }

        public static Comparer<ModKey> AlphabeticalAndMastersFirst => AlphabeticalMastersFirstComparer.Instance;

        public static Comparer<ModKey> LoadOrderComparer(
            IReadOnlyList<ModKey> loadOrder,
            Comparer<ModKey>? matchingFallback = null)
        {
            return new ModKeyListComparer(loadOrder, matchingFallback);
        }

        private class ModKeyListComparer : Comparer<ModKey>
        {
            private readonly IReadOnlyList<ModKey> _loadOrder;
            private readonly Comparer<ModKey>? _notOnLoadOrderFallback;

            public ModKeyListComparer(
                IReadOnlyList<ModKey> loadOrder,
                Comparer<ModKey>? notOnLoadOrderFallback)
            {
                _loadOrder = loadOrder;
                _notOnLoadOrderFallback = notOnLoadOrderFallback;
            }

            public override int Compare(ModKey x, ModKey y)
            {
                if (x == y) return 0;
                var xIndex = _loadOrder.IndexOf(x);
                if (xIndex == -1)
                {
                    if (_notOnLoadOrderFallback != null)
                    {
                        return _notOnLoadOrderFallback.Compare(x, y);
                    }
                    throw new ArgumentOutOfRangeException($"ModKey was not on load order: {x}");
                }
                var yIndex = _loadOrder.IndexOf(y);
                if (yIndex == -1)
                {
                    if (_notOnLoadOrderFallback != null)
                    {
                        return _notOnLoadOrderFallback.Compare(x, y);
                    }
                    throw new ArgumentOutOfRangeException($"ModKey was not on load order: {y}");
                }
                return xIndex.CompareTo(yIndex);
            }
        }
        #endregion
    }
}
