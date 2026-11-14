using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Built-in functions provided by the runtime rather than script source:
/// ScriptObject natives (GetState, GoToState) available on every script, the array
/// Find function, and events being callable like functions.
/// </summary>
public sealed class BuiltinFunctionTests
{
    private static List<CallExpression> GetCalls(PapyrusSemanticModel model) =>
        model.GetBodies()
            .SelectMany(body => body.DescendantNodes())
            .OfType<CallExpression>()
            .ToList();

    [Fact]
    public void GoToState_Unqualified_ResolvesToNativeFunction()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Function Foo()
                GoToState("Busy")
            EndFunction
            """);

        var call = GetCalls(model).Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("GoToState");
        call.Function.IsNative.ShouldBeTrue();
        call.Function.ContainingSymbol.ShouldBeSameAs(PapyrusBuiltins.ScriptObject);
        call.Function.Parameters.Single().Type.Name.ShouldBe("string");
    }

    [Fact]
    public void GetState_OnSelf_ResolvesWithReceiver()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Function Foo()
                string s = self.GetState()
            EndFunction
            """);

        var call = GetCalls(model).Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("GetState");
        call.Function.ReturnType.ShouldNotBeNull();
        call.Function.ReturnType.Name.ShouldBe("string");
        call.Receiver.ShouldNotBeNull();
        call.Receiver.ShouldBeOfType<SelfExpression>();
    }

    [Fact]
    public void GoToState_OnUnknownForeignReceiver_Resolves()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Function Foo(MissingScript m)
                m.GoToState("x")
            EndFunction
            """);

        // ScriptObject natives apply to every script instance, even when the
        // receiver's script is not part of the compilation.
        var call = GetCalls(model).Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("GoToState");
    }

    [Fact]
    public void ArrayFind_ResolvesWithElementType()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Quest Property Quests Auto
            Function Foo()
                int[] numbers = new int[5]
                int i = numbers.Find(3)
                Quest[] scripts = new Quest[5]
                Quest q = scripts[0]
                int j = scripts.Find(q)
            EndFunction
            """);

        var calls = GetCalls(model);
        calls.Count.ShouldBe(2);

        var intFind = calls[0];
        intFind.Function.ShouldNotBeNull();
        intFind.Function.Name.ShouldBe("Find");
        intFind.Function.ReturnType!.Name.ShouldBe("int");
        intFind.Function.IsNative.ShouldBeTrue();
        intFind.Function.Parameters.Single().Type.Name.ShouldBe("int");

        var questFind = calls[1];
        questFind.Function.ShouldNotBeNull();
        questFind.Function.Parameters.Single().Type.Name.ShouldBe("Quest");
    }

    [Fact]
    public void ArrayFind_SharesSymbolPerElementType()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Function Foo()
                int[] a = new int[5]
                int[] b = new int[5]
                int i = a.Find(1)
                int j = b.Find(2)
            EndFunction
            """);

        var calls = GetCalls(model);
        calls[0].Function.ShouldNotBeNull();
        calls[1].Function.ShouldNotBeNull();
        calls[0].Function.ShouldBeSameAs(calls[1].Function!);
    }

    [Fact]
    public void GoToState_OnArrayOrPredefined_DoesNotResolve()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Function Foo()
                int[] arr = new int[5]
                arr.GoToState("x")
                int i = 0
                i.GetState()
            EndFunction
            """);

        var calls = GetCalls(model);
        calls.Count.ShouldBe(2);
        calls.All(c => c.Function == null).ShouldBeTrue();
    }

    [Fact]
    public void EventCall_OnParent_ResolvesToEventSymbol()
    {
        var compilation = new PapyrusCodebase([
            SyntaxTree.Parse("""
                ScriptName Alpha
                Event OnUpdate()
                EndEvent
                """),
            SyntaxTree.Parse("""
                ScriptName Gamma extends Alpha
                Function Foo()
                    parent.OnUpdate()
                EndFunction
                """),
        ]);
        var gamma = compilation.GetModel("Gamma")!;

        var call = GetCalls(gamma).Single();
        call.Function.ShouldNotBeNull();
        call.Function.ShouldBeOfType<EventSymbol>();
        call.Function.Name.ShouldBe("OnUpdate");
    }

    [Fact]
    public void UnqualifiedEventCall_ResolvesToOwnEvent()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Function Foo()
                OnCustom()
            EndFunction
            Event OnCustom()
            EndEvent
            """);

        var call = GetCalls(model).Single();
        call.Function.ShouldNotBeNull();
        call.Function.ShouldBeOfType<EventSymbol>();
        call.Function.Name.ShouldBe("OnCustom");
    }

    [Fact]
    public void NativeEvent_IsFlagged()
    {
        var model = PapyrusSemanticModel.CreateFromSource("""
            ScriptName Test
            Event OnLoad() native
            """);

        model.Script.Events.Single().IsNative.ShouldBeTrue();
    }
}
