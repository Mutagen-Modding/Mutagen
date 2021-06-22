using System;
using System.Collections.Generic;
using System.Linq;
using Loqui;
using Loqui.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Noggog;

namespace Mutagen.Bethesda.SourceGenerators.CustomAspectInterface
{
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
                    var fg = new FileGeneration();

                    foreach (var use in requiredUsings)
                    {
                        fg.AppendLine($"using {use};");
                    }
                    fg.AppendLine();

                    using (new NamespaceWrapper(fg, namespaceGroup.Key))
                    {
                        using (new RegionWrapper(fg, "Wrappers"))
                        {
                            foreach (var decl in namespaceGroup.Value.UsedInterfaces)
                            {
                                if (context.CancellationToken.IsCancellationRequested) return;
                                foreach (var t in decl.Types)
                                {
                                    var className = $"{t.Syntax.Type}Wrapper";
                                    using (var c = new ClassWrapper(fg, className))
                                    {
                                        c.Interfaces.Add(decl.Symbol.Name);
                                    }
                                    using (new BraceWrapper(fg))
                                    {
                                        fg.AppendLine($"private readonly {t.Syntax.Type} _wrapped;");
                                        foreach (var member in decl.Symbol.GetMembers())
                                        {
                                            switch (member)
                                            {
                                                case IPropertySymbol prop:
                                                    fg.AppendLine($"public {prop.Type} {prop.Name}");
                                                    using (new BraceWrapper(fg))
                                                    {
                                                        if (!prop.IsWriteOnly)
                                                        {
                                                            fg.AppendLine($"get => _wrapped.{member.Name};");
                                                        }
                                                        if (!prop.IsReadOnly)
                                                        {
                                                            fg.AppendLine($"set => _wrapped.{member.Name} = value;");
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    continue;
                                            }
                                            fg.AppendLine();
                                        }

                                        using (var args = new FunctionWrapper(fg, $"public {className}"))
                                        {
                                            args.Add($"{t.Syntax.Type} rhs");
                                        }
                                        using (new BraceWrapper(fg))
                                        {
                                            fg.AppendLine("_wrapped = rhs;");
                                        }
                                    }
                                    fg.AppendLine();
                                }
                            }
                            fg.AppendLine();
                        }

                        if (context.CancellationToken.IsCancellationRequested) return;
                        using (new RegionWrapper(fg, "Mix Ins"))
                        {
                            using (var c = new ClassWrapper(fg, "WrapperMixIns"))
                            {
                                c.Static = true;
                            }
                            using (new BraceWrapper(fg))
                            {
                                foreach (var decl in namespaceGroup.Value.UsedInterfaces)
                                {
                                    if (context.CancellationToken.IsCancellationRequested) return;
                                    foreach (var t in decl.Types)
                                    {
                                        using (var args = new FunctionWrapper(fg,
                                            $"public static {t.Syntax.Type}Wrapper As{decl.Symbol.Name}"))
                                        {
                                            args.Add($"this {t.Syntax.Type} rhs");
                                        }
                                        using (new BraceWrapper(fg))
                                        {
                                            fg.AppendLine($"return new {t.Syntax.Type}Wrapper(rhs);");
                                        }
                                        fg.AppendLine();
                                    }
                                }
                            }
                        }
                    }
                    context.AddSource($"CustomAspectInterfaces.g.cs", fg.ToString());
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
}