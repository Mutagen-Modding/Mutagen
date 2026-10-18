using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface IPropertySymbol : ISymbol
{
	TypeSymbol Type { get; }
	bool IsAuto { get; }
	bool IsAutoReadOnly { get; }
	bool IsHidden { get; }
	bool IsConditional { get; }
	ExpressionSyntax? Initializer { get; }
	IFunctionSymbol? Getter { get; }
	IFunctionSymbol? Setter { get; }
}
