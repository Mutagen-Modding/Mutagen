namespace Mutagen.Bethesda.Papyrus.Syntax;

public enum DiagnosticId
{
    InvalidCharacter,
    UnexpectedToken,
    DuplicateMember,
    DuplicateParameter,
    DuplicateLocal,
    UnterminatedStringLiteral,
    UnterminatedBlockComment,
    UnterminatedDocumentationComment,
    HexLiteralMissingDigits,
}

/// <summary>
/// A diagnostic reported while scanning or parsing a source file.
/// </summary>
public sealed record PapyrusDiagnostic(DiagnosticId Id, string Message, TextSpan Span)
{
    public override string ToString() => $"{Id} at {Span}: {Message}";
}
