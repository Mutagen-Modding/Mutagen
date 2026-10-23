using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// A single script in a project: its source text, the parsed syntax tree, and the script name from the header.
/// Documents are immutable, so changing text means parsing a new document (see <see cref="PapyrusProject.UpdateDocument"/>).
/// </summary>
public sealed class PapyrusDocument
{
    public string Name { get; }
    public string Text { get; }
    public string? FilePath { get; }
    public SyntaxTree SyntaxTree { get; }

    private PapyrusDocument(string name, string text, string? filePath, SyntaxTree syntaxTree)
    {
        Name = name;
        Text = text;
        FilePath = filePath;
        SyntaxTree = syntaxTree;
    }

    /// <summary>Parses the text and identifies the script by its header name.</summary>
    public static PapyrusDocument Parse(string text, string? filePath = null)
    {
        var tree = SyntaxTree.Parse(text);
        return new PapyrusDocument(tree.Root.Header.Name.Text, text, filePath, tree);
    }
}
