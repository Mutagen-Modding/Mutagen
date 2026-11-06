using System.Collections.Concurrent;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// A Papyrus type: one of the four predefined element types or a script type,
/// optionally an array. Type names are compared case-insensitively.
/// </summary>
public sealed class TypeSymbol : IEquatable<TypeSymbol>
{
    private const string StringName = "string";
    private const string BoolName = "bool";
    private const string IntName = "int";
    private const string FloatName = "float";

    public static readonly TypeSymbol Bool = new(BoolName, isArray: false, isPredefined: true);
    public static readonly TypeSymbol Int = new(IntName, isArray: false, isPredefined: true);
    public static readonly TypeSymbol Float = new(FloatName, isArray: false, isPredefined: true);
    public static readonly TypeSymbol String = new(StringName, isArray: false, isPredefined: true);
    public static readonly TypeSymbol BoolArray = new(BoolName, isArray: true, isPredefined: true);
    public static readonly TypeSymbol IntArray = new(IntName, isArray: true, isPredefined: true);
    public static readonly TypeSymbol FloatArray = new(FloatName, isArray: true, isPredefined: true);
    public static readonly TypeSymbol StringArray = new(StringName, isArray: true, isPredefined: true);

    public string Name { get; }
    public bool IsArray { get; }
    public bool IsPredefined { get; }

    private TypeSymbol(string name, bool isArray, bool isPredefined)
    {
        Name = name;
        IsArray = isArray;
        IsPredefined = isPredefined;
    }

    // Script types are created constantly (declarations, casts, receivers); interning
    // them makes construction cheap and equality a reference check in the common case.
    // The key is case-sensitive so each type keeps the exact casing it was written
    // with; equality itself remains case-insensitive.
    private static readonly ConcurrentDictionary<(string Name, bool IsArray), TypeSymbol> ScriptTypeCache =
        new(new ScriptTypeKeyComparer());

    /// <summary>The type representing an instance of a script (used for <c>self</c> and typed receivers).</summary>
    internal static TypeSymbol ScriptType(string name) => Intern(name, isArray: false);

    private static TypeSymbol Intern(string name, bool isArray) =>
        ScriptTypeCache.GetOrAdd((name, isArray), static key => new TypeSymbol(key.Name, key.IsArray, isPredefined: false));

    private sealed class ScriptTypeKeyComparer : IEqualityComparer<(string Name, bool IsArray)>
    {
        public bool Equals((string Name, bool IsArray) x, (string Name, bool IsArray) y) =>
            x.IsArray == y.IsArray && string.Equals(x.Name, y.Name, StringComparison.Ordinal);

        public int GetHashCode((string Name, bool IsArray) key) =>
            HashCode.Combine(key.Name.GetHashCode(StringComparison.Ordinal), key.IsArray);
    }

    public static TypeSymbol FromSyntax(TypeSyntax type) => type switch
    {
        PredefinedTypeSyntax predefined => predefined.Token.Kind switch
        {
            SyntaxKind.BoolKeyword => Bool,
            SyntaxKind.IntKeyword => Int,
            SyntaxKind.FloatKeyword => Float,
            SyntaxKind.StringKeyword => String,
            _ => throw new InvalidOperationException($"Unexpected predefined type token: {predefined.Token.Kind}"),
        },
        NamedTypeSyntax named => Intern(named.Identifier.Text, isArray: false),
        ArrayTypeSyntax array => MakeArray(FromSyntax(array.ElementType)),
        _ => throw new InvalidOperationException($"Unexpected type syntax: {type.Kind}"),
    };

    public static TypeSymbol MakeArray(TypeSymbol elementType)
    {
        if (!elementType.IsPredefined) return Intern(elementType.Name, isArray: true);

        return elementType.Name.ToLowerInvariant() switch
        {
            BoolName => BoolArray,
            IntName => IntArray,
            FloatName => FloatArray,
            StringName => StringArray,
            _ => throw new InvalidOperationException($"Unexpected predefined type: {elementType.Name}"),
        };
    }

    /// <summary>The element type of an array type; the type itself otherwise.</summary>
    public TypeSymbol ElementType
    {
        get
        {
            if (!IsArray) return this;
            if (!IsPredefined) return Intern(Name, isArray: false);
            return Name.ToLowerInvariant() switch
            {
                BoolName => Bool,
                IntName => Int,
                FloatName => Float,
                StringName => String,
                _ => throw new InvalidOperationException($"Unexpected predefined type: {Name}"),
            };
        }
    }

    public bool Equals(TypeSymbol? other) =>
        ReferenceEquals(this, other)
     || (other != null
         && IsArray == other.IsArray
         && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase));

    public override bool Equals(object? obj) => Equals(obj as TypeSymbol);

    public override int GetHashCode() =>
        HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(Name), IsArray);

    public override string ToString() => IsArray ? $"{Name}[]" : Name;
}
