using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// The semantic model of a single parsed script: it owns the script's symbols and
/// answers questions about the code inside function and event bodies. Bodies are
/// bound lazily and cached; every bound expression is indexed by its syntax node so
/// callers can ask what a given expression refers to and what type it has.
/// </summary>
public sealed class PapyrusSemanticModel
{
    private readonly IReadOnlyList<PapyrusDiagnostic> _binderDiagnostics;
    // Span-keyed declaration/symbol indexes are built lazily: a loaded workspace
    // binds bodies only for scripts that are actually queried, so unqueried models
    // never pay for their indexes.
    private Dictionary<TextSpan, FunctionDeclarationSyntax>? _functionDeclarations;
    private Dictionary<TextSpan, EventDeclarationSyntax>? _eventDeclarations;
    private Dictionary<TextSpan, FunctionSymbol>? _functionSymbols;
    private Dictionary<TextSpan, EventSymbol>? _eventSymbols;
    private readonly Dictionary<ISymbol, FunctionBody?> _bodyCache = [];
    private IReadOnlyList<PapyrusDiagnostic>? _allDiagnostics;
    private bool _allBodiesBound;

    public static PapyrusSemanticModel CreateFromSource(string source)
    {
        var tree = SyntaxTree.Parse(source);
        return new PapyrusSemanticModel(tree);
    }

    public PapyrusSemanticModel(SyntaxTree tree)
    {
        SyntaxTree = tree;
        var binder = new ScriptBinder(tree);
        Script = binder.Bind();
        _binderDiagnostics = binder.Diagnostics;
        ScriptType = TypeSymbol.ScriptType(Script.Name);
    }

    /// <summary>
    /// Constructor for codebase members: indexes are built up front, in the
    /// codebase's parallel construction phase, where that is faster than
    /// interleaving them with body binding.
    /// </summary>
    internal PapyrusSemanticModel(SyntaxTree tree, bool buildIndexes)
        : this(tree)
    {
        if (buildIndexes)
        {
            EnsureIndexed();
        }
    }

    public ScriptSymbol Script { get; }

    /// <summary>The type representing an instance of this script (the type of <c>self</c>).</summary>
    internal TypeSymbol ScriptType { get; }

    /// <summary>The codebase this model belongs to, if it was built as part of one.</summary>
    internal PapyrusCodebase? Codebase { get; set; }

    /// <summary>Looks up a script by name in this model's codebase, including this script itself.</summary>
    internal IScriptSymbol? GetScriptByName(string name)
    {
        if (string.Equals(name, Script.Name, StringComparison.OrdinalIgnoreCase)) return Script;

        return Codebase?.GetModel(name)?.Script;
    }

    internal FunctionSymbol? FindFunctionSymbol(FunctionDeclarationSyntax syntax)
    {
        EnsureIndexed();
        return _functionSymbols!.GetValueOrDefault(syntax.Identifier.Span);
    }

    internal EventSymbol? FindEventSymbol(EventDeclarationSyntax syntax)
    {
        EnsureIndexed();
        return _eventSymbols!.GetValueOrDefault(syntax.Identifier.Span);
    }

    /// <summary>
    /// Bound expressions in binding order, indexed by the <c>SemanticData</c> handle
    /// stored on their syntax nodes.
    /// </summary>
    private readonly List<PapyrusExpression> _boundExpressions = [];

    internal int AddBoundExpression(PapyrusExpression bound)
    {
        _boundExpressions.Add(bound);
        return _boundExpressions.Count - 1;
    }

    internal List<PapyrusDiagnostic> BodyDiagnostics { get; } = [];

    internal SyntaxTree SyntaxTree { get; }

    /// <summary>
    /// Binds and returns the bodies of every function, event, and property accessor
    /// in the script. Bodies without statements (native functions etc.) are skipped.
    /// </summary>
    public IEnumerable<FunctionBody> GetBodies()
    {
        EnsureAllBodies();
        return _bodyCache.Values.OfType<FunctionBody>();
    }

    /// <summary>Returns the bound body of a function, or null if it has none (e.g. native functions).</summary>
    public FunctionBody? GetBody(FunctionSymbol function)
    {
        if (_bodyCache.TryGetValue(function, out var cached)) return cached;

        var body = BindBody(function);
        _bodyCache[function] = body;
        return body;
    }

    /// <summary>Returns the bound body of an event, or null if it has none.</summary>
    public FunctionBody? GetBody(EventSymbol @event)
    {
        if (_bodyCache.TryGetValue(@event, out var cached)) return cached;

        var body = BindBody(@event);
        _bodyCache[@event] = body;
        return body;
    }

    /// <summary>
    /// Returns the body of the function or event with the given name, case-insensitively.
    /// Script-level members are found first; if none matches, state members are
    /// searched. Returns null when there is no such member or it has no body.
    /// </summary>
    public FunctionBody? GetBody(string name)
    {
        foreach (var function in Script.Functions)
        {
            if (string.Equals(function.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return GetBody(function);
            }
        }
        foreach (var @event in Script.Events)
        {
            if (string.Equals(@event.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return GetBody(@event);
            }
        }
        foreach (var state in Script.States)
        {
            foreach (var member in state.Members)
            {
                switch (member)
                {
                    case FunctionSymbol function when string.Equals(function.Name, name, StringComparison.OrdinalIgnoreCase):
                        return GetBody(function);
                    case EventSymbol @event when string.Equals(@event.Name, name, StringComparison.OrdinalIgnoreCase):
                        return GetBody(@event);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the bound expression for a syntax expression node, binding the
    /// containing body first if necessary.
    /// </summary>
    public PapyrusExpression? GetExpression(ExpressionSyntax expression)
    {
        EnsureContainingBodyBound(expression);
        var index = expression.SemanticData;
        return index >= 0 ? _boundExpressions[index] : null;
    }

    /// <summary>
    /// Returns the symbol an expression refers to: the variable/parameter/property of
    /// a name reference, the function of a call, or the script of <c>self</c>.
    /// </summary>
    public ISymbol? GetSymbolInfo(ExpressionSyntax expression)
    {
        EnsureContainingBodyBound(expression);
        var index = expression.SemanticData;
        if (index < 0) return null;

        var bound = _boundExpressions[index];
        return bound switch
        {
            VariableExpression variable => variable.Variable,
            MemberExpression member => member.Member,
            CallExpression call => call.Function,
            SelfExpression self => self.Script,
            ScriptExpression script => script.Script,
            _ => null,
        };
    }

    /// <summary>Returns the type of an expression's value, or null when it has no known type.</summary>
    public TypeSymbol? GetTypeInfo(ExpressionSyntax expression)
    {
        EnsureContainingBodyBound(expression);
        var index = expression.SemanticData;
        return index >= 0 ? _boundExpressions[index].Type : null;
    }

    /// <summary>
    /// All diagnostics of the script: parse diagnostics from the syntax tree,
    /// declaration diagnostics from the binder, and body diagnostics from binding
    /// every function and event body.
    /// </summary>
    public IReadOnlyList<PapyrusDiagnostic> GetDiagnostics()
    {
        EnsureAllBodies();
        _allDiagnostics ??= SyntaxTree.Diagnostics.Length == 0
            && _binderDiagnostics.Count == 0
            && BodyDiagnostics.Count == 0
            ? Array.Empty<PapyrusDiagnostic>()
            : [.. SyntaxTree.Diagnostics, .. _binderDiagnostics, .. BodyDiagnostics];
        return _allDiagnostics;
    }

    private FunctionBody? BindBody(ISymbol owner)
    {
        EnsureIndexed();
        switch (owner)
        {
            case FunctionSymbol function:
            {
                if (!_functionDeclarations!.TryGetValue(function.DeclaringSyntaxReference.Span, out var functionDeclaration))
                {
                    return null;
                }
                if (functionDeclaration.Body == null)
                {
                    return null;
                }
                return new BodyBinder(this).BindFunctionBody(function, functionDeclaration.Body);
            }
            case EventSymbol @event:
            {
                if (!_eventDeclarations!.TryGetValue(@event.DeclaringSyntaxReference.Span, out var eventDeclaration))
                {
                    return null;
                }
                if (eventDeclaration.Body == null)
                {
                    return null;
                }
                return new BodyBinder(this).BindEventBody(@event, eventDeclaration.Body);
            }
            default:
                return null;
        }
    }

    private void EnsureContainingBodyBound(PapyrusSyntaxNode node)
    {
        for (var current = node; current != null; current = current.Parent)
        {
            switch (current)
            {
                case FunctionDeclarationSyntax functionDeclaration:
                    EnsureIndexed();
                    if (_functionSymbols!.TryGetValue(functionDeclaration.Identifier.Span, out var functionSymbol))
                    {
                        GetBody(functionSymbol);
                    }
                    return;
                case EventDeclarationSyntax eventDeclaration:
                    EnsureIndexed();
                    if (_eventSymbols!.TryGetValue(eventDeclaration.Identifier.Span, out var eventSymbol))
                    {
                        GetBody(eventSymbol);
                    }
                    return;
            }
        }
    }

    /// <summary>
    /// Builds the span-keyed declaration and symbol indexes on first use.
    /// </summary>
    private void EnsureIndexed()
    {
        if (_functionDeclarations != null) return;

        var functionDeclarations = new Dictionary<TextSpan, FunctionDeclarationSyntax>();
        var eventDeclarations = new Dictionary<TextSpan, EventDeclarationSyntax>();
        foreach (var node in SyntaxTree.Root.DescendantNodes())
        {
            switch (node)
            {
                case FunctionDeclarationSyntax function:
                    functionDeclarations[function.Identifier.Span] = function;
                    break;
                case EventDeclarationSyntax @event:
                    eventDeclarations[@event.Identifier.Span] = @event;
                    break;
            }
        }

        var functionSymbols = new Dictionary<TextSpan, FunctionSymbol>();
        var eventSymbols = new Dictionary<TextSpan, EventSymbol>();
        foreach (var member in Script.Members)
        {
            IndexSymbol(member, functionSymbols, eventSymbols);
        }
        foreach (var state in Script.States)
        {
            foreach (var member in state.Members)
            {
                IndexSymbol(member, functionSymbols, eventSymbols);
            }
        }

        // Assign last so a partially built state is never observable.
        _eventSymbols = eventSymbols;
        _functionSymbols = functionSymbols;
        _eventDeclarations = eventDeclarations;
        _functionDeclarations = functionDeclarations;
    }

    private void EnsureAllBodies()
    {
        if (_allBodiesBound) return;

        _allBodiesBound = true;
        foreach (var member in Script.Members)
        {
            BindMemberBodies(member);
        }
        foreach (var state in Script.States)
        {
            foreach (var member in state.Members)
            {
                BindMemberBodies(member);
            }
        }
    }

    private void BindMemberBodies(ISymbol symbol)
    {
        switch (symbol)
        {
            case FunctionSymbol function:
                GetBody(function);
                break;
            case EventSymbol @event:
                GetBody(@event);
                break;
            case PropertySymbol property:
                if (property.Getter is FunctionSymbol getter)
                {
                    GetBody(getter);
                }
                if (property.Setter is FunctionSymbol setter)
                {
                    GetBody(setter);
                }
                break;
        }
    }

    private static void IndexSymbol(ISymbol symbol, Dictionary<TextSpan, FunctionSymbol> functionSymbols, Dictionary<TextSpan, EventSymbol> eventSymbols)
    {
        switch (symbol)
        {
            case FunctionSymbol function:
                functionSymbols[function.DeclaringSyntaxReference.Span] = function;
                break;
            case EventSymbol @event:
                eventSymbols[@event.DeclaringSyntaxReference.Span] = @event;
                break;
            case PropertySymbol property:
                if (property.Getter is FunctionSymbol getter)
                {
                    functionSymbols[getter.DeclaringSyntaxReference.Span] = getter;
                }
                if (property.Setter is FunctionSymbol setter)
                {
                    functionSymbols[setter.DeclaringSyntaxReference.Span] = setter;
                }
                break;
        }
    }
}
