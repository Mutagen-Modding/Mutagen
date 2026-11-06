using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class ParameterSymbol : IParameterSymbol
{
	internal ParameterSymbol(
		string name,
		TypeSymbol type,
		ExpressionSyntax? @default,
		DeclaringSyntaxReference declaringSyntaxReference)
	{
		Name = name;
		Type = type;
		Default = @default;
		DeclaringSyntaxReference = declaringSyntaxReference;
	}

	public string Name { get; }
	public SymbolKind Kind => SymbolKind.Parameter;
	public ISymbol? ContainingSymbol => ContainingFunction;
	public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
	public TypeSymbol Type { get; }
	public ExpressionSyntax? Default { get; }
	public FunctionSymbol? ContainingFunction { get; internal set; }

	// Parameters are variable-like: they can be read anywhere a variable can.
	public bool IsConditional => false;
	public ExpressionSyntax? Initializer => Default;

	public override string ToString() => Name;
}
