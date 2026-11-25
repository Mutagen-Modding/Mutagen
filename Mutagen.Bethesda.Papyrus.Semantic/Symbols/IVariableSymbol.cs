using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface IVariableSymbol : ISymbol
{
	TypeSymbol Type { get; }
	bool IsConditional { get; }
	ExpressionSyntax? Initializer { get; }
}
