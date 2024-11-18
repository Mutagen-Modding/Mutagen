using System.Collections.Immutable;
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
public class CustomAspectInterfaceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var customAspectInterfaceDefs = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (node, _) => (InterfaceDeclarationSyntax)node.Node)
            .Select(static (interf, cancel) =>
            {
                cancel.ThrowIfCancellationRequested();
                var customAspectAttr = interf.AttributeLists.SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.Name.ToString() == "CustomAspectInterface");
                if (customAspectAttr == null) return null;
                return new InterfaceDeclaration(interf, customAspectAttr.ArgumentList?.Arguments.Select(x => x.Expression)
                    .WhereCastable<ExpressionSyntax, TypeOfExpressionSyntax>().ToArray() ?? Enumerable.Empty<TypeOfExpressionSyntax>().ToArray());
            })
            .Where(x => x != null)
            .Select((x, _) => x!);

        var aspectInterfaceMethodDecls = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is MethodDeclarationSyntax,
                transform: static (node, _) => (MethodDeclarationSyntax)node.Node)
            .Where(IsInPartialClass);
        
        var combination = context.CompilationProvider
            .Combine(aspectInterfaceMethodDecls.Collect())
            .Combine(customAspectInterfaceDefs.Collect());

        context.RegisterSourceOutput(combination, (sourceContext, data) =>
        {
            Execute(sourceContext, data.Left.Left, data.Left.Right, data.Right);
        });
    }

    private static bool IsInPartialClass(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax cl)
        {
            return cl.Modifiers.Any(m => m.Text == "partial");
        }

        if (node.Parent == null) return false;
        return IsInPartialClass(node.Parent);
    }

    public void Execute(
        SourceProductionContext context, 
        Compilation compilation, 
        ImmutableArray<MethodDeclarationSyntax> methods,
        ImmutableArray<InterfaceDeclaration> interfaces)
    {
        if (context.CancellationToken.IsCancellationRequested) return;

        if (context.CancellationToken.IsCancellationRequested) return;
        var interfaceSymbols = GetInterfaceTypeDictionary(compilation, interfaces);

        // Group things by namespace
        if (context.CancellationToken.IsCancellationRequested) return;
        var targets = new Dictionary<string, GenerationTarget>();

        foreach (var namespaceGroup in interfaceSymbols.Values
                     .GroupBy(x => x.Symbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            if (namespaceGroup.Key == null) continue;
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

    private static Dictionary<string, InterfaceSymbolDeclaration> GetInterfaceTypeDictionary(
        Compilation compilation,
        ImmutableArray<InterfaceDeclaration> interfaces)
    {
        Dictionary<string, InterfaceSymbolDeclaration> interfaceSymbols = new();
        foreach (var interf in interfaces)
        {
            var root = interf.Interface.SyntaxTree.GetRoot().SyntaxTree;
            var model = compilation.GetSemanticModel(root);
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