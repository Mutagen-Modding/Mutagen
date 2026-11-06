using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

public sealed class SymbolTests
{
    private static (ScriptSymbol Script, PapyrusDiagnostic[] Diagnostics) Bind(string source)
    {
        var tree = SyntaxTree.Parse(source);
        var binder = new ScriptBinder(tree);
        var script = binder.Bind();
        return (script, binder.Diagnostics.ToArray());
    }

    [Fact]
    public void ExampleScript_BindsWithoutDiagnostics()
    {
        var (script, diagnostics) = Bind(ExampleScript.Source);
        diagnostics.ShouldBeEmpty();

        script.Name.ShouldBe("BSKDefaultMoveActorsToSelfOnLoaded");
        script.BaseScriptName.ShouldBe("ObjectReference");
        script.Functions.Count.ShouldBe(1);
        script.Events.Count.ShouldBe(1);
        script.Properties.Count.ShouldBe(1);
        script.Variables.Count.ShouldBe(0);
        script.States.Count.ShouldBe(0);
    }

    [Fact]
    public void ScriptFlags()
    {
        var (script, _) = Bind("ScriptName Test extends ObjectReference Hidden Conditional");
        script.IsHidden.ShouldBeTrue();
        script.IsConditional.ShouldBeTrue();
    }

    [Fact]
    public void FunctionSymbol_Details()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            int Function Add(int a, int b) global
                return a + b
            EndFunction
            """);
        diagnostics.ShouldBeEmpty();

        var function = script.Functions.Single();
        function.Name.ShouldBe("Add");
        function.ReturnType.ShouldNotBeNull();
        function.ReturnType.Name.ShouldBe("int");
        function.ReturnType.IsArray.ShouldBeFalse();
        function.Parameters.Count.ShouldBe(2);
        function.Parameters[0].Name.ShouldBe("a");
        function.Parameters[1].Name.ShouldBe("b");
        function.IsGlobal.ShouldBeTrue();
        function.IsNative.ShouldBeFalse();
        function.Locals.Count.ShouldBe(0);
        function.ContainingSymbol.ShouldBeSameAs(script);
    }

    [Fact]
    public void NativeFunction_HasNoLocals()
    {
        var (script, _) = Bind("""
            ScriptName Test
            int Function Foo() native
            """);
        var function = script.Functions.Single();
        function.IsNative.ShouldBeTrue();
        function.Locals.Count.ShouldBe(0);
    }

    [Fact]
    public void EventSymbol_Parameters()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            Event OnHit(ObjectReference akTarget, int aiDamage)
            EndEvent
            """);
        diagnostics.ShouldBeEmpty();
        var @event = script.Events.Single();
        @event.Name.ShouldBe("OnHit");
        @event.Parameters.Count.ShouldBe(2);
        @event.Parameters[0].Name.ShouldBe("akTarget");
        @event.ContainingSymbol.ShouldBeSameAs(script);
    }

    [Fact]
    public void AutoProperty()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            bool Property bEnabled Auto Hidden
            """);
        diagnostics.ShouldBeEmpty();
        var property = script.Properties.Single();
        property.Name.ShouldBe("bEnabled");
        property.Type.Name.ShouldBe("bool");
        property.IsAuto.ShouldBeTrue();
        property.IsHidden.ShouldBeTrue();
        property.IsAutoReadOnly.ShouldBeFalse();
        property.Initializer.ShouldBeNull();
        property.Getter.ShouldBeNull();
        property.Setter.ShouldBeNull();
    }

    [Fact]
    public void PropertyWithInitializer()
    {
        var (script, _) = Bind("""
            ScriptName Test
            int Property Level = 5 AutoReadOnly
            """);
        var property = script.Properties.Single();
        property.IsAutoReadOnly.ShouldBeTrue();
        property.Initializer.ShouldNotBeNull();
    }

    [Fact]
    public void FullProperty_HasGetterAndSetter()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            int Property Value Hidden
                Function Get()
                    return 5
                EndFunction
                Function Set(int newValue)
                EndFunction
            EndProperty
            """);
        diagnostics.ShouldBeEmpty();
        var property = script.Properties.Single();
        property.Getter.ShouldNotBeNull();
        property.Setter.ShouldNotBeNull();
        property.Getter.Name.ShouldBe("Get");
        property.Setter.Parameters.Single().Name.ShouldBe("newValue");
        property.Getter.ContainingSymbol.ShouldBeSameAs(property);
    }

    [Fact]
    public void VariableSymbols()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            int counter = 0 Conditional
            """);
        diagnostics.ShouldBeEmpty();
        var variable = script.Variables.Single();
        variable.Name.ShouldBe("counter");
        variable.Type.Name.ShouldBe("int");
        variable.IsConditional.ShouldBeTrue();
        variable.Initializer.ShouldNotBeNull();
    }

    [Fact]
    public void ArrayTypes()
    {
        var (script, _) = Bind("""
            ScriptName Test
            Actor[] Property Actors Auto
            float[] Function Matrix()
                return new float[3]
            EndFunction
            """);
        var property = script.Properties.Single();
        property.Type.IsArray.ShouldBeTrue();
        property.Type.Name.ShouldBe("Actor");
        var function = script.Functions.Single();
        function.ReturnType.ShouldNotBeNull();
        function.ReturnType.IsArray.ShouldBeTrue();
        function.ReturnType.Name.ShouldBe("float");
        function.ReturnType.IsPredefined.ShouldBeTrue();
    }

    [Fact]
    public void States_WithMembers()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            Auto State Busy
                Event OnBeginState()
                EndEvent
            EndState
            State Idle
                Function Foo()
                EndFunction
            EndState
            """);
        diagnostics.ShouldBeEmpty();
        script.States.Count.ShouldBe(2);
        var busy = script.States[0];
        busy.Name.ShouldBe("Busy");
        busy.IsAuto.ShouldBeTrue();
        busy.Members.Count.ShouldBe(1);
        busy.Members[0].ShouldBeOfType<EventSymbol>();
        busy.ContainingSymbol.ShouldBeSameAs(script);
        var idle = script.States[1];
        idle.IsAuto.ShouldBeFalse();
        idle.Members[0].ShouldBeOfType<FunctionSymbol>();
        idle.Members[0].ContainingSymbol.ShouldBeSameAs(idle);
    }

    [Fact]
    public void SameFunctionNameInDifferentStates_IsAllowed()
    {
        var (_, diagnostics) = Bind("""
                                    ScriptName Test
                                    State A
                                        Function Foo()
                                        EndFunction
                                    EndState
                                    State B
                                        Function Foo()
                                        EndFunction
                                    EndState
                                    """);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void DuplicateFunctionNames_ProduceDiagnostic()
    {
        var (_, diagnostics) = Bind("""
            ScriptName Test
            Function Foo()
            EndFunction
            Function Foo()
            EndFunction
            """);
        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(DiagnosticId.DuplicateMember);
    }

    [Fact]
    public void FunctionAndPropertySameName_IsAllowed()
    {
        // Vanilla scripts (e.g. MQGreybeardAbsorbScript) declare properties and
        // functions with the same name; they live in separate namespaces.
        var (_, diagnostics) = Bind("""
            ScriptName Test
            int Property Foo Auto
            Function Foo()
            EndFunction
            """);
        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void VariableAndPropertyCollision_ProducesDiagnostic()
    {
        var (_, diagnostics) = Bind("""
            ScriptName Test
            int foo = 0
            int Property foo Auto
            """);
        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(DiagnosticId.DuplicateMember);
    }

    [Fact]
    public void FunctionAndEventCollision_ProducesDiagnostic()
    {
        var (_, diagnostics) = Bind("""
            ScriptName Test
            Function Foo()
            EndFunction
            Event Foo()
            EndEvent
            """);
        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(DiagnosticId.DuplicateMember);
    }

    [Fact]
    public void DuplicateStates_Merge()
    {
        var (script, diagnostics) = Bind("""
            ScriptName Test
            State ready
                Function A()
                EndFunction
            EndState
            State ready
                Function B()
                EndFunction
            EndState
            """);
        diagnostics.ShouldBeEmpty();
        script.States.Count.ShouldBe(1);
        script.States[0].Name.ShouldBe("ready");
        script.States[0].Members.Count.ShouldBe(2);
    }

    [Fact]
    public void DuplicateNames_CaseInsensitive()
    {
        var (_, diagnostics) = Bind("""
            ScriptName Test
            Function Foo()
            EndFunction
            Function FOO()
            EndFunction
            """);
        diagnostics.Length.ShouldBe(1);
    }

    [Fact]
    public void DuplicateParameters_ProduceDiagnostic()
    {
        var (_, diagnostics) = Bind("""
            ScriptName Test
            Function Foo(int a, bool a)
            EndFunction
            """);
        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(DiagnosticId.DuplicateParameter);
    }

    [Fact]
    public void DeclaringSyntaxReference_PointsToNameToken()
    {
        var source = """
            ScriptName Test
            Function Foo()
            EndFunction
            """;
        var tree = SyntaxTree.Parse(source);
        var binder = new ScriptBinder(tree);
        var script = binder.Bind();
        var function = script.Functions.Single();
        var reference = function.DeclaringSyntaxReference;
        reference.SyntaxTree.ShouldBeSameAs(tree);
        var text = source.Substring(reference.Span.Start, reference.Span.Length);
        text.ShouldBe("Foo");
    }

    [Fact]
    public void Imports_AreRecorded()
    {
        var (script, _) = Bind("""
            ScriptName Test
            Import Utility
            Import Game
            """);
        script.Imports.ShouldBe(["Utility", "Game"], ignoreOrder: false);
    }

    [Fact]
    public void GetterSetterNames_DoNotCollideWithScriptMembers()
    {
        var (_, diagnostics) = Bind("""
            ScriptName Test
            int Property Value Hidden
                Function Get()
                    return 5
                EndFunction
            EndProperty
            Function Get()
            EndFunction
            """);
        diagnostics.ShouldBeEmpty();
    }
}
