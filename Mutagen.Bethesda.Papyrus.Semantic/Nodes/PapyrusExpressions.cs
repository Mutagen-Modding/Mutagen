using System.Collections.Immutable;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Nodes;

public sealed class LiteralExpression(LiteralExpressionSyntax syntax, TypeSymbol? type, ConstantValue constantValue) : PapyrusExpression(syntax, type)
{
    public ConstantValue ConstantValue { get; } = constantValue;
}

/// <summary>The <c>self</c> keyword; its type is the containing script.</summary>
public sealed class SelfExpression(LiteralExpressionSyntax syntax, IScriptSymbol script, TypeSymbol type) : PapyrusExpression(syntax, type)
{
    public IScriptSymbol Script { get; } = script;
}

/// <summary>The <c>none</c> literal; it has no type.</summary>
public sealed class NoneExpression(LiteralExpressionSyntax syntax) : PapyrusExpression(syntax, null);

/// <summary>A reference to a local, parameter, script variable, or script property.</summary>
public sealed class VariableExpression(IdentifierNameSyntax syntax, IVariableSymbol variable, TypeSymbol? type) : PapyrusExpression(syntax, type)
{
    public IVariableSymbol Variable { get; } = variable;
}

/// <summary>
/// A member access. <see cref="Member"/> is null when the member could not be
/// resolved; <see cref="Name"/> is the member name as written, so unresolved
/// accesses stay self-describing.
/// </summary>
public sealed class MemberExpression(MemberAccessExpressionSyntax syntax, PapyrusExpression receiver, ISymbol? member, string name, TypeSymbol? type)
    : PapyrusExpression(syntax, type)
{
    public PapyrusExpression Receiver { get; } = receiver;
    public ISymbol? Member { get; } = member;
    public string Name { get; } = name;
}

/// <summary>The <c>length</c> property of an array.</summary>
public sealed class ArrayLengthExpression(MemberAccessExpressionSyntax syntax, PapyrusExpression array) : PapyrusExpression(syntax, TypeSymbol.Int)
{
    public PapyrusExpression Array { get; } = array;
}

/// <summary>An element access into an array.</summary>
public sealed class ElementAccessExpression(ElementAccessExpressionSyntax syntax, PapyrusExpression array, PapyrusExpression index, TypeSymbol? type)
    : PapyrusExpression(syntax, type)
{
    public PapyrusExpression Array { get; } = array;
    public PapyrusExpression Index { get; } = index;
}

/// <summary>
/// A function call. <see cref="Function"/> is null when the call could not be
/// resolved; <see cref="Name"/> is the invoked name as written, so unresolved
/// calls stay self-describing.
/// </summary>
public sealed class CallExpression(InvocationExpressionSyntax syntax, PapyrusExpression? receiver, IFunctionSymbol? function, string? name, ImmutableArray<PapyrusExpression> arguments, TypeSymbol? type)
    : PapyrusExpression(syntax, type)
{
    public PapyrusExpression? Receiver { get; } = receiver;
    public IFunctionSymbol? Function { get; } = function;
    public string? Name { get; } = name;
    public ImmutableArray<PapyrusExpression> Arguments { get; } = arguments;
}

/// <summary>
/// A reference to a script itself: a script name used as a call receiver
/// (<c>Utility.Wait()</c>) or the <c>parent</c> keyword. Its type is the script's type.
/// </summary>
public sealed class ScriptExpression(PapyrusSyntaxNode syntax, IScriptSymbol script, TypeSymbol type, bool isParent = false) : PapyrusExpression(syntax, type)
{
    public IScriptSymbol Script { get; } = script;

    /// <summary>True when this reference is the <c>parent</c> keyword rather than a script name.</summary>
    public bool IsParent { get; } = isParent;
}

public sealed class CastExpression(CastExpressionSyntax syntax, TypeSymbol type, PapyrusExpression operand) : PapyrusExpression(syntax, type)
{
    public PapyrusExpression Operand { get; } = operand;
}

public sealed class ArrayCreationExpression(ArrayCreationExpressionSyntax syntax, TypeSymbol type, PapyrusExpression? size) : PapyrusExpression(syntax, type)
{
    public PapyrusExpression? Size { get; } = size;
}

public sealed class BinaryExpression(BinaryExpressionSyntax syntax, SyntaxKind operatorKind, PapyrusExpression left, PapyrusExpression right, TypeSymbol? type)
    : PapyrusExpression(syntax, type)
{
    public SyntaxKind OperatorKind { get; } = operatorKind;
    public PapyrusExpression Left { get; } = left;
    public PapyrusExpression Right { get; } = right;
}

public sealed class UnaryExpression(PrefixUnaryExpressionSyntax syntax, SyntaxKind operatorKind, PapyrusExpression operand, TypeSymbol? type)
    : PapyrusExpression(syntax, type)
{
    public SyntaxKind OperatorKind { get; } = operatorKind;
    public PapyrusExpression Operand { get; } = operand;
}

public sealed class AssignmentExpression(AssignmentExpressionSyntax syntax, PapyrusExpression target, PapyrusExpression value, TypeSymbol? type)
    : PapyrusExpression(syntax, type)
{
    public PapyrusExpression Target { get; } = target;
    public PapyrusExpression Value { get; } = value;
}

/// <summary>A placeholder for expressions that could not be bound meaningfully.</summary>
public sealed class BadExpression(PapyrusSyntaxNode? syntax) : PapyrusExpression(syntax, null);
