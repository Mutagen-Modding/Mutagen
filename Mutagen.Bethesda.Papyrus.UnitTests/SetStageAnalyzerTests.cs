using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// Demonstrates an analyzer built on the semantic code tree: find every SetStage
/// call made on a quest property or variable and extract the (quest, stage) pair.
/// The analyzer walks bound nodes with a BodyWalker — no syntax tree, no lookups —
/// and targets a single function by name.
/// </summary>
public sealed class SetStageAnalyzerTests
{
    private sealed record SetStageCall(string QuestName, ISymbol? QuestSymbol, object? Stage);

    private sealed class SetStageWalker : BodyWalker
    {
        public List<SetStageCall> Calls { get; } = [];

        protected override void VisitCall(CallExpression node)
        {
            if (string.Equals(node.Name, "SetStage", StringComparison.OrdinalIgnoreCase)
                && node.Receiver?.Type is { } receiverType
                && receiverType.Name.Equals("Quest", StringComparison.OrdinalIgnoreCase))
            {
                var questSymbol = node.Receiver switch
                {
                    VariableExpression variable => variable.Variable,
                    MemberExpression member => member.Member,
                    _ => null,
                };
                object? stage = null;
                if (node.Arguments.Length > 0)
                {
                    stage = node.Arguments[0] switch
                    {
                        LiteralExpression literal => literal.ConstantValue.Value,
                        VariableExpression variable => variable.Variable.Name,
                        _ => null, // computed stage, not a compile-time-known value
                    };
                }
                Calls.Add(new SetStageCall(questSymbol?.Name ?? "", questSymbol, stage));
            }

            base.VisitCall(node); // descend into arguments
        }
    }

    private static PapyrusSemanticModel CreateModel(string source) =>
        PapyrusSemanticModel.CreateFromSource(source);

    [Fact]
    public void Extracts_QuestAndStage_FromTargetedFunction()
    {
        var model = CreateModel("""
            ScriptName Test

            Quest Property MainQuest Auto
            Quest Property SideQuest Auto
            Quest GlobalQuestVar

            int stage = 5

            Event OnInit()
                MainQuest.SetStage(10)
                SideQuest.SetStage(20)
                GlobalQuestVar.SetStage(stage)
                int localStage = 30
                MainQuest.SetStage(localStage)
                MainQuest.SetStage(stage + 1)
            EndEvent

            Function Advance()
                MainQuest.SetStage(40)
            EndFunction
            """);

        // Target one member by name; other functions are not visited.
        var onInit = model.GetBody("OnInit");
        onInit.ShouldNotBeNull();

        var walker = new SetStageWalker();
        walker.Visit(onInit);

        walker.Calls.Count.ShouldBe(5);
        walker.Calls[0].QuestName.ShouldBe("MainQuest");
        walker.Calls[0].QuestSymbol.ShouldBeOfType<PropertySymbol>();
        walker.Calls[0].Stage.ShouldBe(10L);
        walker.Calls[1].QuestName.ShouldBe("SideQuest");
        walker.Calls[1].Stage.ShouldBe(20L);
        walker.Calls[2].QuestName.ShouldBe("GlobalQuestVar");
        walker.Calls[2].QuestSymbol.ShouldBeOfType<VariableSymbol>();
        walker.Calls[2].Stage.ShouldBe("stage");
        walker.Calls[3].QuestName.ShouldBe("MainQuest");
        walker.Calls[3].Stage.ShouldBe("localStage");

        // stage + 1 is computed at runtime; the call is still found, the stage just
        // has no compile-time value.
        walker.Calls[4].QuestName.ShouldBe("MainQuest");
        walker.Calls[4].Stage.ShouldBeNull();

        // A different target yields only that function's calls.
        var advanceWalker = new SetStageWalker();
        advanceWalker.Visit(model.GetBody("Advance"));
        advanceWalker.Calls.Count.ShouldBe(1);
        advanceWalker.Calls[0].QuestName.ShouldBe("MainQuest");
        advanceWalker.Calls[0].Stage.ShouldBe(40L);

        model.GetBody("DoesNotExist").ShouldBeNull();
    }

    [Fact]
    public void Ignores_NonSetStageCalls_AndNonQuestReceivers()
    {
        var model = CreateModel("""
            ScriptName Test
            ObjectReference Property Marker Auto
            Event OnInit()
                Marker.MoveTo(Self)
                Foo(5)
            EndEvent
            Function Foo(int v)
            EndFunction
            """);

        var walker = new SetStageWalker();
        foreach (var body in model.GetBodies())
        {
            walker.Visit(body);
        }
        walker.Calls.Count.ShouldBe(0);
    }

    [Fact]
    public void ActionBodyWalker_HandlesMultipleNodeKinds()
    {
        var model = CreateModel("""
            ScriptName Test
            Quest Property MainQuest Auto
            Event OnInit()
                MainQuest.SetStage(10)
                int x = 5
                if (x > 0)
                    MainQuest.SetStage(20)
                endIf
            EndEvent
            """);

        // Supplying an action for a node kind means you own that subtree: the @if
        // action prunes the if's children, so only the top-level call is seen.
        var prunedCalls = new List<object?>();
        var locals = new List<string>();
        var ifs = 0;
        var pruned = new FuncBodyWalker(
            call: c => prunedCalls.Add(c.Name),
            localDeclaration: l => locals.Add(l.Local.Name),
            @if: _ => ifs++);
        pruned.Visit(model.GetBody("OnInit"));
        prunedCalls.ShouldBe(["SetStage"]);
        locals.ShouldBe(["x"]);
        ifs.ShouldBe(1);

        // Without an @if action, the walker descends into the if and finds the
        // nested call too.
        var allCalls = new List<object?>();
        var full = new FuncBodyWalker(call: c => allCalls.Add(c.Name));
        full.Visit(model.GetBody("OnInit"));
        allCalls.ShouldBe(["SetStage", "SetStage"]);
    }
}
