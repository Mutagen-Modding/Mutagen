using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Unqualified names resolve through the base chain: a script can use members
/// declared anywhere in its inheritance hierarchy, e.g. BatteringTrap using the
/// WindupSound property declared on TrapBase two levels up.
/// </summary>
public sealed class InheritedMemberTests
{
    private static string DeclaringScriptName(ISymbol symbol)
    {
        for (var current = symbol.ContainingSymbol; current != null; current = current.ContainingSymbol)
        {
            if (current is ScriptSymbol script) return script.Name;
        }
        return "";
    }

    [Fact]
    public void UnqualifiedProperty_ResolvesThroughDistantBase()
    {
        // Real corpus case: BatteringTrap -> MovingTrap -> TrapBase, where
        // TrapBase declares the WindupSound property.
        var compilation = new PapyrusCodebase(
        [
            SyntaxTree.Parse("ScriptName ObjectReference"),
            SyntaxTree.Parse("""
                ScriptName Sound
                Function Play(ObjectReference source)
                EndFunction
                """),
            SyntaxTree.Parse("""
                ScriptName TrapBase extends ObjectReference
                sound property WindupSound auto
                """),
            SyntaxTree.Parse("ScriptName MovingTrap extends TrapBase"),
            SyntaxTree.Parse("""
                scriptName BatteringTrap extends MovingTrap
                Event OnLoad()
                    WindupSound.play( self as ObjectReference)
                EndEvent
                """),
        ]);
        var model = compilation.GetModel("BatteringTrap")!;

        var call = model.GetBody("OnLoad")!.DescendantNodes().OfType<CallExpression>().Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("Play");
        DeclaringScriptName(call.Function).ShouldBe("Sound");

        var receiver = call.Receiver.ShouldBeOfType<VariableExpression>();
        receiver.Variable.ShouldBeOfType<PropertySymbol>();
        receiver.Variable.Name.ShouldBe("WindupSound");
        DeclaringScriptName(receiver.Variable).ShouldBe("TrapBase");
        receiver.Type!.Name.ShouldBe("sound");
    }

    [Fact]
    public void UnqualifiedVariable_ResolvesThroughBase()
    {
        var compilation = new PapyrusCodebase(
        [
            SyntaxTree.Parse("""
                ScriptName Base
                int counter = 0
                """),
            SyntaxTree.Parse("""
                ScriptName Child extends Base
                Function Foo()
                    counter += 1
                EndFunction
                """),
        ]);
        var model = compilation.GetModel("Child")!;

        var statement = model.GetBody("Foo")!.Statements[0].ShouldBeOfType<ExpressionStatement>();
        var target = statement.Expression.ShouldBeOfType<AssignmentExpression>().Target;
        var variable = target.ShouldBeOfType<VariableExpression>();

        variable.Variable.Name.ShouldBe("counter");
        DeclaringScriptName(variable.Variable).ShouldBe("Base");
    }

    [Fact]
    public void OwnMember_BeatsInheritedMember()
    {
        var compilation = new PapyrusCodebase(
        [
            SyntaxTree.Parse("""
                ScriptName Base
                int value = 1
                """),
            SyntaxTree.Parse("""
                ScriptName Child extends Base
                int value = 2
                Function Foo()
                    value += 1
                EndFunction
                """),
        ]);
        var model = compilation.GetModel("Child")!;

        var statement = model.GetBody("Foo")!.Statements[0].ShouldBeOfType<ExpressionStatement>();
        var target = statement.Expression.ShouldBeOfType<AssignmentExpression>().Target;
        var variable = target.ShouldBeOfType<VariableExpression>();

        DeclaringScriptName(variable.Variable).ShouldBe("Child");
    }

    [Fact]
    public void NearestAncestor_WinsOverDistantOne()
    {
        var compilation = new PapyrusCodebase(
        [
            SyntaxTree.Parse("""
                ScriptName GrandBase
                int value = 1
                """),
            SyntaxTree.Parse("""
                ScriptName Middle extends GrandBase
                int value = 2
                """),
            SyntaxTree.Parse("""
                ScriptName Leaf extends Middle
                Function Foo()
                    value += 1
                EndFunction
                """),
        ]);
        var model = compilation.GetModel("Leaf")!;

        var statement = model.GetBody("Foo")!.Statements[0].ShouldBeOfType<ExpressionStatement>();
        var target = statement.Expression.ShouldBeOfType<AssignmentExpression>().Target;
        var variable = target.ShouldBeOfType<VariableExpression>();

        DeclaringScriptName(variable.Variable).ShouldBe("Middle");
    }
}
