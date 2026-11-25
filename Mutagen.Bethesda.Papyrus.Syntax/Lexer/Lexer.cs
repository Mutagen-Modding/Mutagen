using System.Collections.Concurrent;

namespace Mutagen.Bethesda.Papyrus.Syntax.Lexer;

/// <summary>
/// Turns Papyrus source text into a stream of tokens with attached trivia.
/// Tokens carry exact source positions; identifier text is interned.
/// </summary>
public sealed class Lexer(SourceText source)
{
    private readonly List<LexedTrivia> _triviaScratch = [];
    private int _pos;

    public SourceText Source { get; } = source;
    public List<PapyrusDiagnostic> Diagnostics { get; } = [];

    public static LexedSource LexAll(string source)
    {
        var lexer = new Lexer(new SourceText(source));
        // Rough pre-size from source length avoids most list growth copies.
        var tokens = new List<LexedToken>(Math.Max(64, source.Length / 10));
        LexedToken token;
        do
        {
            token = lexer.LexNext();
            tokens.Add(token);
        } while (token.Kind != SyntaxKind.EndOfFileToken);
        return new LexedSource(lexer.Source, [.. tokens], [.. lexer.Diagnostics]);
    }

    public LexedToken LexNext()
    {
        var leadingStart = _pos;
        var leading = ScanTrivia(trailing: false);
        var start = _pos;
        var (kind, text) = ScanToken();
        var width = _pos - start;
        var trailingStart = _pos;
        var trailing = ScanTrivia(trailing: true);
        // Trivia widths fall out of the scan positions; no extra pass needed.
        return new LexedToken(kind, start, width, text, leading, trailing, start - leadingStart, _pos - trailingStart);
    }

    private LexedTrivia[] ScanTrivia(bool trailing)
    {
        // The scratch list is reused across tokens; ToArray materializes the
        // exact-size array that gets stored on the token.
        _triviaScratch.Clear();
        while (_pos < Source.Length)
        {
            var start = _pos;
            var c = Source.Text[_pos];
            SyntaxKind kind;
            string? text = null;
            var stopAfter = false;
            switch (c)
            {
                case ' ' or '\t':
                    kind = SyntaxKind.WhitespaceTrivia;
                    while (_pos < Source.Length && Source.Text[_pos] is ' ' or '\t') _pos++;
                    break;

                case '\r' or '\n':
                    kind = SyntaxKind.EndOfLineTrivia;
                    _pos++;
                    if (c == '\r' && _pos < Source.Length && Source.Text[_pos] == '\n')
                    {
                        _pos++;
                    }
                    stopAfter = trailing;
                    break;

                case '\\':
                {
                    int look = _pos + 1;
                    while (look < Source.Length && Source.Text[look] is ' ' or '\t') look++;
                    if (look < Source.Length && Source.Text[look] is '\r' or '\n')
                    {
                        kind = SyntaxKind.LineContinuationTrivia;
                        _pos = look;
                        if (Source.Text[_pos] == '\r')
                        {
                            _pos++;
                            if (_pos < Source.Length && Source.Text[_pos] == '\n') _pos++;
                        }
                        else
                        {
                            _pos++;
                        }
                        stopAfter = trailing;
                        break;
                    }
                    return Materialize(_triviaScratch);
                }

                case ';' when _pos + 1 < Source.Length && Source.Text[_pos + 1] == '/':
                    kind = SyntaxKind.BlockCommentTrivia;
                    _pos += 2;
                    var blockEnd = -1;
                    while (_pos < Source.Length)
                    {
                        if (Source.Text[_pos] == '/' && _pos + 1 < Source.Length && Source.Text[_pos + 1] == ';')
                        {
                            blockEnd = _pos;
                            break;
                        }
                        _pos++;
                    }
                    if (blockEnd < 0)
                    {
                        AddDiagnostic(DiagnosticId.UnterminatedBlockComment, start);
                        _pos = Source.Length;
                    }
                    else
                    {
                        _pos = blockEnd + 2;
                    }
                    text = Source.Text[start.._pos];
                    break;

                case ';':
                    kind = SyntaxKind.LineCommentTrivia;
                    _pos++;
                    while (_pos < Source.Length && Source.Text[_pos] is not ('\r' or '\n')) _pos++;
                    text = Source.Text[start.._pos];
                    break;

                case '{':
                    kind = SyntaxKind.DocumentationTrivia;
                    _pos++;
                    while (_pos < Source.Length && Source.Text[_pos] != '}') _pos++;
                    if (_pos >= Source.Length)
                    {
                        AddDiagnostic(DiagnosticId.UnterminatedDocumentationComment, start);
                    }
                    else
                    {
                        _pos++;
                    }
                    text = Source.Text[start.._pos];
                    break;

                default:
                    return Materialize(_triviaScratch);
            }

            _triviaScratch.Add(new LexedTrivia(kind, _pos - start, text));
            if (stopAfter) break;
        }

        return Materialize(_triviaScratch);
    }

    // Whitespace/EOL-only trivia arrays repeat identically across every file (indentation patterns)
    // Interning them removes most trivia allocations.
    private static readonly ConcurrentDictionary<TriviaKey, LexedTrivia[]> InternedTrivia = new();

    // Widths above this are rare - skipping them keeps the intern cache small.
    private const int MaxInternedTriviaWidth = 64;

    /// <summary>
    /// Turns the scratch list into the array stored on the token. Whitespace/EOL-only
    /// arrays of one or two items are interned: those patterns (indentation plus line
    /// break) repeat identically across every file, so sharing one array per pattern
    /// removes most trivia allocations. Anything containing comment text, or longer
    /// than two items, is unique enough to allocate directly.
    /// </summary>
    private static LexedTrivia[] Materialize(List<LexedTrivia> trivia)
    {
        switch (trivia.Count) {
            case 0:
                return [];
            // Internable: no comment text (positions are reconstructible from the
            // token's full span, so content is fully described by kind + width), and
            // widths are capped to keep the intern cache small.
            case <= 2
                when trivia[0].Text == null
             && (trivia.Count == 1 || trivia[1].Text == null)
             && trivia[0].Width <= MaxInternedTriviaWidth
             && (trivia.Count == 1 || trivia[1].Width <= MaxInternedTriviaWidth): {
                var key = trivia.Count == 1
                    ? new TriviaKey(trivia[0].Kind, trivia[0].Width)
                    : new TriviaKey(trivia[0].Kind, trivia[0].Width, trivia[1].Kind, trivia[1].Width);
                return InternedTrivia.GetOrAdd(key, static k =>
                {
                    var first = new LexedTrivia(k.Kind0, k.Width0, null);
                    return k.IsSingle
                        ? [first]
                        : [first, new LexedTrivia(k.Kind1, k.Width1, null)];
                });
            }
            default:
                return trivia.ToArray();
        }
    }

    /// <summary>
    /// Content key of an internable trivia array: the kinds and widths of its one or
    /// two items. <see cref="Kind1"/> stays 0 for single-item arrays, which cannot
    /// collide with two-item keys because no real trivia width is 0.
    /// </summary>
    private readonly struct TriviaKey(SyntaxKind kind0, int width0, SyntaxKind kind1 = 0, int width1 = 0) : IEquatable<TriviaKey>
    {
        public SyntaxKind Kind0 { get; } = kind0;
        public int Width0 { get; } = width0;
        public SyntaxKind Kind1 { get; } = kind1;
        public int Width1 { get; } = width1;
        public bool IsSingle => Width1 == 0;

        public bool Equals(TriviaKey other) =>
            Kind0 == other.Kind0 && Width0 == other.Width0 && Kind1 == other.Kind1 && Width1 == other.Width1;
        public override bool Equals(object? obj) => obj is TriviaKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Kind0, Width0, Kind1, Width1);
    }

    private (SyntaxKind Kind, string Text) ScanToken()
    {
        if (_pos >= Source.Length) return (SyntaxKind.EndOfFileToken, string.Empty);
        var start = _pos;
        switch (Source.Text[_pos])
        {
            case '(':
                _pos++;
                return (SyntaxKind.OpenParenToken, FixedText(SyntaxKind.OpenParenToken));
            case ')':
                _pos++;
                return (SyntaxKind.CloseParenToken, FixedText(SyntaxKind.CloseParenToken));
            case '[':
                _pos++;
                return (SyntaxKind.OpenBracketToken, FixedText(SyntaxKind.OpenBracketToken));
            case ']':
                _pos++;
                return (SyntaxKind.CloseBracketToken, FixedText(SyntaxKind.CloseBracketToken));
            case ',':
                _pos++;
                return (SyntaxKind.CommaToken, FixedText(SyntaxKind.CommaToken));
            case '.':
                _pos++;
                return (SyntaxKind.DotToken, FixedText(SyntaxKind.DotToken));

            case '+':
                return TwoChar('=', SyntaxKind.PlusEqualsToken, SyntaxKind.PlusToken);
            case '-':
                return TwoChar('=', SyntaxKind.MinusEqualsToken, SyntaxKind.MinusToken);
            case '*':
                return TwoChar('=', SyntaxKind.StarEqualsToken, SyntaxKind.StarToken);
            case '/':
                return TwoChar('=', SyntaxKind.SlashEqualsToken, SyntaxKind.SlashToken);
            case '%':
                return TwoChar('=', SyntaxKind.PercentEqualsToken, SyntaxKind.PercentToken);
            case '=':
                return TwoChar('=', SyntaxKind.EqualsEqualsToken, SyntaxKind.EqualsToken);
            case '!':
                return TwoChar('=', SyntaxKind.ExclamationEqualsToken, SyntaxKind.ExclamationToken);
            case '<':
                return TwoChar('=', SyntaxKind.LessThanEqualsToken, SyntaxKind.LessThanToken);
            case '>':
                return TwoChar('=', SyntaxKind.GreaterThanEqualsToken, SyntaxKind.GreaterThanToken);

            case '&':
                if (Peek(1) == '&')
                {
                    _pos += 2;
                    return (SyntaxKind.AmpersandAmpersandToken, FixedText(SyntaxKind.AmpersandAmpersandToken));
                }
                return BadToken(start);
            case '|':
                if (Peek(1) == '|')
                {
                    _pos += 2;
                    return (SyntaxKind.PipePipeToken, FixedText(SyntaxKind.PipePipeToken));
                }
                return BadToken(start);

            case '"':
                return ScanStringLiteral(start);

            default:
                if (IsIdentifierStart(Source.Text[_pos])) return ScanIdentifier(start);
                if (char.IsAsciiDigit(Source.Text[_pos])) return ScanNumber(start);
                return BadToken(start);
        }
    }

    // Punctuation has no casing and a finite set of spellings; static strings remove
    // the per-file interning of the most common tokens entirely.
    private static string FixedText(SyntaxKind kind) => kind switch
    {
        SyntaxKind.OpenParenToken => "(",
        SyntaxKind.CloseParenToken => ")",
        SyntaxKind.OpenBracketToken => "[",
        SyntaxKind.CloseBracketToken => "]",
        SyntaxKind.CommaToken => ",",
        SyntaxKind.DotToken => ".",
        SyntaxKind.PlusToken => "+",
        SyntaxKind.PlusEqualsToken => "+=",
        SyntaxKind.MinusToken => "-",
        SyntaxKind.MinusEqualsToken => "-=",
        SyntaxKind.StarToken => "*",
        SyntaxKind.StarEqualsToken => "*=",
        SyntaxKind.SlashToken => "/",
        SyntaxKind.SlashEqualsToken => "/=",
        SyntaxKind.PercentToken => "%",
        SyntaxKind.PercentEqualsToken => "%=",
        SyntaxKind.EqualsToken => "=",
        SyntaxKind.EqualsEqualsToken => "==",
        SyntaxKind.ExclamationToken => "!",
        SyntaxKind.ExclamationEqualsToken => "!=",
        SyntaxKind.LessThanToken => "<",
        SyntaxKind.LessThanEqualsToken => "<=",
        SyntaxKind.GreaterThanToken => ">",
        SyntaxKind.GreaterThanEqualsToken => ">=",
        SyntaxKind.AmpersandAmpersandToken => "&&",
        SyntaxKind.PipePipeToken => "||",
        _ => throw new InvalidOperationException($"Token kind {kind} has no fixed text."),
    };

    private (SyntaxKind Kind, string Text) TwoChar(char second, SyntaxKind twoCharKind, SyntaxKind singleKind)
    {
        if (Peek(1) == second)
        {
            _pos += 2;
            return (twoCharKind, FixedText(twoCharKind));
        }
        _pos++;
        return (singleKind, FixedText(singleKind));
    }

    private (SyntaxKind Kind, string Text) ScanIdentifier(int start)
    {
        while (_pos < Source.Length && IsIdentifierPart(Source.Text[_pos])) _pos++;
        var span = Source.Text.AsSpan(start, _pos - start);
        if (PapyrusKeywords.TryGet(span, out var kind)) return (kind, InternKeyword(span));
        return (SyntaxKind.IdentifierToken, Intern(span));
    }

    // Keyword spellings (case variants) repeat across every file; they are shared
    // globally. The set is tiny in practice and capped so pathological input cannot
    // grow it without bound. The original source casing is preserved.
    private static readonly ConcurrentDictionary<string, string> KeywordSpellings = new(StringComparer.Ordinal);

    // Tracked with a separate counter: ConcurrentDictionary.Count acquires all
    // internal locks and is far too expensive to call per token.
    private static int _keywordSpellingCount;

    private const int MaxKeywordSpellings = 1024;

    private string InternKeyword(ReadOnlySpan<char> span)
    {
        if (_keywordSpellingCount >= MaxKeywordSpellings)
        {
            return Intern(span);
        }
        var lookup = KeywordSpellings.GetAlternateLookup<ReadOnlySpan<char>>();
        if (lookup.TryGetValue(span, out var existing)) return existing;
        var spelling = span.ToString();
        if (lookup.TryAdd(span, spelling))
        {
            Interlocked.Increment(ref _keywordSpellingCount);
        }
        return spelling;
    }

    private (SyntaxKind Kind, string Text) ScanNumber(int start)
    {
        if (Source.Text[start] == '0' && Peek(1) is 'x' or 'X')
        {
            _pos += 2;
            var digitsStart = _pos;
            while (_pos < Source.Length && char.IsAsciiHexDigit(Source.Text[_pos])) _pos++;
            if (_pos == digitsStart)
            {
                AddDiagnostic(DiagnosticId.HexLiteralMissingDigits, start);
                return (SyntaxKind.BadToken, Intern(start));
            }
            return (SyntaxKind.IntegerLiteralToken, Intern(start));
        }

        while (_pos < Source.Length && char.IsAsciiDigit(Source.Text[_pos])) _pos++;
        if (Peek(0) == '.' && _pos + 1 < Source.Length && char.IsAsciiDigit(Source.Text[_pos + 1]))
        {
            _pos++;
            while (_pos < Source.Length && char.IsAsciiDigit(Source.Text[_pos])) _pos++;
            return (SyntaxKind.FloatLiteralToken, Intern(start));
        }
        return (SyntaxKind.IntegerLiteralToken, Intern(start));
    }

    private (SyntaxKind Kind, string Text) ScanStringLiteral(int start)
    {
        _pos++;
        while (_pos < Source.Length)
        {
            var c = Source.Text[_pos];
            if (c == '\\')
            {
                _pos += 2;
                if (_pos > Source.Length) _pos = Source.Length;
                continue;
            }
            if (c == '"')
            {
                _pos++;
                return (SyntaxKind.StringLiteralToken, Intern(start));
            }
            if (c is '\r' or '\n') break;
            _pos++;
        }
        AddDiagnostic(DiagnosticId.UnterminatedStringLiteral, start);
        return (SyntaxKind.StringLiteralToken, Intern(start));
    }

    private (SyntaxKind Kind, string Text) BadToken(int start)
    {
        _pos++;
        AddDiagnostic(DiagnosticId.InvalidCharacter, start);
        return (SyntaxKind.BadToken, Intern(start));
    }

    // Identifiers, numbers, and string literals are repeated massively across files (script names, parameter names, common literals)
    // sharing them globally cuts many token-text allocations.
    // The global cache is capped to avoid endless growth
    private static readonly ConcurrentDictionary<string, string> GlobalInterned = new(StringComparer.Ordinal);
    private static int _globalInternedCount;
    private const int MaxGlobalInterned = 200_000;

    private readonly Dictionary<string, string> _localInterned = new();

    private string Intern(int start) => Intern(Source.Text.AsSpan(start, _pos - start));

    private string Intern(ReadOnlySpan<char> span)
    {
        if (_globalInternedCount < MaxGlobalInterned)
        {
            var global = GlobalInterned.GetAlternateLookup<ReadOnlySpan<char>>();
            if (global.TryGetValue(span, out var existing)) return existing;
            var s = span.ToString();
            if (global.TryAdd(span, s))
            {
                Interlocked.Increment(ref _globalInternedCount);
            }
            return s;
        }

        var local = _localInterned.GetAlternateLookup<ReadOnlySpan<char>>();
        if (local.TryGetValue(span, out var localExisting)) return localExisting;
        var localNew = span.ToString();
        local.TryAdd(span, localNew);
        return localNew;
    }

    private char Peek(int offset) => _pos + offset < Source.Length ? Source.Text[_pos + offset] : '\0';

    private void AddDiagnostic(DiagnosticId id, int start)
    {
        Diagnostics.Add(new PapyrusDiagnostic(id, GetDiagnosticMessage(id), new TextSpan(start, _pos - start)));
    }

    private static string GetDiagnosticMessage(DiagnosticId id) => id switch
    {
        DiagnosticId.InvalidCharacter => "Invalid character in source.",
        DiagnosticId.UnterminatedStringLiteral => "Unterminated string literal.",
        DiagnosticId.UnterminatedBlockComment => "Unterminated block comment; missing '/;'.",
        DiagnosticId.UnterminatedDocumentationComment => "Unterminated documentation comment; missing '}'.",
        DiagnosticId.HexLiteralMissingDigits => "Hex literal is missing digits.",
        _ => "Unknown diagnostic.",
    };

    private static bool IsIdentifierStart(char c) => char.IsAsciiLetter(c) || c == '_' || c > 127;

    private static bool IsIdentifierPart(char c) => IsIdentifierStart(c) || char.IsAsciiDigit(c);
}
