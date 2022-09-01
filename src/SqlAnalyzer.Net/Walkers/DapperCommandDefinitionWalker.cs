using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlAnalyzer.Net.Walkers
{
    public class DapperCommandDefinitionWalker : CSharpSyntaxWalker
    {
        private readonly string _variableName;

        public DapperCommandDefinitionWalker(string variableName)
        {
            _variableName = variableName;
            IsAllParametersStatic = true;
        }

        public List<string> SqlParameters { get; } = new List<string>();
        public string Sql { get; private set; } 

        public bool IsAllParametersStatic { get; private set; }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.Identifier.ValueText != _variableName)
                return;

            var constructors = node.DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .ToList();

            var constructor = constructors.FirstOrDefault();
            if (constructor?.ArgumentList == null)
                return;
            if (constructor.ArgumentList.Arguments.Count == 0)
                return;

            foreach (var argument in constructor.ArgumentList.Arguments)
            {
                if (argument.Expression is LiteralExpressionSyntax literalSyntax)
                {
                    Sql = literalSyntax.GetText().ToString();
                }

                if (argument.Expression is AnonymousObjectCreationExpressionSyntax objectCreationExpressionSyntax)
                {
                    SqlParameters.AddRange(objectCreationExpressionSyntax
                        .DescendantNodes()
                        .OfType<AnonymousObjectMemberDeclaratorSyntax>()
                        .Select(n => n.NameEquals.Name.Identifier.ValueText));
                }
            }

            IsAllParametersStatic = false;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            
        }
    }
}
