using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class VariableSymbol : IVariableSymbol
{
	internal VariableSymbol(
		string name,
		TypeSymbol type,
		bool isConditional,
		ExpressionSyntax? initializer,
		ISymbol containingSymbol,
		DeclaringSyntaxReference declaringSyntaxReference)
	{
		Name = name;
		Type = type;
		IsConditional = isConditional;
		Initializer = initializer;
		ContainingSymbol = containingSymbol;
		DeclaringSyntaxReference = declaringSyntaxReference;
	}

	public string Name { get; }
	public SymbolKind Kind => SymbolKind.Variable;
	public ISymbol ContainingSymbol { get; }
	public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
	public TypeSymbol Type { get; }
	public bool IsConditional { get; }
	public ExpressionSyntax? Initializer { get; }

	public override string ToString() => Name;
}
