namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class FunctionSymbol : IFunctionSymbol
{
	private readonly List<VariableSymbol> _locals = [];

	internal FunctionSymbol(
		string name,
		TypeSymbol? returnType,
		IReadOnlyList<ParameterSymbol> parameters,
		bool isNative,
		bool isGlobal,
		ISymbol containingSymbol,
		DeclaringSyntaxReference declaringSyntaxReference)
	{
		Name = name;
		ReturnType = returnType;
		Parameters = parameters;
		IsNative = isNative;
		IsGlobal = isGlobal;
		ContainingSymbol = containingSymbol;
		DeclaringSyntaxReference = declaringSyntaxReference;
		foreach (var parameter in parameters)
		{
			parameter.ContainingFunction = this;
		}
	}

	public string Name { get; }
	public SymbolKind Kind => SymbolKind.Function;
	public ISymbol ContainingSymbol { get; }
	public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
	public TypeSymbol? ReturnType { get; }
	public IReadOnlyList<IParameterSymbol> Parameters { get; }

	/// <summary>
	/// The local variables declared anywhere in the function body, in declaration
	/// order. The body's statements themselves belong to the bound-node layer, not
	/// to the symbol world.
	/// </summary>
	public IReadOnlyList<IVariableSymbol> Locals => _locals;

	public bool IsNative { get; }
	public bool IsGlobal { get; }

	internal void AddLocal(VariableSymbol local) => _locals.Add(local);

	public override string ToString() => Name;
}
