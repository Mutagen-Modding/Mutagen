using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// Points at the syntax that declared a symbol: the owning tree and the span of the
/// declaring name token.
/// </summary>
public readonly struct DeclaringSyntaxReference
{
	public SyntaxTree SyntaxTree { get; }
	public TextSpan Span { get; }

	internal DeclaringSyntaxReference(SyntaxTree syntaxTree, TextSpan span)
	{
		SyntaxTree = syntaxTree;
		Span = span;
	}
}
