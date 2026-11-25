using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Cross-script resolution: member calls and property access on foreign script
/// types, global function calls via script names, `parent`, imports, and base
/// chains — all through a PapyrusCompilation holding many scripts.
/// </summary>
public sealed class CrossScriptTests
{
    private static PapyrusCodebase CreateCompilation(params string[] sources) =>
        new(sources.Select(SyntaxTree.Parse));

    private static List<CallExpression> GetCalls(PapyrusSemanticModel model) =>
        model.GetBodies()
            .SelectMany(body => body.DescendantNodes())
            .OfType<CallExpression>()
            .ToList();

    [Fact]
    public void ForeignMemberCall_ResolvesToTargetScriptFunction()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            Function Bar(int v)
            EndFunction
            """,
            """
            ScriptName Beta
            Function Foo(Alpha a)
                a.Bar(5)
            EndFunction
            """);
        var alpha = compilation.GetModel("Alpha")!.Script;
        var beta = compilation.GetModel("Beta")!;

        var call = GetCalls(beta).Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Bar");
        call.Function.ContainingSymbol.ShouldBeSameAs(alpha);
        call.Name.ShouldBe("Bar");
        call.Receiver.ShouldNotBeNull();
        call.Receiver.Type!.Name.ShouldBe("Alpha");
    }

    [Fact]
    public void ForeignMemberCall_ResolvesThroughBaseChain()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            Function Bar()
            EndFunction
            """,
            """
            ScriptName Gamma extends Alpha
            """,
            """
            ScriptName Beta
            Function Foo(Gamma g)
                g.Bar()
            EndFunction
            """);
        var alpha = compilation.GetModel("Alpha")!.Script;

        var call = GetCalls(compilation.GetModel("Beta")!).Single();
        call.Function.ShouldNotBeNull();
        call.Function.ContainingSymbol.ShouldBeSameAs(alpha);
    }

    [Fact]
    public void ForeignPropertyAccess_ResolvesMember()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            int Property Count Auto
            """,
            """
            ScriptName Beta
            Function Foo(Alpha a)
                int x = a.Count
            EndFunction
            """);

        var memberAccess = compilation.GetModel("Beta")!.GetBodies()
            .SelectMany(body => body.DescendantNodes())
            .OfType<MemberExpression>()
            .Single();
        memberAccess.Member.ShouldNotBeNull();
        memberAccess.Member.Name.ShouldBe("Count");
        memberAccess.Type!.Name.ShouldBe("int");
    }

    [Fact]
    public void GlobalCall_Qualified_ResolvesToGlobalFunction()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            int Function Add(int a, int b) global
                return a + b
            EndFunction
            """,
            """
            ScriptName Beta
            Function Foo()
                int x = Alpha.Add(1, 2)
            EndFunction
            """);
        var alpha = compilation.GetModel("Alpha")!.Script;

        var call = GetCalls(compilation.GetModel("Beta")!).Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Add");
        call.Function.IsGlobal.ShouldBeTrue();
        call.Receiver.ShouldBeOfType<ScriptExpression>();
        ((ScriptExpression)call.Receiver).Script.ShouldBeSameAs(alpha);
    }

    [Fact]
    public void GlobalCall_Unqualified_ResolvesViaImport()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            int Function Add(int a, int b) global
                return a + b
            EndFunction
            """,
            """
            ScriptName Beta
            Import Alpha
            Function Foo()
                int x = Add(1, 2)
            EndFunction
            """);

        var call = GetCalls(compilation.GetModel("Beta")!).Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Add");
        call.Function.IsGlobal.ShouldBeTrue();
    }

    [Fact]
    public void ParentCall_ResolvesToBaseScriptFunction()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            Function Bar()
            EndFunction
            """,
            """
            ScriptName Gamma extends Alpha
            Function Foo()
                parent.Bar()
            EndFunction
            """);
        var alpha = compilation.GetModel("Alpha")!.Script;

        var call = GetCalls(compilation.GetModel("Gamma")!).Single();
        call.Function.ShouldNotBeNull();
        call.Function.ContainingSymbol.ShouldBeSameAs(alpha);
        call.Receiver.ShouldBeOfType<ScriptExpression>();
        ((ScriptExpression)call.Receiver).Script.ShouldBeSameAs(alpha);
    }

    [Fact]
    public void UnqualifiedCall_ResolvesInheritedFunction()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            Function Bar()
            EndFunction
            """,
            """
            ScriptName Gamma extends Alpha
            Function Foo()
                Bar()
            EndFunction
            """);
        var alpha = compilation.GetModel("Alpha")!.Script;

        var call = GetCalls(compilation.GetModel("Gamma")!).Single();
        call.Function.ShouldNotBeNull();
        call.Function.ContainingSymbol.ShouldBeSameAs(alpha);
    }

    [Fact]
    public void UnknownForeignScript_StaysUnresolved()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Beta
            Function Foo(Missing m)
                m.Bar()
            EndFunction
            """);

        var call = GetCalls(compilation.GetModel("Beta")!).Single();
        call.Function.ShouldBeNull();
        call.Name.ShouldBe("Bar");
    }

    [Fact]
    public void Compilation_ResolvesScriptsCaseInsensitively_AndBaseScripts()
    {
        var compilation = CreateCompilation(
            """
            ScriptName Alpha
            """,
            """
            ScriptName Gamma extends Alpha
            """);

        compilation.GetModel("ALPHA").ShouldNotBeNull();
        compilation.GetModel("gamma").ShouldNotBeNull();
        var gamma = compilation.GetModel("Gamma")!.Script;
        gamma.BaseScript.ShouldNotBeNull();
        gamma.BaseScript.Name.ShouldBe("Alpha");
    }

    [Fact]
    public void SingleScriptModel_WithoutCompilation_StaysUnresolved()
    {
        var tree = SyntaxTree.Parse("""
            ScriptName Beta
            Function Foo(Alpha a)
                a.Bar()
            EndFunction
            """);
        var model = new PapyrusSemanticModel(tree);

        var call = GetCalls(model).Single();
        call.Function.ShouldBeNull();
    }

    [Fact]
    public void GetBoundBodies_EnumeratesAllBodies()
    {
        var tree = SyntaxTree.Parse("""
            ScriptName Test
            int Property Value Hidden
                Function Get()
                    return 5
                EndFunction
            EndProperty
            Event OnLoad()
            EndEvent
            Function Foo()
            EndFunction
            """);
        var model = new PapyrusSemanticModel(tree);

        model.GetBodies().Count().ShouldBe(3);
    }
}
