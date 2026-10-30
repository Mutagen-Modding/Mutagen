namespace Mutagen.Bethesda.Papyrus.Syntax;

public enum SyntaxKind
{
    // Trivia
    WhitespaceTrivia,
    EndOfLineTrivia,
    LineContinuationTrivia,
    LineCommentTrivia,
    BlockCommentTrivia,
    DocumentationTrivia,

    // Literals
    IntegerLiteralToken,
    FloatLiteralToken,
    StringLiteralToken,

    // Identifiers
    IdentifierToken,

    // Header keywords
    ScriptNameKeyword,
    ExtendsKeyword,
    ImportKeyword,

    // Type keywords
    BoolKeyword,
    IntKeyword,
    FloatKeyword,
    StringKeyword,

    // Literal keywords
    TrueKeyword,
    FalseKeyword,
    NoneKeyword,
    SelfKeyword,
    ParentKeyword,

    // Property keywords
    PropertyKeyword,
    EndPropertyKeyword,
    AutoKeyword,
    AutoReadOnlyKeyword,
    ConditionalKeyword,
    HiddenKeyword,

    // Function keywords
    FunctionKeyword,
    EndFunctionKeyword,
    NativeKeyword,
    GlobalKeyword,

    // Event keywords
    EventKeyword,
    EndEventKeyword,

    // State keywords
    StateKeyword,
    EndStateKeyword,

    // Statement keywords
    IfKeyword,
    ElseIfKeyword,
    ElseKeyword,
    EndIfKeyword,
    WhileKeyword,
    EndWhileKeyword,
    ReturnKeyword,
    NewKeyword,
    AsKeyword,
    LengthKeyword,

    // Punctuation
    OpenParenToken,
    CloseParenToken,
    OpenBracketToken,
    CloseBracketToken,
    CommaToken,
    DotToken,

    // Operators
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    PercentToken,
    EqualsToken,
    PlusEqualsToken,
    MinusEqualsToken,
    StarEqualsToken,
    SlashEqualsToken,
    PercentEqualsToken,
    EqualsEqualsToken,
    ExclamationEqualsToken,
    LessThanToken,
    GreaterThanToken,
    LessThanEqualsToken,
    GreaterThanEqualsToken,
    AmpersandAmpersandToken,
    PipePipeToken,
    ExclamationToken,

    // Syntax tree node kinds
    CompilationUnit,
    ScriptHeader,
    ExtendsClause,
    ImportDirective,
    VariableDeclaration,
    PropertyDeclaration,
    FunctionDeclaration,
    EventDeclaration,
    StateDeclaration,
    ParameterList,
    Parameter,
    EqualsValueClause,
    FunctionBody,
    SkippedTokens,

    LocalDeclarationStatement,
    ExpressionStatement,
    ReturnStatement,
    IfStatement,
    ElseIfClause,
    ElseClause,
    WhileStatement,

    LiteralExpression,
    IdentifierName,
    ParenthesizedExpression,
    PrefixUnaryExpression,
    BinaryExpression,
    CastExpression,
    MemberAccessExpression,
    ElementAccessExpression,
    InvocationExpression,
    ArgumentList,
    Argument,
    ArrayCreationExpression,
    AssignmentExpression,

    PredefinedType,
    NamedType,
    ArrayType,

    // Error / end
    BadToken,
    EndOfFileToken,
}
