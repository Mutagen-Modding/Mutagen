namespace Mutagen.Bethesda.Papyrus.Syntax;

/// <summary>
/// The Papyrus reserved word table. Keywords are matched case-insensitively;
/// the original source text is preserved on the token.
/// </summary>
public static class PapyrusKeywords
{
    public static IReadOnlyDictionary<string, SyntaxKind> All { get; } = new Dictionary<string, SyntaxKind>
    {
        ["as"] = SyntaxKind.AsKeyword,
        ["auto"] = SyntaxKind.AutoKeyword,
        ["autoreadonly"] = SyntaxKind.AutoReadOnlyKeyword,
        ["bool"] = SyntaxKind.BoolKeyword,
        ["conditional"] = SyntaxKind.ConditionalKeyword,
        ["else"] = SyntaxKind.ElseKeyword,
        ["elseif"] = SyntaxKind.ElseIfKeyword,
        ["endevent"] = SyntaxKind.EndEventKeyword,
        ["endfunction"] = SyntaxKind.EndFunctionKeyword,
        ["endif"] = SyntaxKind.EndIfKeyword,
        ["endproperty"] = SyntaxKind.EndPropertyKeyword,
        ["endstate"] = SyntaxKind.EndStateKeyword,
        ["endwhile"] = SyntaxKind.EndWhileKeyword,
        ["event"] = SyntaxKind.EventKeyword,
        ["extends"] = SyntaxKind.ExtendsKeyword,
        ["false"] = SyntaxKind.FalseKeyword,
        ["float"] = SyntaxKind.FloatKeyword,
        ["function"] = SyntaxKind.FunctionKeyword,
        ["global"] = SyntaxKind.GlobalKeyword,
        ["hidden"] = SyntaxKind.HiddenKeyword,
        ["if"] = SyntaxKind.IfKeyword,
        ["import"] = SyntaxKind.ImportKeyword,
        ["int"] = SyntaxKind.IntKeyword,
        ["length"] = SyntaxKind.LengthKeyword,
        ["native"] = SyntaxKind.NativeKeyword,
        ["new"] = SyntaxKind.NewKeyword,
        ["none"] = SyntaxKind.NoneKeyword,
        ["parent"] = SyntaxKind.ParentKeyword,
        ["property"] = SyntaxKind.PropertyKeyword,
        ["return"] = SyntaxKind.ReturnKeyword,
        ["scriptname"] = SyntaxKind.ScriptNameKeyword,
        ["self"] = SyntaxKind.SelfKeyword,
        ["state"] = SyntaxKind.StateKeyword,
        ["string"] = SyntaxKind.StringKeyword,
        ["true"] = SyntaxKind.TrueKeyword,
        ["while"] = SyntaxKind.WhileKeyword,
    };

    public static bool TryGet(ReadOnlySpan<char> text, out SyntaxKind kind)
    {
        kind = default;
        if (text.Length == 0) return false;
        switch ((char)(text[0] | 0x20))
        {
            case 'a':
                if (text.Length == 2 && (text[1] | 0x20) == 's') { kind = SyntaxKind.AsKeyword; return true; }
                if (text.Equals("auto", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.AutoKeyword; return true; }
                if (text.Equals("autoreadonly", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.AutoReadOnlyKeyword; return true; }
                return false;
            case 'b':
                if (text.Equals("bool", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.BoolKeyword; return true; }
                return false;
            case 'c':
                if (text.Equals("conditional", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ConditionalKeyword; return true; }
                return false;
            case 'e':
                if (text.Equals("else", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ElseKeyword; return true; }
                if (text.Equals("elseif", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ElseIfKeyword; return true; }
                if (text.Equals("endevent", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EndEventKeyword; return true; }
                if (text.Equals("endfunction", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EndFunctionKeyword; return true; }
                if (text.Equals("endif", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EndIfKeyword; return true; }
                if (text.Equals("endproperty", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EndPropertyKeyword; return true; }
                if (text.Equals("endstate", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EndStateKeyword; return true; }
                if (text.Equals("endwhile", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EndWhileKeyword; return true; }
                if (text.Equals("event", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.EventKeyword; return true; }
                if (text.Equals("extends", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ExtendsKeyword; return true; }
                return false;
            case 'f':
                if (text.Equals("false", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.FalseKeyword; return true; }
                if (text.Equals("float", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.FloatKeyword; return true; }
                if (text.Equals("function", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.FunctionKeyword; return true; }
                return false;
            case 'g':
                if (text.Equals("global", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.GlobalKeyword; return true; }
                return false;
            case 'h':
                if (text.Equals("hidden", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.HiddenKeyword; return true; }
                return false;
            case 'i':
                if (text.Equals("if", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.IfKeyword; return true; }
                if (text.Equals("import", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ImportKeyword; return true; }
                if (text.Equals("int", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.IntKeyword; return true; }
                return false;
            case 'l':
                if (text.Equals("length", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.LengthKeyword; return true; }
                return false;
            case 'n':
                if (text.Equals("native", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.NativeKeyword; return true; }
                if (text.Equals("new", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.NewKeyword; return true; }
                if (text.Equals("none", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.NoneKeyword; return true; }
                return false;
            case 'p':
                if (text.Equals("parent", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ParentKeyword; return true; }
                if (text.Equals("property", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.PropertyKeyword; return true; }
                return false;
            case 'r':
                if (text.Equals("return", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ReturnKeyword; return true; }
                return false;
            case 's':
                if (text.Equals("scriptname", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.ScriptNameKeyword; return true; }
                if (text.Equals("self", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.SelfKeyword; return true; }
                if (text.Equals("state", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.StateKeyword; return true; }
                if (text.Equals("string", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.StringKeyword; return true; }
                return false;
            case 't':
                if (text.Equals("true", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.TrueKeyword; return true; }
                return false;
            case 'w':
                if (text.Equals("while", StringComparison.OrdinalIgnoreCase)) { kind = SyntaxKind.WhileKeyword; return true; }
                return false;
            default:
                return false;
        }
    }
}
