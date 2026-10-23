using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Lexer;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

public sealed class LexerTests
{
    private static LexedSource Lex(string source) => Lexer.LexAll(source);

    [Fact]
    public void EmptySource_ProducesOnlyEof()
    {
        var lexed = Lex("");
        lexed.Tokens.Length.ShouldBe(1);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.EndOfFileToken);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void WhitespaceOnlySource_ProducesEofWithLeadingTrivia()
    {
        var lexed = Lex("  \t ");
        lexed.Tokens.Length.ShouldBe(1);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.EndOfFileToken);
        lexed.Tokens[0].LeadingTrivia.Length.ShouldBe(1);
        lexed.Tokens[0].LeadingTrivia[0].Kind.ShouldBe(SyntaxKind.WhitespaceTrivia);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void WhitespaceAndNewlinesBecomeLeadingTrivia()
    {
        var lexed = Lex("  \r\n  a");
        var a = lexed.Tokens[0];
        a.Kind.ShouldBe(SyntaxKind.IdentifierToken);
        a.LeadingTrivia.Length.ShouldBe(3);
        a.LeadingTrivia[0].Kind.ShouldBe(SyntaxKind.WhitespaceTrivia);
        a.LeadingTrivia[1].Kind.ShouldBe(SyntaxKind.EndOfLineTrivia);
        a.LeadingTrivia[2].Kind.ShouldBe(SyntaxKind.WhitespaceTrivia);
    }

    [Fact]
    public void LineCommentBecomesTrailingTrivia()
    {
        var lexed = Lex("a ; comment\nb");
        lexed.Tokens.Length.ShouldBe(3); // a, b, EOF
        var a = lexed.Tokens[0];
        a.TrailingTrivia.Length.ShouldBe(3);
        a.TrailingTrivia[0].Kind.ShouldBe(SyntaxKind.WhitespaceTrivia);
        a.TrailingTrivia[1].Kind.ShouldBe(SyntaxKind.LineCommentTrivia);
        a.TrailingTrivia[1].Text.ShouldBe("; comment");
        a.TrailingTrivia[2].Kind.ShouldBe(SyntaxKind.EndOfLineTrivia);
        lexed.Tokens[1].LeadingTrivia.ShouldBeEmpty();
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void BlockCommentCanSpanLines()
    {
        var lexed = Lex("a ;/ multi\nline /; b");
        lexed.Tokens.Length.ShouldBe(3); // a, b, EOF
        var a = lexed.Tokens[0];
        a.TrailingTrivia.Length.ShouldBe(3);
        a.TrailingTrivia[0].Kind.ShouldBe(SyntaxKind.WhitespaceTrivia);
        a.TrailingTrivia[1].Kind.ShouldBe(SyntaxKind.BlockCommentTrivia);
        a.TrailingTrivia[1].Text.ShouldBe(";/ multi\nline /;");
        a.TrailingTrivia[2].Kind.ShouldBe(SyntaxKind.WhitespaceTrivia);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void DocumentationCommentBecomesLeadingTrivia()
    {
        var lexed = Lex("ScriptName Foo\n{ doc }\nint i");
        var intToken = lexed.Tokens[2];
        intToken.Kind.ShouldBe(SyntaxKind.IntKeyword);
        intToken.LeadingTrivia.Length.ShouldBe(2);
        intToken.LeadingTrivia[0].Kind.ShouldBe(SyntaxKind.DocumentationTrivia);
        intToken.LeadingTrivia[0].Text.ShouldBe("{ doc }");
        intToken.LeadingTrivia[1].Kind.ShouldBe(SyntaxKind.EndOfLineTrivia);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void UnterminatedBlockComment_ProducesDiagnostic()
    {
        var lexed = Lex("a ;/ abc");
        lexed.Diagnostics.Length.ShouldBe(1);
        lexed.Diagnostics[0].Id.ShouldBe(DiagnosticId.UnterminatedBlockComment);
        lexed.Tokens[0].TrailingTrivia[1].Kind.ShouldBe(SyntaxKind.BlockCommentTrivia);
    }

    [Fact]
    public void UnterminatedDocumentationComment_ProducesDiagnostic()
    {
        var lexed = Lex("{ abc");
        lexed.Diagnostics.Length.ShouldBe(1);
        lexed.Diagnostics[0].Id.ShouldBe(DiagnosticId.UnterminatedDocumentationComment);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.EndOfFileToken);
        lexed.Tokens[0].LeadingTrivia[0].Kind.ShouldBe(SyntaxKind.DocumentationTrivia);
    }

    [Fact]
    public void Identifiers_PreserveOriginalText()
    {
        var lexed = Lex("akActor TeleportActors _x9 Actor2 int0");
        lexed.Tokens.Length.ShouldBe(6); // 5 identifiers + EOF
        for (var i = 0; i < 5; i++)
        {
            lexed.Tokens[i].Kind.ShouldBe(SyntaxKind.IdentifierToken);
        }
        lexed.Tokens[0].Text.ShouldBe("akActor");
        lexed.Tokens[1].Text.ShouldBe("TeleportActors");
        lexed.Tokens[2].Text.ShouldBe("_x9");
        lexed.Tokens[3].Text.ShouldBe("Actor2");
        lexed.Tokens[4].Text.ShouldBe("int0");
    }

    public static IEnumerable<object[]> KeywordData =>
        PapyrusKeywords.All.Select(kv => new object[] { kv.Key, kv.Value });

    [Theory]
    [MemberData(nameof(KeywordData))]
    public void Keywords_AreRecognizedCaseInsensitively(string text, SyntaxKind expectedKind)
    {
        var lexed = Lex(text);
        lexed.Tokens.Length.ShouldBe(2); // token + EOF
        lexed.Tokens[0].Kind.ShouldBe(expectedKind);
        lexed.Tokens[0].Text.ShouldBe(text);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Keywords_MixedCase_OriginalTextPreserved()
    {
        var lexed = Lex("ScriptName SCRIPTNAME scriptname");
        lexed.Tokens.Length.ShouldBe(4);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.ScriptNameKeyword);
        lexed.Tokens[0].Text.ShouldBe("ScriptName");
        lexed.Tokens[1].Kind.ShouldBe(SyntaxKind.ScriptNameKeyword);
        lexed.Tokens[1].Text.ShouldBe("SCRIPTNAME");
        lexed.Tokens[2].Kind.ShouldBe(SyntaxKind.ScriptNameKeyword);
        lexed.Tokens[2].Text.ShouldBe("scriptname");
    }

    [Fact]
    public void KeywordFastPath_MatchesKeywordTable()
    {
        foreach (var kv in PapyrusKeywords.All)
        {
            PapyrusKeywords.TryGet(kv.Key, out var kind).ShouldBeTrue(kv.Key);
            kind.ShouldBe(kv.Value, kv.Key);

            var mixed = char.ToUpperInvariant(kv.Key[0]) + kv.Key[1..];
            PapyrusKeywords.TryGet(mixed, out var mixedKind).ShouldBeTrue(mixed);
            mixedKind.ShouldBe(kv.Value, mixed);
        }

        PapyrusKeywords.TryGet("notAKeyword", out _).ShouldBeFalse();
        PapyrusKeywords.TryGet("akActor1", out _).ShouldBeFalse();
        PapyrusKeywords.TryGet("some_name", out _).ShouldBeFalse();
        PapyrusKeywords.TryGet("", out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("42")]
    [InlineData("0x1F")]
    [InlineData("0XaB")]
    [InlineData("1234567890")]
    public void IntegerLiterals(string text)
    {
        var lexed = Lex(text);
        lexed.Tokens.Length.ShouldBe(2);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.IntegerLiteralToken);
        lexed.Tokens[0].Text.ShouldBe(text);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("3.14")]
    [InlineData("0.5")]
    [InlineData("123.0")]
    public void FloatLiterals(string text)
    {
        var lexed = Lex(text);
        lexed.Tokens.Length.ShouldBe(2);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.FloatLiteralToken);
        lexed.Tokens[0].Text.ShouldBe(text);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void IntegerFollowedByDotMember_IsNotFloat()
    {
        var lexed = Lex("0.Length");
        lexed.Tokens.Length.ShouldBe(4); // 0, ., Length, EOF
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.IntegerLiteralToken);
        lexed.Tokens[0].Text.ShouldBe("0");
        lexed.Tokens[1].Kind.ShouldBe(SyntaxKind.DotToken);
        lexed.Tokens[2].Kind.ShouldBe(SyntaxKind.LengthKeyword);
    }

    [Fact]
    public void HexLiteralWithoutDigits_ProducesDiagnostic()
    {
        var lexed = Lex("0x");
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.BadToken);
        lexed.Diagnostics.Length.ShouldBe(1);
        lexed.Diagnostics[0].Id.ShouldBe(DiagnosticId.HexLiteralMissingDigits);
    }

    [Fact]
    public void SimpleStringLiteral()
    {
        var lexed = Lex("\"hello\"");
        lexed.Tokens.Length.ShouldBe(2);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.StringLiteralToken);
        lexed.Tokens[0].Text.ShouldBe("\"hello\"");
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void StringLiteral_WithEscapedQuote()
    {
        var lexed = Lex("""
            "say \"hi\""
            """);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.StringLiteralToken);
        lexed.Tokens[0].Text.ShouldBe("\"say \\\"hi\\\"\"");
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void StringLiteral_WithEscapedBackslash()
    {
        var lexed = Lex("\"back\\\\slash\"");
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.StringLiteralToken);
        lexed.Tokens[0].Text.ShouldBe("\"back\\\\slash\"");
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void UnterminatedStringAtEof_ProducesDiagnostic()
    {
        var lexed = Lex("\"abc");
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.StringLiteralToken);
        lexed.Tokens[0].Text.ShouldBe("\"abc");
        lexed.Diagnostics.Length.ShouldBe(1);
        lexed.Diagnostics[0].Id.ShouldBe(DiagnosticId.UnterminatedStringLiteral);
    }

    [Fact]
    public void UnterminatedStringAtNewline_ProducesDiagnosticAndResumes()
    {
        var lexed = Lex("\"abc\nint");
        lexed.Tokens.Length.ShouldBe(3); // string, int, EOF
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.StringLiteralToken);
        lexed.Tokens[0].Text.ShouldBe("\"abc");
        lexed.Tokens[1].Kind.ShouldBe(SyntaxKind.IntKeyword);
        lexed.Diagnostics.Length.ShouldBe(1);
        lexed.Diagnostics[0].Id.ShouldBe(DiagnosticId.UnterminatedStringLiteral);
    }

    [Theory]
    [InlineData("+", SyntaxKind.PlusToken)]
    [InlineData("-", SyntaxKind.MinusToken)]
    [InlineData("*", SyntaxKind.StarToken)]
    [InlineData("/", SyntaxKind.SlashToken)]
    [InlineData("%", SyntaxKind.PercentToken)]
    [InlineData("=", SyntaxKind.EqualsToken)]
    [InlineData("+=", SyntaxKind.PlusEqualsToken)]
    [InlineData("-=", SyntaxKind.MinusEqualsToken)]
    [InlineData("*=", SyntaxKind.StarEqualsToken)]
    [InlineData("/=", SyntaxKind.SlashEqualsToken)]
    [InlineData("%=", SyntaxKind.PercentEqualsToken)]
    [InlineData("==", SyntaxKind.EqualsEqualsToken)]
    [InlineData("!=", SyntaxKind.ExclamationEqualsToken)]
    [InlineData("<", SyntaxKind.LessThanToken)]
    [InlineData(">", SyntaxKind.GreaterThanToken)]
    [InlineData("<=", SyntaxKind.LessThanEqualsToken)]
    [InlineData(">=", SyntaxKind.GreaterThanEqualsToken)]
    [InlineData("&&", SyntaxKind.AmpersandAmpersandToken)]
    [InlineData("||", SyntaxKind.PipePipeToken)]
    [InlineData("!", SyntaxKind.ExclamationToken)]
    public void Operators(string text, SyntaxKind expectedKind)
    {
        var lexed = Lex(text);
        lexed.Tokens.Length.ShouldBe(2);
        lexed.Tokens[0].Kind.ShouldBe(expectedKind);
        lexed.Tokens[0].Text.ShouldBe(text);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("(", SyntaxKind.OpenParenToken)]
    [InlineData(")", SyntaxKind.CloseParenToken)]
    [InlineData("[", SyntaxKind.OpenBracketToken)]
    [InlineData("]", SyntaxKind.CloseBracketToken)]
    [InlineData(",", SyntaxKind.CommaToken)]
    [InlineData(".", SyntaxKind.DotToken)]
    public void Punctuation(string text, SyntaxKind expectedKind)
    {
        var lexed = Lex(text);
        lexed.Tokens.Length.ShouldBe(2);
        lexed.Tokens[0].Kind.ShouldBe(expectedKind);
        lexed.Tokens[0].Text.ShouldBe(text);
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("@")]
    [InlineData("$")]
    [InlineData("#")]
    [InlineData("&")]
    [InlineData("|")]
    [InlineData("^")]
    public void UnknownCharacter_ProducesBadTokenAndDiagnostic(string text)
    {
        var lexed = Lex(text);
        lexed.Tokens.Length.ShouldBe(2);
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.BadToken);
        lexed.Diagnostics.Length.ShouldBe(1);
        lexed.Diagnostics[0].Id.ShouldBe(DiagnosticId.InvalidCharacter);
    }

    [Fact]
    public void TokenSpans_AreCorrect()
    {
        var lexed = Lex("ab cd");
        lexed.Tokens[0].Start.ShouldBe(0);
        lexed.Tokens[0].Width.ShouldBe(2);
        lexed.Tokens[0].End.ShouldBe(2);
        lexed.Tokens[1].Start.ShouldBe(3);
        lexed.Tokens[1].Width.ShouldBe(2);
        lexed.Tokens[2].Kind.ShouldBe(SyntaxKind.EndOfFileToken);
        lexed.Tokens[2].Start.ShouldBe(5);
        lexed.Tokens[2].Width.ShouldBe(0);
    }

    [Fact]
    public void TriviaSpans_AreCorrect()
    {
        var lexed = Lex("a ; hi\nb");
        var comment = lexed.Tokens[0].TrailingTrivia[1];
        comment.Text.ShouldBe("; hi");
        comment.Width.ShouldBe(4);
        var eol = lexed.Tokens[0].TrailingTrivia[2];
        eol.Kind.ShouldBe(SyntaxKind.EndOfLineTrivia);
        eol.Width.ShouldBe(1);
    }

    [Fact]
    public void LineContinuation_BecomesTrivia()
    {
        var lexed = Lex("""
            a, \
             b
            """);
        lexed.Tokens.Length.ShouldBe(4); // a, comma, b, EOF
        lexed.Tokens[1].Kind.ShouldBe(SyntaxKind.CommaToken);
        lexed.Tokens[1].TrailingTrivia.ShouldContain(t => t.Kind == SyntaxKind.LineContinuationTrivia);
        lexed.Tokens[2].Kind.ShouldBe(SyntaxKind.IdentifierToken);
        lexed.Tokens[2].Text.ShouldBe("b");
        lexed.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void ExampleScript_LexesWithoutDiagnostics()
    {
        var lexed = Lex(ExampleScript.Source);
        lexed.Diagnostics.ShouldBeEmpty();
        lexed.Tokens[0].Kind.ShouldBe(SyntaxKind.ScriptNameKeyword);
        lexed.Tokens[^1].Kind.ShouldBe(SyntaxKind.EndOfFileToken);
    }
}
