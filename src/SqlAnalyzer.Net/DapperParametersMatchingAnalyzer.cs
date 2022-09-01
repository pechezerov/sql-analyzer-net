using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SqlAnalyzer.Net.Extensions;
using SqlAnalyzer.Net.Models;
using SqlAnalyzer.Net.Rules;
using SqlAnalyzer.Net.Walkers;

namespace SqlAnalyzer.Net
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperParametersMatchingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(ParametersMatchingRule.CsharpArgumentNotFoundRule, ParametersMatchingRule.SqlParameterNotFoundRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static (string, ICollection<string>)? FindParameters(SyntaxNodeAnalysisContext context, ArgumentSyntax argument)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(argument.Expression).Symbol;
            if (symbol == null)
                return (null, null);

            if (symbol is IMethodSymbol methodSymbol)
                return (null, methodSymbol.Parameters.Select(p => p.Name).ToList());

            if (symbol is ILocalSymbol localSymbol)
            {
                var methodDeclarationSyntax = argument
                        .Expression
                        .FindMethodDeclaration();
                if (methodDeclarationSyntax == null)
                    return null;

                if (localSymbol.IsDapperCommandDefinition(context.SemanticModel))
                {
                    var dapperAddInvocationExpressionWalker = new DapperCommandDefinitionWalker(symbol.Name);
                    dapperAddInvocationExpressionWalker.Visit(methodDeclarationSyntax.Body);
                    return (dapperAddInvocationExpressionWalker.Sql, dapperAddInvocationExpressionWalker.SqlParameters);

                }
                else if (localSymbol.IsDapperDynamicParameter(context.SemanticModel))
                {
                    var dapperAddInvocationExpressionWalker = new DapperDynamicParametersWalker(symbol.Name);
                    dapperAddInvocationExpressionWalker.Visit(methodDeclarationSyntax.Body);
                    if (!dapperAddInvocationExpressionWalker.IsAllParametersStatic)
                        return null;

                    return (null, dapperAddInvocationExpressionWalker.SqlParameters);
                }
            }

            return (null, null);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            if (!invocationExpressionSyntax.IsDapperInlineSqlMethod(context.SemanticModel))
            {
                return;
            }

            ReportDiagnostics(context, invocationExpressionSyntax);
        }

        private void ReportDiagnostics(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpressionSyntax)
        {
            string sqlText = null;
            ICollection<string> sharpParameters = null;
            foreach (var argument in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                var parameter = argument.DetermineParameter(context.SemanticModel);
                if (string.Equals(parameter.Name, "sql"))
                {
                    var sourceText = argument.TryGetArgumentStringValue(context.SemanticModel);

                    // If SQL code is not constant, return
                    if (sourceText == null)
                    {
                        return;
                    }

                    sqlText = sourceText;

                    continue;
                }

                (string sql, ICollection<string> param)? p = null;
                if (string.Equals(parameter.Name, "param") || string.Equals(parameter.Name, "command"))
                    p = FindParameters(context, argument);

                if (p?.sql != null)
                    sqlText = p?.sql;
                if (p?.param != null)
                    sharpParameters = p?.param;
            }

            ParametersMatchingRule.TryReportDiagnostics(sqlText, sharpParameters, invocationExpressionSyntax.GetLocation(), context, Orm.Dapper);
        }
    }
}
