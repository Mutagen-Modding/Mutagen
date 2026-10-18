using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class ScriptSymbol : IScriptSymbol
{
    private readonly List<ISymbol> _members = [];
    private readonly List<StateSymbol> _states = [];
    private readonly List<string> _imports = [];

    internal ScriptSymbol(
        string name,
        string? baseScriptName,
        bool isHidden,
        bool isConditional,
        SyntaxTree syntaxTree,
        DeclaringSyntaxReference declaringSyntaxReference)
    {
        Name = name;
        BaseScriptName = baseScriptName;
        IsHidden = isHidden;
        IsConditional = isConditional;
        SyntaxTree = syntaxTree;
        DeclaringSyntaxReference = declaringSyntaxReference;
    }

    public string Name { get; }
    public SymbolKind Kind => SymbolKind.Script;
    public ISymbol? ContainingSymbol => null;
    public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
    public SyntaxTree SyntaxTree { get; }

    /// <summary>The name of the base script as written, before cross-script resolution.</summary>
    public string? BaseScriptName { get; }

    /// <summary>
    /// The base script, resolved by the owning codebase; null when the script has
    /// no base, or the model is used without a codebase.
    /// </summary>
    public IScriptSymbol? BaseScript { get; internal set; }

    public bool IsHidden { get; }
    public bool IsConditional { get; }

    public IReadOnlyList<ISymbol> Members => _members;
    public IReadOnlyList<StateSymbol> States => _states;
    public IReadOnlyList<string> Imports => _imports;

    public IReadOnlyList<FunctionSymbol> Functions => field ??= [.. _members.OfType<FunctionSymbol>()];
    public IReadOnlyList<EventSymbol> Events => field ??= [.. _members.OfType<EventSymbol>()];
    public IReadOnlyList<PropertySymbol> Properties => field ??= [.. _members.OfType<PropertySymbol>()];
    public IReadOnlyList<VariableSymbol> Variables => field ??= [.. _members.OfType<VariableSymbol>()];

    internal void AddMember(ISymbol member) => _members.Add(member);
    internal void AddState(StateSymbol state) => _states.Add(state);
    internal void AddImport(string scriptName) => _imports.Add(scriptName);

    public override string ToString() => Name;
}
