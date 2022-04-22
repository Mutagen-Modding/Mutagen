using Mutagen.Bethesda.Kernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Noggog;

namespace Mutagen.Bethesda.Plugins;

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
public readonly struct ModKey : IEquatable<ModKey>
{
    /// <summary>
    /// A static readonly singleton representing a null ModKey
    /// </summary>
    public static readonly ModKey Null = new ModKey(null!, type: ModType.Master);

    private readonly string? _name;
    private readonly int _hash;

    /// <summary>
    /// Mod name
    /// </summary>
    public string Name => _name ?? string.Empty;
        
    /// <summary>
    /// Mod type
    /// </summary>
    public ModType Type { get; }
        
    /// <summary>
    /// Convenience accessor to get the appropriate file name
    /// </summary>
    public FileName FileName => new FileName(this.ToString(), check: false);

    private static readonly char[] InvalidChars = new char[]
    {
        '\"', '<', '>', '|', '\0',
        (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
        (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
        (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
        (char)31, ':', '*', '?', '\\', '/'
    };

    public bool IsNull => string.IsNullOrWhiteSpace(_name);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of mod</param>
    /// <param name="type">Type of mod</param>
    public ModKey(
        string? name,
        ModType type)
    {
        if (name != null
            && HasInvalidCharacters(name))
        {
            throw new ArgumentException($"ModKey name contained path characters: {name}", nameof(name));
        }
        this._name = name == null ? null : string.Intern(name);
        this.Type = type;

        // Cache the hash on construction, as ModKeys are typically created rarely, but hashed often.
        var nameHash = (_name?.Equals(string.Empty) ?? true) ? 0 : _name.GetHashCode(StringComparison.OrdinalIgnoreCase);
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

    public static bool HasInvalidCharacters(ReadOnlySpan<char> str)
    {
        return -1 != str.IndexOfAny(InvalidChars);
    }

    /// <summary>
    /// ModKey equality operator
    /// Name is compared ignoring case
    /// </summary>
    /// <param name="other">ModKey to compare to</param>
    /// <returns>True equal Name and Master value</returns>
    public bool Equals(ModKey other)
    {
        return (IsNull && other.IsNull)
               || (this.Type == other.Type
                   && string.Equals(this.Name, other.Name, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Default equality operator
    /// Name is compared ignoring case
    /// </summary>
    /// <param name="obj">object to compare to</param>
    /// <returns>True if ModKey with equal Name and Master value</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not ModKey key) return false;
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
            modKey.Type.GetFileExtension().AsSpan().CopyTo(chars.Slice(modKey.Name.Length + 1));
        });
    }

    /// <summary>
    /// Attempts to construct a ModKey from a string:
    ///   ModName.esp
    /// </summary>
    /// <param name="str">String to parse</param>
    /// <param name="modKey">ModKey if successfully converted</param>
    /// <param name="errorReason">Reason for a failed conversion</param>
    /// <returns>True if conversion successful</returns>
    public static bool TryFromNameAndExtension(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModKey modKey, out string errorReason)
    {
        if (str.Length == 0 || str.IsWhiteSpace())
        {
            modKey = default;
            errorReason = "Input string was empty or all whitespace";
            return false;
        }
        var index = str.LastIndexOf('.');
        if (index == -1
            || index != str.Length - 4)
        {
            modKey = default;
            errorReason = "Could not locate file extension";
            return false;
        }
        if (!TryConvertExtensionToType(str.Slice(index + 1), out var type))
        {
            modKey = default;
            errorReason = $"Extension could not be converted to a ModType: {str.Slice(index + 1).ToString()}";
            return false;
        }
        return TryFromName(str.Slice(0, index), type, out modKey, out errorReason);
    }

    /// <summary>
    /// Attempts to construct a ModKey from a string:
    ///   ModName.esp
    /// </summary>
    /// <param name="str">String to parse</param>
    /// <param name="modKey">ModKey if successfully converted</param>
    /// <returns>True if conversion successful</returns>
    public static bool TryFromNameAndExtension(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModKey modKey)
    {
        return TryFromNameAndExtension(str, out modKey, out _);
    }

    /// <summary>
    /// Attempts to construct a ModKey from a string:
    ///   ModName.esp
    /// </summary>
    /// <param name="fileName">FileName to parse</param>
    /// <param name="modKey">ModKey if successfully converted</param>
    /// <param name="errorReason">Reason for a failed conversion</param>
    /// <returns>True if conversion successful</returns>
    public static bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModKey modKey, out string errorReason)
    {
        return TryFromNameAndExtension(fileName.String, out modKey, out errorReason);
    }

    /// <summary>
    /// Attempts to construct a ModKey from a string:
    ///   ModName.esp
    /// </summary>
    /// <param name="fileName">FileName to parse</param>
    /// <param name="modKey">ModKey if successfully converted</param>
    /// <returns>True if conversion successful</returns>
    public static bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModKey modKey)
    {
        return TryFromFileName(fileName, out modKey, out _);
    }

    public static bool TryFromName(ReadOnlySpan<char> str, ModType type, [MaybeNullWhen(false)] out ModKey modKey, out string errorReason)
    {
        var invalidIndex = str.IndexOfAny(InvalidChars);
        if (invalidIndex != -1)
        {
            modKey = default;
            errorReason = $"Mod name contained an invalid character: {InvalidChars[invalidIndex]}";
            return false;
        }
        modKey = new ModKey(
            name: str.ToString(),
            type: type);
        errorReason = string.Empty;
        return true;
    }

    public static bool TryFromName(ReadOnlySpan<char> str, ModType type, [MaybeNullWhen(false)] out ModKey modKey)
    {
        return TryFromName(str, type, out modKey, out _);
    }

    public static ModKey FromName(ReadOnlySpan<char> str, ModType type)
    {
        if (TryFromName(str, type, out var modKey, out _)) return modKey;
        throw new ArgumentException("Could not construct ModKey.", nameof(str));
    }

    public static bool TryConvertExtensionToType(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModType modType)
    {
        if (str.Equals(Constants.Esm.AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            modType = ModType.Master;
        }
        else if (str.Equals(Constants.Esp.AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            modType = ModType.Plugin;
        }
        else if (str.Equals(Constants.Esl.AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            modType = ModType.LightMaster;
        }
        else
        {
            modType = default!;
            return false;
        }
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
        throw new ArgumentException("Could not construct ModKey.", nameof(str));
    }

    /// <summary>
    /// Constructs a ModKey from a string:
    ///   ModName.esp
    /// </summary>
    /// <param name="fileName">FileName to parse</param>
    /// <returns>Converted ModKey</returns>
    /// <exception cref="ArgumentException">If string malformed</exception>
    public static ModKey FromFileName(FileName fileName)
    {
        return FromNameAndExtension(fileName.String);
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

    public static implicit operator ModKey(FileName fileName)
    {
        return ModKey.FromFileName(fileName);
    }

    public static implicit operator FileName(ModKey modKey)
    {
        return modKey.FileName;
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

    private static readonly Comparer<ModKey> _alphabetical = Comparer<ModKey>.Create((x, y) => x.FileName.String.CompareTo(y.FileName.String));
    public static Comparer<ModKey> Alphabetical => _alphabetical;
    #endregion
}