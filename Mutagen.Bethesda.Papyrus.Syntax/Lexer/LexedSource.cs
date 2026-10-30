namespace Mutagen.Bethesda.Papyrus.Syntax.Lexer;

/// <summary>
/// The result of lexing a complete source file: the token stream (terminated by an
/// end-of-file token) and any diagnostics produced along the way.
/// </summary>
public sealed record LexedSource(SourceText Source, LexedToken[] Tokens, PapyrusDiagnostic[] Diagnostics);
