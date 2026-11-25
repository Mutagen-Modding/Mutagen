using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// Builds the symbol table of a single script from its syntax tree: script members,
/// states, and parameters, reporting duplicate declarations. Binding the executable
/// code inside function and event bodies is the job of the semantic model's
/// BodyBinder, which layers on top of the declaration tables built here.
/// </summary>
public sealed class ScriptBinder(SyntaxTree tree)
{
    private readonly List<PapyrusDiagnostic> _diagnostics = [];

    public IReadOnlyList<PapyrusDiagnostic> Diagnostics => _diagnostics;

    public ScriptSymbol Bind()
    {
        var root = tree.Root;
        var header = root.Header;
        var script = new ScriptSymbol(
            header.Name.Text,
            BaseScriptName(header.ExtendsClause),
            header.Flags.Any(f => f.Kind == SyntaxKind.HiddenKeyword),
            header.Flags.Any(f => f.Kind == SyntaxKind.ConditionalKeyword),
            tree,
            new DeclaringSyntaxReference(tree, header.Name.Span));

        // Papyrus has two member namespaces per container: functions/events share one,
        // variables/properties share the other (a property is backed by a hidden
        // variable). Names may repeat across the two namespaces. States may be
        // declared in multiple blocks, which merge into one state.
        var functionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var variableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var statesByName = new Dictionary<string, StateSymbol>(StringComparer.OrdinalIgnoreCase);

        foreach (var member in root.Members)
        {
            switch (member)
            {
                case VariableDeclarationSyntax variable:
                    BindVariable(variable, script, variableNames);
                    break;
                case PropertyDeclarationSyntax property:
                    BindProperty(property, script, variableNames);
                    break;
                case FunctionDeclarationSyntax function:
                    script.AddMember(BindFunction(function, script, functionNames, containingState: null));
                    break;
                case EventDeclarationSyntax @event:
                    script.AddMember(BindEvent(@event, script, functionNames, containingState: null));
                    break;
                case StateDeclarationSyntax state:
                    BindState(state, script, statesByName);
                    break;
                case ImportDirectiveSyntax import:
                    script.AddImport(import.ScriptName.Text);
                    break;
            }
        }

        return script;
    }

    private static string? BaseScriptName(ExtendsClauseSyntax? extendsClause) =>
        extendsClause?.BaseType is NamedTypeSyntax named ? named.Identifier.Text : null;

    private void BindVariable(VariableDeclarationSyntax variable, ScriptSymbol script, HashSet<string> names)
    {
        AddMemberName(names, variable.Identifier);
        script.AddMember(new VariableSymbol(
            variable.Identifier.Text,
            TypeSymbol.FromSyntax(variable.Type),
            isConditional: variable.Flags.Any(f => f.Kind == SyntaxKind.ConditionalKeyword),
            variable.Initializer?.Value,
            script,
            new DeclaringSyntaxReference(tree, variable.Identifier.Span)));
    }

    private void BindProperty(PropertyDeclarationSyntax property, ScriptSymbol script, HashSet<string> names)
    {
        AddMemberName(names, property.Identifier);
        var propertySymbol = new PropertySymbol(
            property.Identifier.Text,
            TypeSymbol.FromSyntax(property.Type),
            isAuto: property.Flags.Any(f => f.Kind == SyntaxKind.AutoKeyword),
            isAutoReadOnly: property.Flags.Any(f => f.Kind == SyntaxKind.AutoReadOnlyKeyword),
            isHidden: property.Flags.Any(f => f.Kind == SyntaxKind.HiddenKeyword),
            isConditional: property.Flags.Any(f => f.Kind == SyntaxKind.ConditionalKeyword),
            property.Initializer?.Value,
            getter: null,
            setter: null,
            script,
            new DeclaringSyntaxReference(tree, property.Identifier.Span));

        // Getter/setter function names are scoped to the property, not to the script,
        // so they must not occupy script member names.
        if (property.Getter != null || property.Setter != null)
        {
            var accessorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var getter = property.Getter != null ? BindFunction(property.Getter, propertySymbol, accessorNames, containingState: null) : null;
            var setter = property.Setter != null ? BindFunction(property.Setter, propertySymbol, accessorNames, containingState: null) : null;
            propertySymbol.SetAccessors(getter, setter);
        }

        script.AddMember(propertySymbol);
    }

    private FunctionSymbol BindFunction(
        FunctionDeclarationSyntax function,
        ISymbol containingSymbol,
        HashSet<string> names,
        StateSymbol? containingState)
    {
        AddMemberName(names, function.Identifier);
        var parameters = BindParameters(function.ParameterList);
        var symbol = new FunctionSymbol(
            function.Identifier.Text,
            function.ReturnType != null ? TypeSymbol.FromSyntax(function.ReturnType) : null,
            parameters,
            isNative: function.Flags.Any(f => f.Kind == SyntaxKind.NativeKeyword),
            isGlobal: function.Flags.Any(f => f.Kind == SyntaxKind.GlobalKeyword),
            containingState ?? containingSymbol,
            new DeclaringSyntaxReference(tree, function.Identifier.Span));

        return symbol;
    }

    private EventSymbol BindEvent(
        EventDeclarationSyntax @event,
        ISymbol containingSymbol,
        HashSet<string> names,
        StateSymbol? containingState)
    {
        AddMemberName(names, @event.Identifier);
        // Events have no body in the symbol world; their statements belong to the
        // bound-node layer.
        return new EventSymbol(
            @event.Identifier.Text,
            BindParameters(@event.ParameterList),
            isNative: @event.Flags.Any(f => f.Kind == SyntaxKind.NativeKeyword),
            containingState ?? containingSymbol,
            new DeclaringSyntaxReference(tree, @event.Identifier.Span));
    }

    private void BindState(StateDeclarationSyntax state, ScriptSymbol script, Dictionary<string, StateSymbol> statesByName)
    {
        if (!statesByName.TryGetValue(state.Identifier.Text, out var stateSymbol))
        {
            stateSymbol = new StateSymbol(
                state.Identifier.Text,
                isAuto: state.AutoKeyword != null,
                script,
                new DeclaringSyntaxReference(tree, state.Identifier.Span));
            statesByName[state.Identifier.Text] = stateSymbol;
            script.AddState(stateSymbol);
        }

        var stateMemberNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in state.Members)
        {
            switch (member)
            {
                case FunctionDeclarationSyntax function:
                    stateSymbol.AddMember(BindFunction(function, stateSymbol, stateMemberNames, containingState: stateSymbol));
                    break;
                case EventDeclarationSyntax @event:
                    stateSymbol.AddMember(BindEvent(@event, stateSymbol, stateMemberNames, containingState: stateSymbol));
                    break;
            }
        }
    }

    private IReadOnlyList<ParameterSymbol> BindParameters(ParameterListSyntax parameterList)
    {
        var parameters = new List<ParameterSymbol>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var parameter in parameterList.Parameters)
        {
            if (!names.Add(parameter.Identifier.Text))
            {
                _diagnostics.Add(new PapyrusDiagnostic(
                    DiagnosticId.DuplicateParameter,
                    $"Duplicate parameter '{parameter.Identifier.Text}'.",
                    parameter.Identifier.Span));
            }
            parameters.Add(new ParameterSymbol(
                parameter.Identifier.Text,
                TypeSymbol.FromSyntax(parameter.Type),
                parameter.Default?.Value,
                new DeclaringSyntaxReference(tree, parameter.Identifier.Span)));
        }
        return parameters;
    }

    private void AddMemberName(HashSet<string> names, SyntaxToken identifier)
    {
        if (!names.Add(identifier.Text))
        {
            _diagnostics.Add(new PapyrusDiagnostic(
                DiagnosticId.DuplicateMember,
                $"Duplicate member '{identifier.Text}'.",
                identifier.Span));
        }
    }
}
