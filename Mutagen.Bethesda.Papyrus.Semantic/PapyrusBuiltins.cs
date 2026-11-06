using System.Collections.Concurrent;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// Functions provided by the language runtime rather than by script source: the
/// ScriptObject methods every script inherits (<c>GetState</c>, <c>GoToState</c>)
/// and the array functions (<c>Find</c>). They are modeled as native function
/// symbols contained in synthetic hidden scripts so they are treated like any other function.
/// </summary>
public static class PapyrusBuiltins
{
    private static readonly SyntaxTree EmptyTree = SyntaxTree.Parse("");

    private static DeclaringSyntaxReference None { get; } = new(EmptyTree, default);

    /// <summary>
    /// The implicit base of every Papyrus script; provides <see cref="GetState"/>
    /// and <see cref="GoToState"/> to all script instances.
    /// </summary>
    public static ScriptSymbol ScriptObject { get; } =
        new("ScriptObject", null, isHidden: true, isConditional: false, EmptyTree, None);

    private static ScriptSymbol ArrayScript { get; } =
        new("Array", null, isHidden: true, isConditional: false, EmptyTree, None);

    /// <summary>Returns the name of the state the object is currently in.</summary>
    public static FunctionSymbol GetState { get; } = CreateNative("GetState", TypeSymbol.String, []);

    /// <summary>Moves the object into the given state.</summary>
    public static FunctionSymbol GoToState { get; } = CreateNative("GoToState", null,
    [
        new ParameterSymbol("newState", TypeSymbol.String, null, None),
    ]);

    static PapyrusBuiltins()
    {
        ScriptObject.AddMember(GetState);
        ScriptObject.AddMember(GoToState);
    }

    /// <summary>All ScriptObject functions.</summary>
    public static IReadOnlyList<FunctionSymbol> ScriptObjectFunctions => [GetState, GoToState];

    /// <summary>Finds a ScriptObject function by name, case-insensitively.</summary>
    public static FunctionSymbol? FindScriptObjectFunction(string name)
    {
        if (string.Equals(name, "GetState", StringComparison.OrdinalIgnoreCase)) return GetState;
        if (string.Equals(name, "GoToState", StringComparison.OrdinalIgnoreCase)) return GoToState;

        return null;
    }

    /// <summary>Finds an array function by name for the given array type, case-insensitively.</summary>
    public static FunctionSymbol? FindArrayFunction(TypeSymbol arrayType, string name)
    {
        if (!arrayType.IsArray || !string.Equals(name, "Find", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        return ArrayFindCache.GetOrAdd(arrayType.ElementType, CreateArrayFind);
    }

    /// <summary>All array functions of the given array type.</summary>
    public static IReadOnlyList<FunctionSymbol> ArrayFunctions(TypeSymbol arrayType) =>
        arrayType.IsArray ? [FindArrayFunction(arrayType, "Find")!] : [];

    private static readonly ConcurrentDictionary<TypeSymbol, FunctionSymbol> ArrayFindCache = new();

    private static FunctionSymbol CreateArrayFind(TypeSymbol elementType)
    {
        var function = new FunctionSymbol("Find", TypeSymbol.Int,
        [
            new ParameterSymbol("value", elementType, null, None),
        ], isNative: true, isGlobal: false, ArrayScript, None);
        ArrayScript.AddMember(function);
        return function;
    }

    private static FunctionSymbol CreateNative(string name, TypeSymbol? returnType, IReadOnlyList<ParameterSymbol> parameters) =>
        new(name, returnType, parameters, isNative: true, isGlobal: false, ScriptObject, None);
}
