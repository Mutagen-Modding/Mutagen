using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// Visits the nodes of a semantic code tree in source order. Override a Visit
/// method to react to a node kind; call the base method to continue into its
/// children, or omit it to prune the subtree.
/// </summary>
public abstract class BodyWalker
{
    public void Visit(PapyrusNode? node)
    {
        switch (node)
        {
            case null: return;
            case FunctionBody n: VisitFunctionBody(n); break;
            case LocalDeclarationStatement n: VisitLocalDeclaration(n); break;
            case ExpressionStatement n: VisitExpressionStatement(n); break;
            case ReturnStatement n: VisitReturn(n); break;
            case IfStatement n: VisitIf(n); break;
            case ElseIfClause n: VisitElseIf(n); break;
            case WhileStatement n: VisitWhile(n); break;
            case LiteralExpression n: VisitLiteral(n); break;
            case SelfExpression n: VisitSelf(n); break;
            case NoneExpression n: VisitNone(n); break;
            case VariableExpression n: VisitVariable(n); break;
            case MemberExpression n: VisitMember(n); break;
            case ArrayLengthExpression n: VisitArrayLength(n); break;
            case ElementAccessExpression n: VisitElementAccess(n); break;
            case CallExpression n: VisitCall(n); break;
            case CastExpression n: VisitCast(n); break;
            case ArrayCreationExpression n: VisitArrayCreation(n); break;
            case BinaryExpression n: VisitBinary(n); break;
            case UnaryExpression n: VisitUnary(n); break;
            case AssignmentExpression n: VisitAssignment(n); break;
            case ScriptExpression n: VisitScript(n); break;
            case BadExpression n: VisitBad(n); break;
            default: DefaultVisit(node); break;
        }
    }

    protected virtual void VisitFunctionBody(FunctionBody node) => DefaultVisit(node);
    protected virtual void VisitLocalDeclaration(LocalDeclarationStatement node) => DefaultVisit(node);
    protected virtual void VisitExpressionStatement(ExpressionStatement node) => DefaultVisit(node);
    protected virtual void VisitReturn(ReturnStatement node) => DefaultVisit(node);
    protected virtual void VisitIf(IfStatement node) => DefaultVisit(node);
    protected virtual void VisitElseIf(ElseIfClause node) => DefaultVisit(node);
    protected virtual void VisitWhile(WhileStatement node) => DefaultVisit(node);
    protected virtual void VisitLiteral(LiteralExpression node) => DefaultVisit(node);
    protected virtual void VisitSelf(SelfExpression node) => DefaultVisit(node);
    protected virtual void VisitNone(NoneExpression node) => DefaultVisit(node);
    protected virtual void VisitVariable(VariableExpression node) => DefaultVisit(node);
    protected virtual void VisitMember(MemberExpression node) => DefaultVisit(node);
    protected virtual void VisitArrayLength(ArrayLengthExpression node) => DefaultVisit(node);
    protected virtual void VisitElementAccess(ElementAccessExpression node) => DefaultVisit(node);
    protected virtual void VisitCall(CallExpression node) => DefaultVisit(node);
    protected virtual void VisitCast(CastExpression node) => DefaultVisit(node);
    protected virtual void VisitArrayCreation(ArrayCreationExpression node) => DefaultVisit(node);
    protected virtual void VisitBinary(BinaryExpression node) => DefaultVisit(node);
    protected virtual void VisitUnary(UnaryExpression node) => DefaultVisit(node);
    protected virtual void VisitAssignment(AssignmentExpression node) => DefaultVisit(node);
    protected virtual void VisitScript(ScriptExpression node) => DefaultVisit(node);
    protected virtual void VisitBad(BadExpression node) => DefaultVisit(node);

    protected virtual void DefaultVisit(PapyrusNode node)
    {
        foreach (var child in node.GetChildren())
        {
            Visit(child);
        }
    }
}

/// <summary>
/// A <see cref="BodyWalker"/> driven by delegate actions instead of subclassing:
/// supply an optional action per node kind. Actions that are not supplied fall back
/// to the default behavior (descend into children). Omit the base call by supplying
/// an action that does not recurse to prune a subtree.
/// </summary>
public sealed class FuncBodyWalker(
    Action<FunctionBody>? functionBody = null,
    Action<LocalDeclarationStatement>? localDeclaration = null,
    Action<ExpressionStatement>? expressionStatement = null,
    Action<ReturnStatement>? @return = null,
    Action<IfStatement>? @if = null,
    Action<ElseIfClause>? elseIf = null,
    Action<WhileStatement>? @while = null,
    Action<LiteralExpression>? literal = null,
    Action<SelfExpression>? self = null,
    Action<NoneExpression>? none = null,
    Action<VariableExpression>? variable = null,
    Action<MemberExpression>? member = null,
    Action<ArrayLengthExpression>? arrayLength = null,
    Action<ElementAccessExpression>? elementAccess = null,
    Action<CallExpression>? call = null,
    Action<CastExpression>? cast = null,
    Action<ArrayCreationExpression>? arrayCreation = null,
    Action<BinaryExpression>? binary = null,
    Action<UnaryExpression>? unary = null,
    Action<AssignmentExpression>? assignment = null,
    Action<ScriptExpression>? script = null,
    Action<BadExpression>? bad = null)
    : BodyWalker
{
    protected override void VisitFunctionBody(FunctionBody node)
    {
        if (functionBody is not null) functionBody(node);
        else DefaultVisit(node);
    }
    protected override void VisitLocalDeclaration(LocalDeclarationStatement node)
    {
        if (localDeclaration is not null) localDeclaration(node);
        else DefaultVisit(node);
    }
    protected override void VisitExpressionStatement(ExpressionStatement node)
    {
        if (expressionStatement is not null) expressionStatement(node);
        else DefaultVisit(node);
    }
    protected override void VisitReturn(ReturnStatement node)
    {
        if (@return is not null) @return(node);
        else DefaultVisit(node);
    }
    protected override void VisitIf(IfStatement node)
    {
        if (@if is not null) @if(node);
        else DefaultVisit(node);
    }
    protected override void VisitElseIf(ElseIfClause node)
    {
        if (elseIf is not null) elseIf(node);
        else DefaultVisit(node);
    }
    protected override void VisitWhile(WhileStatement node)
    {
        if (@while is not null) @while(node);
        else DefaultVisit(node);
    }
    protected override void VisitLiteral(LiteralExpression node)
    {
        if (literal is not null) literal(node);
        else DefaultVisit(node);
    }
    protected override void VisitSelf(SelfExpression node)
    {
        if (self is not null) self(node);
        else DefaultVisit(node);
    }
    protected override void VisitNone(NoneExpression node)
    {
        if (none is not null) none(node);
        else DefaultVisit(node);
    }
    protected override void VisitVariable(VariableExpression node)
    {
        if (variable is not null) variable(node);
        else DefaultVisit(node);
    }
    protected override void VisitMember(MemberExpression node)
    {
        if (member is not null) member(node);
        else DefaultVisit(node);
    }
    protected override void VisitArrayLength(ArrayLengthExpression node)
    {
        if (arrayLength is not null) arrayLength(node);
        else DefaultVisit(node);
    }
    protected override void VisitElementAccess(ElementAccessExpression node)
    {
        if (elementAccess is not null) elementAccess(node);
        else DefaultVisit(node);
    }
    protected override void VisitCall(CallExpression node)
    {
        if (call is not null) call(node);
        else DefaultVisit(node);
    }
    protected override void VisitCast(CastExpression node)
    {
        if (cast is not null) cast(node);
        else DefaultVisit(node);
    }
    protected override void VisitArrayCreation(ArrayCreationExpression node)
    {
        if (arrayCreation is not null) arrayCreation(node);
        else DefaultVisit(node);
    }
    protected override void VisitBinary(BinaryExpression node)
    {
        if (binary is not null) binary(node);
        else DefaultVisit(node);
    }
    protected override void VisitUnary(UnaryExpression node)
    {
        if (unary is not null) unary(node);
        else DefaultVisit(node);
    }
    protected override void VisitAssignment(AssignmentExpression node)
    {
        if (assignment is not null) assignment(node);
        else DefaultVisit(node);
    }
    protected override void VisitScript(ScriptExpression node)
    {
        if (script is not null) script(node);
        else DefaultVisit(node);
    }
    protected override void VisitBad(BadExpression node)
    {
        if (bad is not null) bad(node);
        else DefaultVisit(node);
    }
}
