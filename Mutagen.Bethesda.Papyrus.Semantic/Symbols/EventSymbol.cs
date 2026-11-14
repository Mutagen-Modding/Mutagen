namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class EventSymbol : IEventSymbol, IFunctionSymbol
{
	internal EventSymbol(
		string name,
		IReadOnlyList<ParameterSymbol> parameters,
		bool isNative,
		ISymbol containingSymbol,
		DeclaringSyntaxReference declaringSyntaxReference)
	{
		Name = name;
		Parameters = parameters;
		IsNative = isNative;
		ContainingSymbol = containingSymbol;
		DeclaringSyntaxReference = declaringSyntaxReference;
	}

	public string Name { get; }
	public SymbolKind Kind => SymbolKind.Event;
	public ISymbol ContainingSymbol { get; }
	public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
	public IReadOnlyList<IParameterSymbol> Parameters { get; }

	// Events are function-like: they can be called like functions, so they also
	// satisfy IFunctionSymbol. They have no return value and are never global.
	public TypeSymbol? ReturnType => null;
	public bool IsNative { get; }
	public bool IsGlobal => false;

	private readonly List<VariableSymbol> _locals = [];

	/// <summary>
	/// The local variables declared anywhere in the event body, in declaration order.
	/// </summary>
	public IReadOnlyList<IVariableSymbol> Locals => _locals;

	internal void AddLocal(VariableSymbol local) => _locals.Add(local);

	public override string ToString() => Name;
}
