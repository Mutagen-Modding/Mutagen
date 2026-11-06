using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Semantics of the parent keyword, including the re-dispatch quirk:
/// https://ck.uesp.net/wiki/Function_Reference#Notes_on_using_Parent
/// a parent call executes the nearest ancestor's function "as" the direct parent,
/// so when intermediate scripts do not declare the function, the same function can
/// execute multiple times on the way up the hierarchy.
/// </summary>
public sealed class ParentCallTests
{
    // The wiki's 4-script example (debug.trace lines omitted).
    private static PapyrusCodebase CreateQuirkCompilation() => new(
    [
        SyntaxTree.Parse("ScriptName ObjectReference"),
        SyntaxTree.Parse("""
            ScriptName ScriptA extends ObjectReference
            Function Foo()
            EndFunction
            """),
        SyntaxTree.Parse("""
            ScriptName ScriptB extends ScriptA
            Function Foo()
                parent.Foo()
            EndFunction
            """),
        SyntaxTree.Parse("ScriptName ScriptC extends ScriptB"),
        SyntaxTree.Parse("""
            ScriptName ScriptD extends ScriptC
            Function Foo()
                parent.Foo()
            EndFunction
            """),
        SyntaxTree.Parse("""
            ScriptName ScriptCaller
            Function Bar()
                ScriptB.Foo()
            EndFunction
            """),
    ]);

    private static string DeclaringScriptName(IFunctionSymbol function)
    {
        for (var current = function.ContainingSymbol; current != null; current = current.ContainingSymbol)
        {
            if (current is ScriptSymbol script) return script.Name;
        }
        return "";
    }

    [Fact]
    public void ParentCall_ResolvesToNearestAncestor_ButExecutesAsDirectParent()
    {
        var compilation = CreateQuirkCompilation();
        var model = compilation.GetModel("ScriptD")!;

        var call = model.GetBody("Foo")!.DescendantNodes().OfType<CallExpression>().Single();

        call.IsParentCall.ShouldBeTrue();
        var receiver = (ScriptExpression)call.Receiver!;
        receiver.IsParent.ShouldBeTrue();
        receiver.Script.Name.ShouldBe("ScriptC");          // executes as the direct parent
        DeclaringScriptName(call.Function!).ShouldBe("ScriptB"); // found higher up
    }

    [Fact]
    public void ExpandParentCall_FollowsTheQuirk()
    {
        var compilation = CreateQuirkCompilation();
        var model = compilation.GetModel("ScriptD")!;
        var call = model.GetBody("Foo")!.DescendantNodes().OfType<CallExpression>().Single();

        var steps = call.ExpandParentCall(model);

        // ScriptB.Foo executes twice: once "as ScriptC" (found through it), then
        // once "as ScriptB", before reaching ScriptA.
        steps.Count.ShouldBe(3);
        steps[0].Function.Name.ShouldBe("Foo");
        DeclaringScriptName(steps[0].Function).ShouldBe("ScriptB");
        steps[0].ExecutedAs.Name.ShouldBe("ScriptC");
        DeclaringScriptName(steps[1].Function).ShouldBe("ScriptB");
        steps[1].ExecutedAs.Name.ShouldBe("ScriptB");
        DeclaringScriptName(steps[2].Function).ShouldBe("ScriptA");
        steps[2].ExecutedAs.Name.ShouldBe("ScriptA");
    }

    [Fact]
    public void ExpandParentCall_TenScriptExample()
    {
        var trees = new List<SyntaxTree> { SyntaxTree.Parse("ScriptName ObjectReference") };
        trees.Add(SyntaxTree.Parse("""
            ScriptName Script1 extends ObjectReference
            Function Foo()
                parent.Foo()
            EndFunction
            """));
        for (var i = 2; i <= 9; i++)
        {
            trees.Add(SyntaxTree.Parse($"ScriptName Script{i} extends Script{i - 1}"));
        }
        trees.Add(SyntaxTree.Parse("""
            ScriptName Script10 extends Script9
            Function Foo()
                parent.Foo()
            EndFunction
            """));
        var compilation = new PapyrusCodebase(trees);
        var model = compilation.GetModel("Script10")!;
        var call = model.GetBody("Foo")!.DescendantNodes().OfType<CallExpression>().Single();

        var steps = call.ExpandParentCall(model);

        // The parent call executes Script1.Foo 9 times (as Scripts 9 down to 1).
        // The wiki's "10 times" includes Script10.Foo itself, which is the caller,
        // not part of the parent call expansion.
        steps.Count.ShouldBe(9);
        steps.Count(s => DeclaringScriptName(s.Function) == "Script1").ShouldBe(9);
        steps.Select(s => s.ExecutedAs.Name)
            .ShouldBe(["Script9", "Script8", "Script7", "Script6", "Script5", "Script4", "Script3", "Script2", "Script1"]);
    }

    [Fact]
    public void ExpandParentCall_WhenParentDeclaresFunction_SingleStep()
    {
        var compilation = CreateQuirkCompilation();
        var model = compilation.GetModel("ScriptB")!;
        var call = model.GetBody("Foo")!.DescendantNodes().OfType<CallExpression>().Single();

        var steps = call.ExpandParentCall(model);

        steps.Count.ShouldBe(1);
        DeclaringScriptName(steps[0].Function).ShouldBe("ScriptA");
        steps[0].ExecutedAs.Name.ShouldBe("ScriptA");
    }

    [Fact]
    public void IsParentCall_DistinguishesParentFromScriptName()
    {
        var compilation = CreateQuirkCompilation();
        var model = compilation.GetModel("ScriptCaller")!;
        var call = model.GetBody("Bar")!.DescendantNodes().OfType<CallExpression>().Single();

        // ScriptB.Foo() goes through a script reference, not the parent keyword.
        call.Receiver.ShouldNotBeNull();
        call.Receiver.ShouldBeOfType<ScriptExpression>();
        ((ScriptExpression)call.Receiver).IsParent.ShouldBeFalse();
        call.IsParentCall.ShouldBeFalse();
        call.ExpandParentCall(model).ShouldBeEmpty();
    }

    [Fact]
    public void ExpandParentCall_WithoutCompilation_ReturnsDirectStepOnly()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName ScriptB extends ScriptA
            Function Foo()
                parent.Foo()
            EndFunction
            """);
        var call = model.GetBody("Foo")!.DescendantNodes().OfType<CallExpression>().Single();

        // Without a compilation the parent itself is unresolved, so the call does
        // not even bind as a parent call.
        call.IsParentCall.ShouldBeFalse();
        call.ExpandParentCall(model).ShouldBeEmpty();
    }
}
