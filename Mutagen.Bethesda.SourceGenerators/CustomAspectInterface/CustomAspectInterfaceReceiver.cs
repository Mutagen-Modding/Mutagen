using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Noggog;

namespace Mutagen.Bethesda.SourceGenerators.CustomAspectInterface
{
    public class CustomAspectInterfaceReceiver : ISyntaxReceiver
    {
        public List<InterfaceDeclaration> CustomAspectInterfaces = new();
        public List<MethodDeclarationSyntax> MethodDeclarations = new();
        public List<Exception> Exceptions = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            try
            {
                FishForCustomAspectInterface(syntaxNode);
                FishForAspectInterfaceMethodDeclarations(syntaxNode);
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }
        }

        private void FishForCustomAspectInterface(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not InterfaceDeclarationSyntax interf) return;
            var customAspectAttr = interf.AttributeLists.SelectMany(x => x.Attributes)
                .FirstOrDefault(x => x.Name.ToString() == "CustomAspectInterface");
            if (customAspectAttr == null) return;
            CustomAspectInterfaces.Add(
                new(interf, customAspectAttr.ArgumentList?.Arguments.Select(x => x.Expression)
                    .WhereCastable<ExpressionSyntax, TypeOfExpressionSyntax>().ToArray() ?? Enumerable.Empty<TypeOfExpressionSyntax>().ToArray()));
        }

        private void FishForAspectInterfaceMethodDeclarations(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not MethodDeclarationSyntax method) return;
            if (!IsInPartialClass(method)) return;
            MethodDeclarations.Add(method);
        }

        private bool IsInPartialClass(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax cl)
            {
                return cl.Modifiers.Any(m => m.Text == "partial");
            }

            if (node.Parent == null) return false;
            return IsInPartialClass(node.Parent);
        }
    }
}