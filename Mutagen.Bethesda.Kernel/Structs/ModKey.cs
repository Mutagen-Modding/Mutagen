using Mutagen.Bethesda.Kernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [DebuggerDisplay("ModKey {FileName}")]
    public struct ModKey : IEquatable<ModKey>
    {
        /// <summary>
        /// A static readonly singleton representing a null ModKey
        /// </summary>
        public static readonly ModKey Null = new ModKey(null!, type: ModType.Master);

        private readonly string? name_;
        private readonly int _hash;

        /// <summary>
        /// Mod name
        /// </summary>
        public string Name => name_ ?? string.Empty;
        
        /// <summary>
        /// Mod yype
        /// </summary>
        public ModType Type { get; private set; }
        
        /// <summary>
        /// Convenience accessor to get the appropriate file name
        /// </summary>
        public string FileName => this.ToString();

        private static readonly char[] InvalidChars = new char[] { '/', '\\' };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of mod</param>
        /// <param name="type">Type of mod</param>
        public ModKey(
            string name,
            ModType type)
        {
            if (name != null
                && -1 != name.IndexOfAny(InvalidChars))
            {
                throw new ArgumentException($"ModKey name contained path characters: {name}");
            }
            this.name_ = name == null ? null : string.Intern(name);
            this.Type = type;

            // Cache the hash on construction, as ModKeys are typically created rarely, but hashed often.
            var nameHash = (name_?.Equals(string.Empty) ?? true) ? 0 : name_.GetHashCode(StringComparison.OrdinalIgnoreCase);
            if (nameHash != 0)
            {
                HashCode hash = new HashCode();
                hash.Add(nameHash);
                hash.Add(Type);
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
            return this.Type == other.Type
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
            if (string.IsNullOrWhiteSpace(Name)) return "Null";
            return string.Create(Name.Length + 4, this, (chars, modKey) =>
            {
                modKey.Name.AsSpan().CopyTo(chars);
                chars[modKey.Name.Length] = '.';
                var suffix = modKey.Type switch
                {
                    ModType.Master => Constants.Esm,
                    ModType.Plugin => Constants.Esp,
                    ModType.LightMaster => Constants.Esl,
                    _ => throw new NotImplementedException()
                };
                suffix.AsSpan().CopyTo(chars.Slice(modKey.Name.Length + 1));
            });
        }

        /// <summary>
        /// Attempts to construct a ModKey from a string:
        ///   ModName.esp
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="modKey">ModKey if successfully converted</param>
        /// <returns>True if conversion successful</returns>
        public static bool TryFromNameAndExtension(ReadOnlySpan<char> str, [MaybeNullWhen(false)]out ModKey modKey)
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
            ModType type;
            if (endSpan.Equals(Constants.Esm.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                type = ModType.Master;
            }
            else if (endSpan.Equals(Constants.Esp.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                type = ModType.Plugin;
            }
            else if (endSpan.Equals(Constants.Esl.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                type = ModType.LightMaster;
            }
            else
            {
                modKey = default!;
                return false;
            }
            var modString = str.Slice(0, index).ToString();
            modKey = new ModKey(
                name: modString,
                type: type);
            return true;
        }

        /// <summary>
        /// Constructs a ModKey from a string:
        ///   ModName.esp
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>Converted ModKey</returns>
        /// <exception cref="ArgumentException">If string malformed</exception>
        public static ModKey FromNameAndExtension(ReadOnlySpan<char> str)
        {
            if (TryFromNameAndExtension(str, out var key))
            {
                return key;
            }
            if (TryFromNameAndExtension(Path.GetFileName(str), out key))
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

        public static implicit operator ModKey(string nameAndExtension)
        {
            return ModKey.FromNameAndExtension(nameAndExtension);
        }

        #region Comparers
        private class AlphabeticalMastersFirstComparer : Comparer<ModKey>
        {
            public readonly static AlphabeticalMastersFirstComparer Instance = new AlphabeticalMastersFirstComparer();
            public override int Compare(ModKey x, ModKey y)
            {
                if (x.Type == y.Type)
                {
                    return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
                }
                return x.Type.CompareTo(y.Type);
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
                var xIndex = IndexOf(_loadOrder, x);
                if (xIndex == -1)
                {
                    if (_notOnLoadOrderFallback != null)
                    {
                        return _notOnLoadOrderFallback.Compare(x, y);
                    }
                    throw new ArgumentOutOfRangeException($"ModKey was not on load order: {x}");
                }
                var yIndex = IndexOf(_loadOrder, y);
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

        // Ported from Noggog.CSharpExt to avoid import
        private static int IndexOf<T>(IReadOnlyList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var existing = list[i];
                if (EqualityComparer<T>.Default.Equals(existing, item))
                {
                    return i;
                }
            }
            return -1;
        }

        private static readonly Comparer<ModKey> _byTypeComparer = Comparer<ModKey>.Create((x, y) => x.Type.CompareTo(y.Type));
        public static Comparer<ModKey> ByTypeComparer => _byTypeComparer;
        #endregion
    }
}
