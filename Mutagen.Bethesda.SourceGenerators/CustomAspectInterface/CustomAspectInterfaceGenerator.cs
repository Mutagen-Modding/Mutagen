using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.SourceGenerators.CustomAspectInterface;

public record InterfaceDeclaration(
    InterfaceDeclarationSyntax Interface,
    TypeOfExpressionSyntax[] Types);

public record TypeDeclaration(
    TypeOfExpressionSyntax Syntax,
    ITypeSymbol? Symbol);

public record InterfaceSymbolDeclaration(
    InterfaceDeclarationSyntax Interface,
    ITypeSymbol Symbol,
    TypeDeclaration[] Types);

public record InterfaceParameter(
    InterfaceSymbolDeclaration Declaration,
    ITypeSymbol ParameterType,
    string Name);

public record GenerationTarget(
    string Namespace,
    IEnumerable<InterfaceSymbolDeclaration> UsedInterfaces,
    Dictionary<MethodDeclarationSyntax, List<InterfaceParameter>> Declarations);

public record MethodUsageInformation(
    MethodDeclarationSyntax Syntax,
    ISymbol MethodSymbol,
    List<InterfaceParameter> InterfaceUsages);

[Generator]
public class CustomAspectInterfaceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new CustomAspectInterfaceReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.CancellationToken.IsCancellationRequested) return;
        if (context.SyntaxReceiver is not CustomAspectInterfaceReceiver customAspectReceiver) return;

        try
        {
            if (context.CancellationToken.IsCancellationRequested) return;
            var interfaceSymbols = GetInterfaceTypeDictionary(context, customAspectReceiver);

            // Group things by namespace
            if (context.CancellationToken.IsCancellationRequested) return;
            var targets = new Dictionary<string, GenerationTarget>();

            foreach (var namespaceGroup in interfaceSymbols.Values
                         .GroupBy(x => x.Symbol.ContainingNamespace))
            {
                targets[namespaceGroup.Key.Name] = new GenerationTarget(
                    namespaceGroup.Key.Name,
                    namespaceGroup,
                    new());
            }

            HashSet<string> requiredUsings = new(targets.SelectMany(x => x.Value.UsedInterfaces)
                .SelectMany(x => x.Types)
                .Select(i => i.Symbol)
                .NotNull()
                .Select(x => x.ContainingNamespace.ToString()));

            // Generate
            foreach (var namespaceGroup in targets)
            {
                if (context.CancellationToken.IsCancellationRequested) return;
                var sb = new StructuredStringBuilder();

                foreach (var use in requiredUsings)
                {
                    sb.AppendLine($"using {use};");
                }
                sb.AppendLine();

                using (sb.Namespace(namespaceGroup.Key, fileScoped: false))
                {
                    using (sb.Region("Wrappers"))
                    {
                        foreach (var decl in namespaceGroup.Value.UsedInterfaces)
                        {
                            if (context.CancellationToken.IsCancellationRequested) return;
                            foreach (var t in decl.Types)
                            {
                                var className = $"{t.Syntax.Type}Wrapper";
                                using (var c = sb.Class(className))
                                {
                                    c.Interfaces.Add(decl.Symbol.Name);
                                }
                                using (sb.CurlyBrace())
                                {
                                    sb.AppendLine($"private readonly {t.Syntax.Type} _wrapped;");
                                    foreach (var member in decl.Symbol.GetMembers())
                                    {
                                        switch (member)
                                        {
                                            case IPropertySymbol prop:
                                                sb.AppendLine($"public {prop.Type} {prop.Name}");
                                                using (sb.CurlyBrace())
                                                {
                                                    if (!prop.IsWriteOnly)
                                                    {
                                                        sb.AppendLine($"get => _wrapped.{member.Name};");
                                                    }
                                                    if (!prop.IsReadOnly)
                                                    {
                                                        sb.AppendLine($"set => _wrapped.{member.Name} = value;");
                                                    }
                                                }
                                                break;
                                            default:
                                                continue;
                                        }
                                        sb.AppendLine();
                                    }

                                    using (var args = sb.Function($"public {className}"))
                                    {
                                        args.Add($"{t.Syntax.Type} rhs");
                                    }
                                    using (sb.CurlyBrace())
                                    {
                                        sb.AppendLine("_wrapped = rhs;");
                                    }
                                }
                                sb.AppendLine();
                            }
                        }
                        sb.AppendLine();
                    }

                    if (context.CancellationToken.IsCancellationRequested) return;
                    using (sb.Region("Mix Ins"))
                    {
                        using (var c = sb.Class("WrapperMixIns"))
                        {
                            c.Static = true;
                        }
                        using (sb.CurlyBrace())
                        {
                            foreach (var decl in namespaceGroup.Value.UsedInterfaces)
                            {
                                if (context.CancellationToken.IsCancellationRequested) return;
                                foreach (var t in decl.Types)
                                {
                                    using (var args = sb.Function(
                                               $"public static {t.Syntax.Type}Wrapper As{decl.Symbol.Name}"))
                                    {
                                        args.Add($"this {t.Syntax.Type} rhs");
                                    }
                                    using (sb.CurlyBrace())
                                    {
                                        sb.AppendLine($"return new {t.Syntax.Type}Wrapper(rhs);");
                                    }
                                    sb.AppendLine();
                                }
                            }
                        }
                    }
                }
                context.AddSource($"CustomAspectInterfaces.g.cs", sb.ToString());
            }
        }
        catch (Exception ex)
        {
            customAspectReceiver.Exceptions.Add(ex);
        }

        if (customAspectReceiver.Exceptions.Count > 0)
        {
            context.AddSource($"Errors.g.cs", $"Number of exceptions: {customAspectReceiver.Exceptions.Count}\n" +
                                              $"{customAspectReceiver.Exceptions.FirstOrDefault()}");
        }
    }

    private static Dictionary<string, MethodUsageInformation> GetInterfaceUsageDeclarations(GeneratorExecutionContext context,
        CustomAspectInterfaceReceiver customAspectReceiver, Dictionary<string, InterfaceSymbolDeclaration> interfaceSymbols)
    {
        var interfaceUsageDeclarations = new Dictionary<string, MethodUsageInformation>();
        foreach (var methodDeclaration in customAspectReceiver.MethodDeclarations)
        {
            var model = context.Compilation.GetSemanticModel(methodDeclaration.SyntaxTree.GetRoot().SyntaxTree);
            var modelSymbol = new Lazy<ISymbol?>(() => model.GetDeclaredSymbol(methodDeclaration));
            foreach (var parameter in methodDeclaration.ParameterList.Parameters)
            {
                var pSymbol = model.GetDeclaredSymbol(parameter) as IParameterSymbol;
                if (pSymbol == null) continue;
                var str = $"{pSymbol.Type.ContainingNamespace}.{pSymbol.Type.Name}";
                if (!interfaceSymbols.TryGetValue(str, out var interf)) continue;
                var modelSymb = modelSymbol.Value;
                if (modelSymb == null) continue;
                interfaceUsageDeclarations
                    .GetOrAdd(methodDeclaration.Identifier.Text, () => new MethodUsageInformation(methodDeclaration, modelSymb, new()))
                    .InterfaceUsages.Add(new(interf, pSymbol.Type, parameter.Identifier.Text));
            }
        }

        return interfaceUsageDeclarations;
    }

    private static Dictionary<string, InterfaceSymbolDeclaration> GetInterfaceTypeDictionary(GeneratorExecutionContext context,
        CustomAspectInterfaceReceiver customAspectReceiver)
    {
        Dictionary<string, InterfaceSymbolDeclaration> interfaceSymbols = new();
        foreach (var interf in customAspectReceiver.CustomAspectInterfaces)
        {
            var root = interf.Interface.SyntaxTree.GetRoot().SyntaxTree;
            var model = context.Compilation.GetSemanticModel(root);
            var symb = model.GetDeclaredSymbol(interf.Interface) as ITypeSymbol;
            if (symb == null) continue;
            interfaceSymbols[$"{symb.ContainingNamespace}.{symb.Name}"] = new(
                interf.Interface,
                symb,
                interf.Types.Select(t =>
                {
                    ITypeSymbol? typeSymbol = null;
                    if (t.Type is IdentifierNameSyntax ident)
                    {
                        typeSymbol = model.GetTypeInfo(ident).Type;
                    }
                    return new TypeDeclaration(t, typeSymbol);
                }).ToArray());
        }

        return interfaceSymbols;
    }
}