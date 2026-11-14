using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Nodes;

/// <summary>
/// Base class of the semantic code tree: the executable code of function and event
/// bodies, where every reference is resolved to a symbol and every expression has a
/// type. Each node keeps a link to the syntax it was bound from.
/// </summary>
public abstract class PapyrusNode(PapyrusSyntaxNode? syntax)
{
    public PapyrusSyntaxNode? Syntax { get; } = syntax;
}

public abstract class PapyrusStatement(PapyrusSyntaxNode? syntax) : PapyrusNode(syntax);

public abstract class PapyrusExpression(PapyrusSyntaxNode? syntax, TypeSymbol? type) : PapyrusNode(syntax)
{
    /// <summary>The type of the expression's value; null when unknown or none.</summary>
    public TypeSymbol? Type { get; } = type;
}
