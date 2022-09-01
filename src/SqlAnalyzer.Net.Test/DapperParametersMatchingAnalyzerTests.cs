using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Rules;
using SqlAnalyzer.Net.Test.Helpers;

namespace SqlAnalyzer.Net.Test
{
    [TestClass]
    public class DapperParametersMatchingAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "DapperParametersMatchingAnalyzer";

        [TestMethod]
        public void InlineSqlDeclaredVariableParameters_NotTriggered()
        {
            var code = ReadTestData("InlineSqlDeclaredVariableParameters.cs");

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void InlineSqlUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatCsharpArgumentNotFound,
                                       "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlUnmatchedParametersWithCommandDefinition_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedParametersWithCommandDefinition.cs");

            var expected = new DiagnosticResult
            {
                Id = ParametersMatchingRule.DiagnosticId,
                Message = string.Format(
                                       ParametersMatchingRule.MessageFormatCsharpArgumentNotFound,
                                       "not_found"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 17, 13) }
            };

            VerifyCSharpDiagnostic(code, expected);
        }


        [TestMethod]
        public void InlineSqlUnmatchedParametersWithConstructedCommandDefinition_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedParametersWithConstructedCommandDefinition.cs");

            var expected = new DiagnosticResult
            {
                Id = ParametersMatchingRule.DiagnosticId,
                Message = string.Format(
                                       ParametersMatchingRule.MessageFormatCsharpArgumentNotFound,
                                       "not_found"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 17, 13) }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlUnmatchedParametersWithLiterals_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlLiteralParameters.cs");

            var expected = new DiagnosticResult
            {
                Id = ParametersMatchingRule.DiagnosticId,
                Message = string.Format(
                    ParametersMatchingRule.MessageFormatCsharpArgumentNotFound,
                    "not_found"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlUnmatchedSqlVariable_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedSqlVariable.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatSqlVariableNotFound,
                                       "param"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlWithVariableUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlWithConstUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatCsharpArgumentNotFound,
                                       "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlDynamicParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlDynamicParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatSqlVariableNotFound,
                                       "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlDynamicParametersWithConstructor_AnalyzerNotTriggered()
        {
            var code = ReadTestData("InlineSqlDynamicParametersWithConstructor.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperParametersMatchingAnalyzer();
        }
    }
}
