using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Cast semantics: <c>x as ScriptType</c> gives the expression the target script's
/// type, so member calls on the cast resolve against that script and its base chain.
/// </summary>
public sealed class CastTests
{
    private static PapyrusSemanticModel CreateModel(string source) => PapyrusSemanticModel.CreateFromSource(source);

    private static string DeclaringScriptName(IFunctionSymbol function)
    {
        for (var current = function.ContainingSymbol; current != null; current = current.ContainingSymbol)
        {
            if (current is ScriptSymbol script) return script.Name;
        }
        return "";
    }

    [Fact]
    public void Cast_ExpressionHasTargetType()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo(ObjectReference obj)
                Actor actor = obj as Actor
            EndFunction
            """);
        var body = model.GetBody("Foo")!;
        var initializer = body.Statements[0].ShouldBeOfType<LocalDeclarationStatement>().Initializer!;

        initializer.ShouldBeOfType<CastExpression>();
        initializer.Type!.Name.ShouldBe("Actor");
    }

    [Fact]
    public void Cast_ToScriptInCompilation_MemberCallResolves()
    {
        var compilation = new PapyrusCodebase(
        [
            SyntaxTree.Parse("ScriptName Quest"),
            SyntaxTree.Parse("""
                ScriptName OtherScript extends Quest
                Function Call()
                EndFunction
                """),
            SyntaxTree.Parse("""
                ScriptName TopicInfo
                Quest Function GetOwningQuest()
                EndFunction
                """),
            SyntaxTree.Parse("""
                Scriptname Fragment extends TopicInfo
                Function Fragment_0(ObjectReference akSpeakerRef)
                    (GetOwningQuest() as OtherScript).Call()
                EndFunction
                """),
        ]);
        var model = compilation.GetModel("Fragment")!;

        var call = model.GetBody("Fragment_0")!.DescendantNodes().OfType<CallExpression>()
            .Single(c => c.Receiver is CastExpression);

        var receiver = call.Receiver.ShouldNotBeNull();
        receiver.ShouldBeOfType<CastExpression>();
        receiver.Type!.Name.ShouldBe("OtherScript");

        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Call");
        DeclaringScriptName(call.Function).ShouldBe("OtherScript");
    }

    [Fact]
    public void Cast_ToExternalScript_MemberCallStaysStructured()
    {
        // The cast target has no source available (compiled-only script); the
        // receiver still binds with the target's type and the call keeps its name.
        var model = CreateModel("""
            Scriptname Fragment extends TopicInfo
            Function Fragment_0(ObjectReference akSpeakerRef)
                (GetOwningQuest() as OtherScript).Call()
            EndFunction
            """);
        var call = model.GetBody("Fragment_0")!.DescendantNodes().OfType<CallExpression>()
            .Single(c => c.Receiver is CastExpression);

        var receiver = call.Receiver.ShouldNotBeNull();
        receiver.ShouldBeOfType<CastExpression>();
        receiver.Type!.Name.ShouldBe("OtherScript");

        call.Name.ShouldBe("Call");
        call.Function.ShouldBeNull();
    }

    [Fact]
    public void Cast_ToScriptInCompilation_ResolvesThroughBaseChain()
    {
        var compilation = new PapyrusCodebase(
        [
            SyntaxTree.Parse("ScriptName Quest"),
            SyntaxTree.Parse("""
                ScriptName BaseScript extends Quest
                Function Call()
                EndFunction
                """),
            SyntaxTree.Parse("ScriptName OtherScript extends BaseScript"),
            SyntaxTree.Parse("""
                ScriptName TopicInfo
                Quest Function GetOwningQuest()
                EndFunction
                """),
            SyntaxTree.Parse("""
                Scriptname Fragment extends TopicInfo
                Function Fragment_0(ObjectReference akSpeakerRef)
                    (GetOwningQuest() as OtherScript).Call()
                EndFunction
                """),
        ]);
        var model = compilation.GetModel("Fragment")!;

        var call = model.GetBody("Fragment_0")!.DescendantNodes().OfType<CallExpression>()
            .Single(c => c.Receiver is CastExpression);

        call.Function.ShouldNotBeNull();
        DeclaringScriptName(call.Function).ShouldBe("BaseScript");
    }
}
