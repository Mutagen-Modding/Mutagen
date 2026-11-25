using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

/// <summary>
/// A function parameter. Parameters are variable-like: they can be read anywhere a
/// variable can, so they also satisfy <see cref="IVariableSymbol"/>.
/// </summary>
public interface IParameterSymbol : IVariableSymbol
{
	ExpressionSyntax? Default { get; }
}
