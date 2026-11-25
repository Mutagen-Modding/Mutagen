using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>One function execution in the expanded sequence of a parent call.</summary>
/// <param name="Function">The function whose body executes.</param>
/// <param name="ExecutedAs">
/// The script the function executes "as", which decides what <c>parent</c> means
/// inside the body — not necessarily the script that declares the function.
/// </param>
public sealed record ParentCallStep(IFunctionSymbol Function, IScriptSymbol ExecutedAs);

/// <summary>
/// Semantics of the <c>parent</c> keyword. A parent call executes the nearest
/// ancestor's function "as" the direct parent — and when intermediate scripts do
/// not declare the function, the same function can execute multiple times on the
/// way up the hierarchy, because each execution's own parent calls resolve from
/// the script it executes "as", not from the script that declares it.
/// </summary>
public static class ParentCallExtensions
{
    extension(CallExpression call)
    {
        /// <summary>True when this call was made through the <c>parent</c> keyword.</summary>
        public bool IsParentCall => call.Receiver is ScriptExpression { IsParent: true };

        /// <summary>
        /// Expands a parent call into the effective sequence of function executions,
        /// following the re-dispatch quirk. Requires a codebase to bind bodies
        /// declared in other scripts; without one, only the direct step is returned.
        /// Returns an empty list for non-parent or unresolved calls.
        /// </summary>
        public IReadOnlyList<ParentCallStep> ExpandParentCall(PapyrusSemanticModel model)
        {
            if (!call.IsParentCall
                || call.Function is not { } function
                || call.Receiver is not ScriptExpression { Script: ScriptSymbol executedAs })
            {
                return [];
            }
            var steps = new List<ParentCallStep>();
            Expand(model, function, executedAs, steps);
            return steps;
        }
    }

    private static void Expand(PapyrusSemanticModel callerModel, IFunctionSymbol function, ScriptSymbol executedAs, List<ParentCallStep> steps)
    {
        steps.Add(new ParentCallStep(function, executedAs));

        var body = BindBody(callerModel, function);
        if (body == null)
        {
            return;
        }

        // Each parent call inside the executed body resolves from the script the
        // body executes "as": parent is that script's direct parent, the function
        // is the nearest ancestor declaring the name, and it executes "as" that
        // direct parent. ExecutedAs strictly ascends, so this terminates.
        foreach (var inner in body.DescendantNodes().OfType<CallExpression>())
        {
            if (!inner.IsParentCall || inner.Name is not { } name)
            {
                continue;
            }
            var start = executedAs.BaseScript as ScriptSymbol;
            if (start == null)
            {
                continue;
            }
            if (ResolveInChain(start, name) is { } target)
            {
                Expand(callerModel, target, start, steps);
            }
        }
    }

    private static FunctionBody? BindBody(PapyrusSemanticModel callerModel, IFunctionSymbol function)
    {
        var declaringScript = DeclaringScript(function);
        if (declaringScript == null)
        {
            return null;
        }
        var declaringModel = declaringScript == callerModel.Script
            ? callerModel
            : callerModel.Codebase?.GetModel(declaringScript.Name);
        if (declaringModel == null)
        {
            return null;
        }
        return function switch
        {
            FunctionSymbol f => declaringModel.GetBody(f),
            EventSymbol e => declaringModel.GetBody(e),
            _ => null,
        };
    }

    private static ScriptSymbol? DeclaringScript(ISymbol symbol)
    {
        for (var current = symbol.ContainingSymbol; current != null; current = current.ContainingSymbol)
        {
            if (current is ScriptSymbol script)
            {
                return script;
            }
        }
        return null;
    }

    private static IFunctionSymbol? ResolveInChain(IScriptSymbol start, string name)
    {
        for (var current = start as ScriptSymbol; current != null; current = current.BaseScript as ScriptSymbol)
        {
            foreach (var function in current.Functions)
            {
                if (string.Equals(function.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return function;
                }
            }
            foreach (var @event in current.Events)
            {
                if (string.Equals(@event.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return @event;
                }
            }
        }
        return null;
    }
}
