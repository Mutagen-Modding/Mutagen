namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

/// <summary>
/// The parsed representation of a single Papyrus source file: a lossless syntax tree
/// plus all diagnostics produced while parsing it.
/// </summary>
public sealed class SyntaxTree(SourceText source, CompilationUnitSyntax root, PapyrusDiagnostic[] diagnostics)
{
    public SourceText Source => source;
    public CompilationUnitSyntax Root => root;
    public PapyrusDiagnostic[] Diagnostics => diagnostics;

    public static SyntaxTree Parse(string source) => Parse(new SourceText(source));

    public static SyntaxTree Parse(SourceText source)
    {
        var parser = new Parser(source);
        var root = parser.ParseCompilationUnit();
        var diagnostics = parser.Diagnostics.Count == 0 && parser.LexerDiagnostics.Count == 0
            ? []
            : parser.LexerDiagnostics.Concat(parser.Diagnostics)
                .OrderBy(d => d.Span.Start)
                .ToArray();
        var tree = new SyntaxTree(source, root, diagnostics);
        tree.Fixup(root);
        return tree;
    }

    private void Fixup(PapyrusSyntaxNode node)
    {
        if (node is CompilationUnitSyntax rootNode)
        {
            rootNode.AttachTree(this);
        }
        foreach (var child in node.ChildNodesAndTokens())
        {
            child.Parent = node;
            Fixup(child);
        }
    }
}
