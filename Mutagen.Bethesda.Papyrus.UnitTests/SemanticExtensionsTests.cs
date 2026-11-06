using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// The PapyrusSemanticExtensions bring model-scoped queries directly onto deeply
/// nested syntax nodes, symbols, and code nodes by passing the model in.
/// </summary>
public sealed class SemanticExtensionsTests
{
    private const string Source = """
        ScriptName Test
        Quest Property MainQuest Auto
        int counter = 0

        Event OnInit()
            while (counter < 5)
                if (counter > 0)
                    MainQuest.SetStage(counter)
                endIf
                counter += 1
            endWhile
        EndEvent

        Function Advance()
            counter += 1
        EndFunction
        """;

    private static PapyrusSemanticModel CreateModel() =>
        PapyrusSemanticModel.CreateFromSource(Source);

    [Fact]
    public void SyntaxNode_GetSymbolInfo_OnDeeplyNestedIdentifier()
    {
        var model = CreateModel();
        // The `counter` inside `if (counter > 0)`, nested while > if.
        var identifier = model.Script.SyntaxTree.Root.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .First(i => i.Identifier.Text == "counter" && i.Parent is BinaryExpressionSyntax);

        identifier.GetSymbolInfo(model).ShouldBeOfType<VariableSymbol>();
        identifier.GetTypeInfo(model)!.Name.ShouldBe("int");
    }

    [Fact]
    public void SyntaxNode_GetExpression_ReturnsBoundNode()
    {
        var model = CreateModel();
        var invocation = model.Script.SyntaxTree.Root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>().Single();

        var bound = invocation.GetExpression(model);
        bound.ShouldNotBeNull();
        bound.ShouldBeOfType<CallExpression>();
    }

    [Fact]
    public void SyntaxNode_GetSymbol_And_GetBody()
    {
        var model = CreateModel();
        var functionDecl = model.Script.SyntaxTree.Root.DescendantNodes()
            .OfType<FunctionDeclarationSyntax>().Single();
        var eventDecl = model.Script.SyntaxTree.Root.DescendantNodes()
            .OfType<EventDeclarationSyntax>().Single();

        var functionSymbol = functionDecl.GetSymbol(model);
        functionSymbol.ShouldNotBeNull();
        functionSymbol.Name.ShouldBe("Advance");

        functionDecl.GetBody(model).ShouldNotBeNull();
        eventDecl.GetBody(model).ShouldNotBeNull();
    }

    [Fact]
    public void Symbol_GetBody()
    {
        var model = CreateModel();
        var function = model.Script.Functions.Single(f => f.Name == "Advance");
        var @event = model.Script.Events.Single();

        function.GetBody(model).ShouldNotBeNull();
        @event.GetBody(model).ShouldNotBeNull();
    }

    [Fact]
    public void CodeNode_GetContainingMember_And_Body_FromDeeplyNestedCall()
    {
        var model = CreateModel();
        // The SetStage call is nested while > if > expression statement.
        var call = model.GetBody("OnInit")!.DescendantNodes()
            .OfType<CallExpression>().Single();

        var member = call.GetContainingMember(model);
        member.ShouldNotBeNull();
        member.ShouldBeOfType<EventSymbol>();
        member.Name.ShouldBe("OnInit");

        var containingBody = call.GetContainingBody(model);
        containingBody.ShouldNotBeNull();
        containingBody.ShouldBeSameAs(model.GetBody("OnInit"));
    }

    [Fact]
    public void CodeNode_OutsideAnyBody_ReturnsNull()
    {
        var model = CreateModel();
        // A literal in a property initializer is not inside any function body.
        var literal = model.Script.SyntaxTree.Root.DescendantNodes()
            .OfType<LiteralExpressionSyntax>()
            .First(l => l.Token.Kind == SyntaxKind.IntegerLiteralToken);
        var bound = model.GetExpression(literal);
        bound.ShouldBeNull(); // initializers are not bound yet

        var badNode = new BadExpression(null);
        badNode.GetContainingMember(model).ShouldBeNull();
        badNode.GetContainingBody(model).ShouldBeNull();
    }

    [Fact]
    public void ScriptSymbol_GetModel_FromCompilation()
    {
        var compilation = new PapyrusCodebase([
            SyntaxTree.Parse("ScriptName Alpha"),
            SyntaxTree.Parse("ScriptName Beta"),
        ]);
        var alpha = compilation.Models[0].Script;

        alpha.GetModel(compilation).ShouldBeSameAs(compilation.Models[0]);
    }
}
