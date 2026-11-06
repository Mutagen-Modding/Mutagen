using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

public sealed class SemanticModelTests
{
    private static PapyrusSemanticModel CreateModel(string source) => PapyrusSemanticModel.CreateFromSource(source);

    [Fact]
    public void Body_StatementKinds()
    {
        var model = CreateModel("""
            ScriptName Test
            int counter = 0
            Function Foo(int a)
                counter = a
                if (a > 0)
                    return
                endIf
                while (counter < 5)
                    counter += 1
                endWhile
                return counter
            EndFunction
            """);
        var function = model.Script.Functions.Single();
        var body = model.GetBody(function);
        body.ShouldNotBeNull();
        body.Statements.Length.ShouldBe(4);
        body.Statements[0].ShouldBeOfType<ExpressionStatement>();
        body.Statements[1].ShouldBeOfType<IfStatement>();
        body.Statements[2].ShouldBeOfType<WhileStatement>();
        body.Statements[3].ShouldBeOfType<ReturnStatement>();
    }

    [Fact]
    public void LocalDeclaration_IsBoundWithInitializer()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo()
                int x = 5
            EndFunction
            """);
        var body = model.GetBody(model.Script.Functions.Single());
        var declaration = body!.Statements[0].ShouldBeOfType<LocalDeclarationStatement>();
        declaration.Local.Name.ShouldBe("x");
        declaration.Local.Type.Name.ShouldBe("int");
        var initializer = declaration.Initializer.ShouldBeOfType<LiteralExpression>();
        initializer.Type!.Name.ShouldBe("int");
    }

    [Fact]
    public void GetSymbolInfo_ResolvesLocalsParamsScriptVariablesAndProperties()
    {
        var model = CreateModel("""
            ScriptName Test
            int scriptVar = 0
            bool Property bFlag Auto
            Function Foo(int a)
                int x = a
                x = scriptVar
                x = bFlag as int
                x = Foo2(x)
            EndFunction
            int Function Foo2(int v)
                return v
            EndFunction
            """);
        var function = model.Script.Functions.Single(f => f.Name == "Foo");
        _ = model.GetBody(function)!;
        var root = model.Script.SyntaxTree;

        // Find identifier nodes by walking the syntax tree
        var identifiers = root.Root.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Where(i => i.Parent is not MemberAccessExpressionSyntax)
            .ToList();

        var byText = identifiers.GroupBy(i => i.Identifier.Text).ToDictionary(g => g.Key, g => g.First());
        model.GetSymbolInfo(byText["a"]).ShouldBeOfType<ParameterSymbol>();
        model.GetSymbolInfo(byText["x"]).ShouldBeOfType<VariableSymbol>();
        model.GetSymbolInfo(byText["scriptVar"]).ShouldBeOfType<VariableSymbol>();
        model.GetSymbolInfo(byText["bFlag"]).ShouldBeOfType<PropertySymbol>();
        model.GetSymbolInfo(byText["Foo2"]).ShouldBeOfType<FunctionSymbol>();
    }

    [Fact]
    public void GetSymbolInfo_SelfResolvesToScript()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo()
                MoveTo(Self)
            EndFunction
            Function MoveTo(ObjectReference o)
            EndFunction
            """);
        var root = model.Script.SyntaxTree;
        var selfNode = root.Root.DescendantNodes()
            .OfType<LiteralExpressionSyntax>()
            .First(i => i.Token.Kind == SyntaxKind.SelfKeyword);
        model.GetSymbolInfo(selfNode).ShouldBeSameAs(model.Script);
    }

    [Fact]
    public void UnqualifiedCall_ResolvesToScriptFunction()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo()
                Bar(5)
            EndFunction
            Function Bar(int v)
            EndFunction
            """);
        var root = model.Script.SyntaxTree;
        var invocation = root.Root.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
        var call = model.GetExpression(invocation).ShouldBeOfType<CallExpression>();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Bar");
        call.Receiver.ShouldBeNull();
        call.Arguments.Length.ShouldBe(1);
    }

    [Fact]
    public void SelfCall_ResolvesWithReceiver()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo()
                self.Bar()
            EndFunction
            Function Bar()
            EndFunction
            """);
        var root = model.Script.SyntaxTree;
        var invocation = root.Root.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
        var call = model.GetExpression(invocation).ShouldBeOfType<CallExpression>();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Bar");
        call.Receiver.ShouldBeOfType<SelfExpression>();
    }

    [Fact]
    public void ForeignMemberCall_IsUnresolvedButStructured()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo(Actor akActor)
                akActor.MoveTo(Self)
            EndFunction
            """);
        var root = model.Script.SyntaxTree;
        var invocation = root.Root.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
        var call = model.GetExpression(invocation).ShouldBeOfType<CallExpression>();
        call.Function.ShouldBeNull(); // Cross-script resolution is a later phase
        call.Receiver.ShouldNotBeNull();
        call.Receiver.Type!.Name.ShouldBe("Actor");
        call.Arguments.Length.ShouldBe(1);
    }

    [Fact]
    public void GetTypeInfo_LiteralsAndOperators()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo(int i, float f, string s, bool b, int[] arr)
                int a = i + i
                float c = i + f
                bool d = i > 0
                bool e = !b
                int g = arr.Length
                int h = arr[0]
                float j = f as float
                int[] k = new int[5]
            EndFunction
            """);
        var root = model.Script.SyntaxTree;
        var declarations = root.Root.DescendantNodes().OfType<LocalDeclarationStatementSyntax>()
            .ToDictionary(d => d.Identifier.Text);

        model.GetTypeInfo(declarations["a"].Initializer!.Value)!.Name.ShouldBe("int");
        model.GetTypeInfo(declarations["c"].Initializer!.Value)!.Name.ShouldBe("float");
        model.GetTypeInfo(declarations["d"].Initializer!.Value)!.Name.ShouldBe("bool");
        model.GetTypeInfo(declarations["e"].Initializer!.Value)!.Name.ShouldBe("bool");
        model.GetTypeInfo(declarations["g"].Initializer!.Value)!.Name.ShouldBe("int");
        model.GetTypeInfo(declarations["h"].Initializer!.Value)!.Name.ShouldBe("int");
        model.GetTypeInfo(declarations["j"].Initializer!.Value)!.Name.ShouldBe("float");
        model.GetTypeInfo(declarations["k"].Initializer!.Value)!.IsArray.ShouldBeTrue();
    }

    [Fact]
    public void EventBodies_AreBound()
    {
        var model = CreateModel("""
            ScriptName Test
            Event OnLoad()
                int x = 0
                Foo()
            EndEvent
            Function Foo()
            EndFunction
            """);
        var @event = model.Script.Events.Single();
        var body = model.GetBody(@event);
        body.ShouldNotBeNull();
        body.Statements.Length.ShouldBe(2);
    }

    [Fact]
    public void PropertyAccessorBodies_AreBound()
    {
        var model = CreateModel("""
            ScriptName Test
            int Property Value Hidden
                Function Get()
                    return 5
                EndFunction
            EndProperty
            """);
        var property = model.Script.Properties.Single();
        var getter = (FunctionSymbol)property.Getter!;
        var body = model.GetBody(getter);
        body.ShouldNotBeNull();
        body.Statements.Single().ShouldBeOfType<ReturnStatement>();
    }

    [Fact]
    public void StateFunctionCall_ResolvesStateFunctionFirst()
    {
        var model = CreateModel("""
            ScriptName Test
            State Busy
                Function Foo()
                    Work()
                EndFunction
                Function Work()
                EndFunction
            EndState
            Function Work()
            EndFunction
            """);
        var state = model.Script.States.Single();
        _ = state.Members.OfType<FunctionSymbol>().First(f => f.Name == "Foo");
        var root = model.Script.SyntaxTree;
        var invocation = root.Root.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
        var call = model.GetExpression(invocation).ShouldBeOfType<CallExpression>();
        call.Function.ShouldNotBeNull();
        call.Function.ContainingSymbol.ShouldBeSameAs(state);
    }

    [Fact]
    public void DuplicateLocal_InFunctionBody_ProducesDiagnostic()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo()
                int x = 0
                int x = 1
            EndFunction
            """);
        model.GetDiagnostics().ShouldContain(d => d.Id == DiagnosticId.DuplicateLocal);
    }

    [Fact]
    public void DuplicateLocal_InEventBody_ProducesDiagnostic()
    {
        var model = CreateModel("""
            ScriptName Test
            Event OnLoad()
                int x = 0
                int x = 1
            EndEvent
            """);
        model.GetDiagnostics().ShouldContain(d => d.Id == DiagnosticId.DuplicateLocal);
    }

    [Fact]
    public void ParameterAndLocalCollision_ProducesDiagnostic()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo(int a)
                int a = 5
            EndFunction
            """);
        model.GetDiagnostics().ShouldContain(d => d.Id == DiagnosticId.DuplicateLocal);
    }

    [Fact]
    public void NestedScopeShadowing_IsAllowed()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo()
                int x = 0
                if (x)
                    int x = 1
                endIf
            EndFunction
            """);
        model.GetDiagnostics().ShouldNotContain(d => d.Id == DiagnosticId.DuplicateLocal);
    }

    [Fact]
    public void FunctionLocals_AreCollectedInDeclarationOrder()
    {
        var model = CreateModel("""
            ScriptName Test
            Function Foo(int a)
                int x = 0
                if (x)
                    string y = "nested"
                endIf
            EndFunction
            """);
        var function = model.Script.Functions.Single();
        model.GetBody(function).ShouldNotBeNull();
        function.Locals.Count.ShouldBe(2);
        function.Locals[0].Name.ShouldBe("x");
        function.Locals[1].Name.ShouldBe("y");
    }

    [Fact]
    public void Diagnostics_IncludeParseDiagnostics()
    {
        var model = CreateModel("""
            ScriptName Test
            @@@
            """);
        model.GetDiagnostics().ShouldContain(d => d.Id == DiagnosticId.UnexpectedToken);
    }
}
