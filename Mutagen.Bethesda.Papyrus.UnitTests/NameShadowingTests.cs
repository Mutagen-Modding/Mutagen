using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Papyrus resolves names flat: script-level variables and properties take
/// priority over parameters and locals that share the same name — the opposite
/// of C#-style shadowing. These tests pin that compiler behavior.
/// </summary>
public sealed class NameShadowingTests
{
    private static PapyrusSemanticModel CreateModel(string source) => PapyrusSemanticModel.CreateFromSource(source);

    private static IdentifierNameSyntax FindIdentifier(SyntaxTree tree, string name)
    {
        return tree.Root.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Single(i => i.Identifier.Text == name);
    }

    [Fact]
    public void Property_TakesPriorityOverSameNamedParameter()
    {
        var model = CreateModel("""
            ScriptName Test
            ReferenceAlias Property Boss Auto
            Function OnBossFightStarted(Actor Boss)
                Boss.SetEssential()
            EndFunction
            """);
        var identifier = FindIdentifier(model.Script.SyntaxTree, "Boss");
        var symbol = model.GetSymbolInfo(identifier);

        symbol.ShouldBeOfType<PropertySymbol>();
        symbol.Name.ShouldBe("Boss");
    }

    [Fact]
    public void ScriptVariable_TakesPriorityOverSameNamedLocal()
    {
        var model = CreateModel("""
            ScriptName Test
            int count = 0
            Function Foo()
                int count = 5
                count += 1
            EndFunction
            """);
        var body = model.GetBody(model.Script.Functions.Single())!;
        var statement = body.Statements[1].ShouldBeOfType<ExpressionStatement>();
        var target = statement.Expression.ShouldBeOfType<AssignmentExpression>().Target;
        var symbol = model.GetSymbolInfo((IdentifierNameSyntax)target.Syntax!);

        symbol.ShouldBeOfType<VariableSymbol>();
        symbol.Name.ShouldBe("count");
    }

    [Fact]
    public void Property_TakesPriorityOverSameNamedLocal()
    {
        var model = CreateModel("""
            ScriptName Test
            bool Property done Auto
            Function Foo()
                bool done = false
                done = true
            EndFunction
            """);
        var body = model.GetBody(model.Script.Functions.Single())!;
        var statement = body.Statements[1].ShouldBeOfType<ExpressionStatement>();
        var target = statement.Expression.ShouldBeOfType<AssignmentExpression>().Target;
        var symbol = model.GetSymbolInfo((IdentifierNameSyntax)target.Syntax!);

        symbol.ShouldBeOfType<PropertySymbol>();
    }

    [Fact]
    public void LocalWithoutCollision_StillResolvesToLocal()
    {
        var model = CreateModel("""
            ScriptName Test
            int scriptVar = 0
            Function Foo()
                int x = 5
                x = scriptVar
            EndFunction
            """);
        var identifier = FindIdentifier(model.Script.SyntaxTree, "x");
        var symbol = model.GetSymbolInfo(identifier);

        symbol.ShouldBeOfType<VariableSymbol>();
        symbol.Name.ShouldBe("x");
    }

    [Fact]
    public void ParameterWithoutCollision_StillResolvesToParameter()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo(int amount)
                amount += 1
            EndFunction
            """);
        var identifier = FindIdentifier(model.Script.SyntaxTree, "amount");
        var symbol = model.GetSymbolInfo(identifier);

        symbol.ShouldBeOfType<ParameterSymbol>();
    }

    [Fact]
    public void Shadowing_DoesNotAffectOtherNames()
    {
        // The real-world case: property Boss shadows parameter Boss, but the rest
        // of the body — including the member call on the shadowed name — binds
        // normally through the property's type.
        var model = CreateModel("""
            ScriptName Test
            ReferenceAlias Property Boss Auto
            ActorBase BossActorBase
            Function OnBossFightStarted(Actor Boss)
                BossActorBase = Boss.GetActorBase()
            EndFunction
            """);
        var body = model.GetBody(model.Script.Functions.Single())!;
        var statement = body.Statements[0].ShouldBeOfType<ExpressionStatement>();
        var assignment = statement.Expression.ShouldBeOfType<AssignmentExpression>();
        var call = assignment.Value.ShouldBeOfType<CallExpression>();
        call.Name.ShouldBe("GetActorBase");

        var receiver = call.Receiver.ShouldNotBeNull();
        var symbol = model.GetSymbolInfo((IdentifierNameSyntax)receiver.Syntax!);
        symbol.ShouldBeOfType<PropertySymbol>();
    }
}
