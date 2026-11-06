using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// Convenience extensions that bring model-scoped queries directly onto syntax
/// nodes, symbols, and code nodes. Deeply nested nodes carry no reference to their
/// model, so the model is passed in as an argument — this keeps global context
/// available with intellisense right on the node.
/// </summary>
public static class PapyrusSemanticExtensions
{
    extension(ExpressionSyntax expression)
    {
        /// <summary>The symbol this expression refers to, or null when unresolved.</summary>
        public ISymbol? GetSymbolInfo(PapyrusSemanticModel model) => model.GetSymbolInfo(expression);

        /// <summary>The type of this expression's value, or null when unknown.</summary>
        public TypeSymbol? GetTypeInfo(PapyrusSemanticModel model) => model.GetTypeInfo(expression);

        /// <summary>The bound code node for this expression, or null when outside a body.</summary>
        public PapyrusExpression? GetExpression(PapyrusSemanticModel model) => model.GetExpression(expression);
    }

    extension(FunctionDeclarationSyntax function)
    {
        /// <summary>The symbol declared by this function declaration, or null when foreign to the model.</summary>
        public FunctionSymbol? GetSymbol(PapyrusSemanticModel model) => model.FindFunctionSymbol(function);

        /// <summary>The bound body of this function declaration, or null when it has none.</summary>
        public FunctionBody? GetBody(PapyrusSemanticModel model) =>
            model.FindFunctionSymbol(function) is { } symbol ? model.GetBody(symbol) : null;
    }

    extension(EventDeclarationSyntax @event)
    {
        /// <summary>The symbol declared by this event declaration, or null when foreign to the model.</summary>
        public EventSymbol? GetSymbol(PapyrusSemanticModel model) => model.FindEventSymbol(@event);

        /// <summary>The bound body of this event declaration, or null when it has none.</summary>
        public FunctionBody? GetBody(PapyrusSemanticModel model) =>
            model.FindEventSymbol(@event) is { } symbol ? model.GetBody(symbol) : null;
    }

    extension(FunctionSymbol function)
    {
        /// <summary>The bound body of this function, or null when it has none (e.g. native functions).</summary>
        public FunctionBody? GetBody(PapyrusSemanticModel model) => model.GetBody(function);
    }

    extension(EventSymbol @event)
    {
        /// <summary>The bound body of this event, or null when it has none.</summary>
        public FunctionBody? GetBody(PapyrusSemanticModel model) => model.GetBody(@event);
    }

    extension(PapyrusNode node)
    {
        /// <summary>
        /// The function or event whose body contains this code node, found by walking
        /// the node's syntax ancestors; null for nodes outside any body.
        /// </summary>
        public ISymbol? GetContainingMember(PapyrusSemanticModel model)
        {
            for (var current = node.Syntax; current != null; current = current.Parent)
            {
                switch (current)
                {
                    case FunctionDeclarationSyntax function:
                        return model.FindFunctionSymbol(function);
                    case EventDeclarationSyntax @event:
                        return model.FindEventSymbol(@event);
                }
            }
            return null;
        }

        /// <summary>The body containing this code node, or null for nodes outside any body.</summary>
        public FunctionBody? GetContainingBody(PapyrusSemanticModel model) =>
            node.GetContainingMember(model) switch
            {
                FunctionSymbol function => model.GetBody(function),
                EventSymbol @event => model.GetBody(@event),
                _ => null,
            };
    }

    extension(IScriptSymbol script)
    {
        /// <summary>The model of this script within a compilation, or null when not part of it.</summary>
        public PapyrusSemanticModel? GetModel(PapyrusCodebase compilation) =>
            compilation.GetModel(script.Name);
    }
}
