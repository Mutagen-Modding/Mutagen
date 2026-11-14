namespace Mutagen.Bethesda.Papyrus.Semantic.Nodes;

/// <summary>
/// Structural traversal of the semantic code tree, mirroring PapyrusSyntaxNode.DescendantNodes.
/// </summary>
public static class PapyrusNodeExtensions
{
    extension(PapyrusNode node) {
        public IEnumerable<PapyrusNode> DescendantNodes()
        {
            foreach (var child in GetChildren(node))
            {
                yield return child;
                foreach (var descendant in child.DescendantNodes())
                {
                    yield return descendant;
                }
            }
        }
        
        public IEnumerable<PapyrusNode> GetChildren() => node switch
        {
            FunctionBody n => n.Statements,
            LocalDeclarationStatement n => n.Initializer != null ? [n.Initializer] : [],
            ExpressionStatement n => [n.Expression],
            ReturnStatement n => n.Expression != null ? [n.Expression] : [],
            IfStatement n => Single(n.Condition)
                .Concat(n.ThenStatements)
                .Concat(n.ElseIfClauses)
                .Concat(n.ElseStatements),
            ElseIfClause n => Single(n.Condition).Concat(n.Statements),
            WhileStatement n => Single(n.Condition).Concat(n.Statements),
            MemberExpression n => [n.Receiver],
            ArrayLengthExpression n => [n.Array],
            ElementAccessExpression n => [n.Array, n.Index],
            CallExpression n => Single(n.Receiver).Concat(n.Arguments),
            CastExpression n => [n.Operand],
            ArrayCreationExpression n => n.Size != null ? [n.Size] : [],
            BinaryExpression n => [n.Left, n.Right],
            UnaryExpression n => [n.Operand],
            AssignmentExpression n => [n.Target, n.Value],
            _ => [],
        };
    }

    private static IEnumerable<PapyrusNode> Single(PapyrusNode? node) =>
        node != null ? [node] : [];
}
