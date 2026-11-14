using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class PropertySymbol : IPropertySymbol, IVariableSymbol
{
	internal PropertySymbol(
		string name,
		TypeSymbol type,
		bool isAuto,
		bool isAutoReadOnly,
		bool isHidden,
		bool isConditional,
		ExpressionSyntax? initializer,
		FunctionSymbol? getter,
		FunctionSymbol? setter,
		ISymbol containingSymbol,
		DeclaringSyntaxReference declaringSyntaxReference)
	{
		Name = name;
		Type = type;
		IsAuto = isAuto;
		IsAutoReadOnly = isAutoReadOnly;
		IsHidden = isHidden;
		IsConditional = isConditional;
		Initializer = initializer;
		Getter = getter;
		Setter = setter;
		ContainingSymbol = containingSymbol;
		DeclaringSyntaxReference = declaringSyntaxReference;
	}

	public string Name { get; }
	public SymbolKind Kind => SymbolKind.Property;
	public ISymbol ContainingSymbol { get; }
	public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
	public TypeSymbol Type { get; }
	public bool IsAuto { get; }
	public bool IsAutoReadOnly { get; }
	public bool IsHidden { get; }
	public bool IsConditional { get; }
	public ExpressionSyntax? Initializer { get; }
	public IFunctionSymbol? Getter { get; private set; }
	public IFunctionSymbol? Setter { get; private set; }

	internal void SetAccessors(FunctionSymbol? getter, FunctionSymbol? setter)
	{
		Getter = getter;
		Setter = setter;
	}

	public override string ToString() => Name;
}
