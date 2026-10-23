using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

public sealed class ParserTests
{
    private static CompilationUnitSyntax Parse(string source) => SyntaxTree.Parse(source).Root;

    private static SyntaxTree ParseTree(string source) => SyntaxTree.Parse(source);

    [Fact]
    public void EmptySource_ParsesToEmptyCompilationUnit()
    {
        var tree = ParseTree("");
        tree.Diagnostics.ShouldBeEmpty();
        tree.Root.Members.Count.ShouldBe(0);
        tree.Root.ToFullString().ShouldBe("");
    }

    [Fact]
    public void ExampleScript_ParsesWithoutDiagnostics()
    {
        var tree = ParseTree(ExampleScript.Source);
        tree.Diagnostics.ShouldBeEmpty();

        var root = tree.Root;
        root.Header.Name.Text.ShouldBe("BSKDefaultMoveActorsToSelfOnLoaded");
        root.Header.ExtendsClause.ShouldNotBeNull();
        root.Header.ExtendsClause.BaseType.ShouldBeOfType<NamedTypeSyntax>();
        root.Members.Count.ShouldBe(3);
        root.Members[0].ShouldBeOfType<PropertyDeclarationSyntax>();
        root.Members[1].ShouldBeOfType<EventDeclarationSyntax>();
    }

    [Fact]
    public void ExampleScript_RoundTrips()
    {
        var tree = ParseTree(ExampleScript.Source);
        tree.Root.ToFullString().ShouldBe(ExampleScript.Source);
    }

    [Fact]
    public void EventBody_HasExpectedStatements()
    {
        var root = Parse(ExampleScript.Source);
        var onLoad = (EventDeclarationSyntax)root.Members[1];
        onLoad.Identifier.Text.ShouldBe("OnLoad");
        onLoad.ParameterList.Parameters.Count.ShouldBe(0);
        onLoad.Body.ShouldNotBeNull();
        var statements = onLoad.Body.Statements;
        statements.Count.ShouldBe(2);
        statements[0].ShouldBeOfType<LocalDeclarationStatementSyntax>();
        statements[1].ShouldBeOfType<WhileStatementSyntax>();
    }

    [Fact]
    public void WhileStatement_Structure()
    {
        var root = Parse(ExampleScript.Source);
        var onLoad = (EventDeclarationSyntax)root.Members[1];
        var whileStatement = (WhileStatementSyntax)onLoad.Body!.Statements[1];
        whileStatement.Condition.ShouldBeOfType<ParenthesizedExpressionSyntax>();
        whileStatement.Statements.Count.ShouldBe(3);
        whileStatement.EndWhileToken.Text.ShouldBe("EndWhile");
    }

    [Fact]
    public void LocalDeclaration_WithInitializer()
    {
        var root = Parse(ExampleScript.Source);
        var onLoad = (EventDeclarationSyntax)root.Members[1];
        var declaration = (LocalDeclarationStatementSyntax)onLoad.Body!.Statements[0];
        declaration.Type.ShouldBeOfType<PredefinedTypeSyntax>();
        declaration.Identifier.Text.ShouldBe("i");
        declaration.Initializer.ShouldNotBeNull();
        declaration.Initializer.Value.ShouldBeOfType<LiteralExpressionSyntax>();
    }

    [Fact]
    public void AssignmentStatement_PlusEquals()
    {
        var root = Parse(ExampleScript.Source);
        var onLoad = (EventDeclarationSyntax)root.Members[1];
        var whileStatement = (WhileStatementSyntax)onLoad.Body!.Statements[1];
        var statement = (ExpressionStatementSyntax)whileStatement.Statements[2];
        var assignment = (AssignmentExpressionSyntax)statement.Expression;
        assignment.OperatorToken.Kind.ShouldBe(SyntaxKind.PlusEqualsToken);
        assignment.Left.ShouldBeOfType<IdentifierNameSyntax>();
        assignment.Right.ShouldBeOfType<LiteralExpressionSyntax>();
    }

    [Fact]
    public void MemberAccessAndInvocation()
    {
        var root = Parse(ExampleScript.Source);
        var teleport = (FunctionDeclarationSyntax)root.Members[2];
        var ifStatement = (IfStatementSyntax)teleport.Body!.Statements[0];
        var expressionStatement = (ExpressionStatementSyntax)ifStatement.Statements[0];
        var invocation = (InvocationExpressionSyntax)expressionStatement.Expression;
        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
        memberAccess.Expression.ShouldBeOfType<IdentifierNameSyntax>();
        memberAccess.Name.Text.ShouldBe("MoveTo");
        invocation.ArgumentList.Arguments[0].Expression.Kind.ShouldBe(SyntaxKind.LiteralExpression);
    }

    [Fact]
    public void ElementAccess_InInitializer()
    {
        var root = Parse(ExampleScript.Source);
        var onLoad = (EventDeclarationSyntax)root.Members[1];
        var whileStatement = (WhileStatementSyntax)onLoad.Body!.Statements[1];
        var declaration = (LocalDeclarationStatementSyntax)whileStatement.Statements[0];
        var initializer = declaration.Initializer!.Value;
        initializer.ShouldBeOfType<ElementAccessExpressionSyntax>();
        var elementAccess = (ElementAccessExpressionSyntax)initializer;
        elementAccess.Expression.ShouldBeOfType<IdentifierNameSyntax>();
        elementAccess.Index.ShouldBeOfType<IdentifierNameSyntax>();
    }

    [Fact]
    public void ArrayProperty()
    {
        var root = Parse(ExampleScript.Source);
        var property = (PropertyDeclarationSyntax)root.Members[0];
        property.Type.ShouldBeOfType<ArrayTypeSyntax>();
        property.Identifier.Text.ShouldBe("TeleportActors");
        property.Flags.Any(f => f.Kind == SyntaxKind.AutoKeyword).ShouldBeTrue();
        property.EndPropertyToken.ShouldBeNull();
    }

    [Fact]
    public void FunctionParameter()
    {
        var root = Parse(ExampleScript.Source);
        var teleport = (FunctionDeclarationSyntax)root.Members[2];
        teleport.ReturnType.ShouldBeNull();
        teleport.Identifier.Text.ShouldBe("Teleport");
        teleport.ParameterList.Parameters.Count.ShouldBe(1);
        teleport.ParameterList.Parameters[0].Identifier.Text.ShouldBe("akActor");
    }

    [Fact]
    public void Precedence_MultiplicationBindsTighter()
    {
        var root = Parse("""
            ScriptName Test
            int x = 1 + 2 * 3
            """);
        var member = (VariableDeclarationSyntax)root.Members[0];
        var value = member.Initializer!.Value;
        var addition = (BinaryExpressionSyntax)value;
        addition.OperatorToken.Kind.ShouldBe(SyntaxKind.PlusToken);
        addition.Left.ShouldBeOfType<LiteralExpressionSyntax>();
        var multiplication = (BinaryExpressionSyntax)addition.Right;
        multiplication.OperatorToken.Kind.ShouldBe(SyntaxKind.StarToken);
    }

    [Fact]
    public void Precedence_LogicalOrLowest()
    {
        var root = Parse("""
            ScriptName Test
            bool x = a || b && c
            """);
        var member = (VariableDeclarationSyntax)root.Members[0];
        var orExpression = (BinaryExpressionSyntax)member.Initializer!.Value;
        orExpression.OperatorToken.Kind.ShouldBe(SyntaxKind.PipePipeToken);
        var andExpression = (BinaryExpressionSyntax)orExpression.Right;
        andExpression.OperatorToken.Kind.ShouldBe(SyntaxKind.AmpersandAmpersandToken);
    }

    [Fact]
    public void UnaryMinus_BindsCast()
    {
        // Per grammar: <unary> ::= ['-'] <cast atom>, so -x as int is -(x as int)
        var root = Parse("""
            ScriptName Test
            int x = -y as int
            """);
        var member = (VariableDeclarationSyntax)root.Members[0];
        var unary = (PrefixUnaryExpressionSyntax)member.Initializer!.Value;
        unary.OperatorToken.Kind.ShouldBe(SyntaxKind.MinusToken);
        unary.Operand.ShouldBeOfType<CastExpressionSyntax>();
    }

    [Fact]
    public void MemberInvocationChain()
    {
        var root = Parse("""
            ScriptName Test
            int x = a.b().c
            """);
        var member = (VariableDeclarationSyntax)root.Members[0];
        var memberAccess = (MemberAccessExpressionSyntax)member.Initializer!.Value;
        memberAccess.Name.Text.ShouldBe("c");
        var invocation = (InvocationExpressionSyntax)memberAccess.Expression;
        invocation.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
    }

    [Fact]
    public void NamedArgument()
    {
        var root = Parse("""
            ScriptName Test
            Event OnLoad()
                Foo(a = 1, b)
            EndEvent
            """);
        var eventDeclaration = (EventDeclarationSyntax)root.Members[0];
        var statement = (ExpressionStatementSyntax)eventDeclaration.Body!.Statements[0];
        var invocation = (InvocationExpressionSyntax)statement.Expression;
        invocation.ArgumentList.Arguments.Count.ShouldBe(2);
        invocation.ArgumentList.Arguments[0].Name.ShouldNotBeNull();
        invocation.ArgumentList.Arguments[0].Name!.Text.ShouldBe("a");
        invocation.ArgumentList.Arguments[1].Name.ShouldBeNull();
    }

    [Fact]
    public void ArrayCreation()
    {
        var root = Parse("""
            ScriptName Test
            Actor[] actors = new Actor[10]
            """);
        var member = (VariableDeclarationSyntax)root.Members[0];
        member.Type.ShouldBeOfType<ArrayTypeSyntax>();
        var creation = (ArrayCreationExpressionSyntax)member.Initializer!.Value;
        creation.ElementType.ShouldBeOfType<NamedTypeSyntax>();
        creation.SizeExpression.ShouldBeOfType<LiteralExpressionSyntax>();
    }

    [Fact]
    public void IfElseIfElse()
    {
        var root = Parse("""
            ScriptName Test
            Event OnLoad()
                if (a)
                    Foo()
                elseif (b)
                    Bar()
                else
                    Baz()
                endIf
            EndEvent
            """);
        var eventDeclaration = (EventDeclarationSyntax)root.Members[0];
        var ifStatement = (IfStatementSyntax)eventDeclaration.Body!.Statements[0];
        ifStatement.Statements.Count.ShouldBe(1);
        ifStatement.ElseIfClauses.Count.ShouldBe(1);
        ifStatement.ElseIfClauses[0].Condition.ShouldBeOfType<ParenthesizedExpressionSyntax>();
        ifStatement.ElseClause.ShouldNotBeNull();
        ifStatement.ElseClause.Statements.Count.ShouldBe(1);
    }

    [Fact]
    public void ReturnWithAndWithoutValue()
    {
        var root = Parse("""
            ScriptName Test
            Function Foo()
                if (a)
                    return 5
                endIf
                return
            EndFunction
            """);
        var function = (FunctionDeclarationSyntax)root.Members[0];
        var ifStatement = (IfStatementSyntax)function.Body!.Statements[0];
        var returnWithValue = (ReturnStatementSyntax)ifStatement.Statements[0];
        returnWithValue.Expression.ShouldNotBeNull();
        var returnWithoutValue = (ReturnStatementSyntax)function.Body.Statements[1];
        returnWithoutValue.Expression.ShouldBeNull();
    }

    [Fact]
    public void AutoReadOnlyProperty()
    {
        var root = Parse("""
            ScriptName Test
            int Property Version = 5 AutoReadOnly
            """);
        var property = (PropertyDeclarationSyntax)root.Members[0];
        property.Initializer.ShouldNotBeNull();
        property.Flags.Any(f => f.Kind == SyntaxKind.AutoReadOnlyKeyword).ShouldBeTrue();
    }

    [Fact]
    public void FullProperty_WithGetterAndSetter()
    {
        var root = Parse("""
            ScriptName Test
            int Property Value Hidden
                Function Get()
                    return 5
                EndFunction
                Function Set(int newValue)
                EndFunction
            EndProperty
            """);
        var property = (PropertyDeclarationSyntax)root.Members[0];
        property.Getter.ShouldNotBeNull();
        property.Setter.ShouldNotBeNull();
        property.EndPropertyToken.ShouldNotBeNull();
        property.Getter.Body!.Statements[0].ShouldBeOfType<ReturnStatementSyntax>();
    }

    [Fact]
    public void AutoState()
    {
        var root = Parse("""
            ScriptName Test
            Auto State Busy
                Event OnLoad()
                EndEvent
            EndState
            """);
        var state = (StateDeclarationSyntax)root.Members[0];
        state.AutoKeyword.ShouldNotBeNull();
        state.Identifier.Text.ShouldBe("Busy");
        state.Members.Count.ShouldBe(1);
        state.Members[0].ShouldBeOfType<EventDeclarationSyntax>();
    }

    [Fact]
    public void AutoState_AfterAutoProperty_KeepsAuto()
    {
        // Regression: the property flag loop used to consume the state's Auto as a
        // duplicate property flag, silently turning `Auto State` into `State`.
        var root = Parse("""
            ScriptName Test
            int Property Value Auto
            Auto State Busy
                Event OnLoad()
                EndEvent
            EndState
            """);
        var property = (PropertyDeclarationSyntax)root.Members[0];
        property.Flags.Single().Kind.ShouldBe(SyntaxKind.AutoKeyword);

        var state = (StateDeclarationSyntax)root.Members[1];
        state.AutoKeyword.ShouldNotBeNull();
        state.Identifier.Text.ShouldBe("Busy");
    }

    [Fact]
    public void NativeAndGlobalFunctions()
    {
        var root = Parse("""
            ScriptName Test
            int Function Foo() native
            Function Bar() global
                Foo()
            EndFunction
            """);
        var nativeFunction = (FunctionDeclarationSyntax)root.Members[0];
        nativeFunction.IsNative.ShouldBeTrue();
        nativeFunction.Body.ShouldBeNull();
        var globalFunction = (FunctionDeclarationSyntax)root.Members[1];
        globalFunction.IsGlobal.ShouldBeTrue();
        globalFunction.Body.ShouldNotBeNull();
    }

    [Fact]
    public void ImportDirective()
    {
        var root = Parse("""
            ScriptName Test
            Import Utility
            """);
        var import = (ImportDirectiveSyntax)root.Members[0];
        import.ScriptName.Text.ShouldBe("Utility");
    }

    [Fact]
    public void ScriptFlags()
    {
        var root = Parse("""
            ScriptName Test extends ObjectReference Hidden Conditional
            """);
        root.Header.Flags.Count.ShouldBe(2);
        root.Header.Flags[0].Kind.ShouldBe(SyntaxKind.HiddenKeyword);
        root.Header.Flags[1].Kind.ShouldBe(SyntaxKind.ConditionalKeyword);
    }

    [Fact]
    public void ParentWiring()
    {
        var tree = ParseTree(ExampleScript.Source);
        var root = tree.Root;
        root.Parent.ShouldBeNull();
        foreach (var descendant in root.DescendantNodes())
        {
            descendant.SyntaxTree.ShouldBeSameAs(tree);
            descendant.Parent.ShouldNotBeNull();
        }
        var property = (PropertyDeclarationSyntax)root.Members[0];
        property.Parent.ShouldBeSameAs(root);
        property.Identifier.Parent.ShouldBeSameAs(property);
    }

    [Fact]
    public void Spans_AreAbsolute()
    {
        var tree = ParseTree(ExampleScript.Source);
        var root = tree.Root;
        root.FullSpan.Start.ShouldBe(0);
        root.FullSpan.End.ShouldBe(ExampleScript.Source.Length);
        var property = (PropertyDeclarationSyntax)root.Members[0];
        property.Span.Start.ShouldBeGreaterThan(root.Header.FullSpan.End);
    }

    [Fact]
    public void MissingEndIf_ProducesDiagnosticAndMissingToken()
    {
        var tree = ParseTree("""
            ScriptName Test
            Event OnLoad()
                if (a)
                    Foo()
            EndEvent
            """);
        tree.Diagnostics.ShouldNotBeEmpty();
        tree.Diagnostics.ShouldContain(d => d.Id == DiagnosticId.UnexpectedToken);
        var eventDeclaration = (EventDeclarationSyntax)tree.Root.Members[0];
        var ifStatement = (IfStatementSyntax)eventDeclaration.Body!.Statements[0];
        ifStatement.EndIfToken.IsMissing.ShouldBeTrue();
    }

    [Fact]
    public void GarbageTokens_AreSkippedButPreserved()
    {
        var source = """
            ScriptName Test
            @@@ garbage
            int x = 5
            """;
        var tree = ParseTree(source);
        tree.Diagnostics.ShouldNotBeEmpty();
        tree.Root.ToFullString().ShouldBe(source);
        var variable = tree.Root.Members.OfType<VariableDeclarationSyntax>().Single();
        variable.Identifier.Text.ShouldBe("x");
    }

    [Fact]
    public void LineContinuation_InParameterList()
    {
        var source = """
            ScriptName Test
            Event OnTrapHit(float afX, \
            	float afY)
            EndEvent
            """;
        var tree = ParseTree(source);
        tree.Root.ToFullString().ShouldBe(source);
        var eventDeclaration = (EventDeclarationSyntax)tree.Root.Members[0];
        eventDeclaration.ParameterList.Parameters.Count.ShouldBe(2);
        eventDeclaration.ParameterList.Parameters[1].Identifier.Text.ShouldBe("afY");
    }

    [Fact]
    public void RoundTrips_WithDocCommentAndFlags()
    {
        var source = """
            ScriptName Test extends ObjectReference
            {Documentation comment}
            ; A comment
            bool Property bAuto Auto Conditional
            """;
        var tree = ParseTree(source);
        tree.Root.ToFullString().ShouldBe(source);
    }
}
