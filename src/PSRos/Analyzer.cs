using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PSRos
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PowerShellModuleAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "Analyzer1";
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Cmdlet name not a standard verb",
            messageFormat: "Cmdlet '{0}' has a non-standard verb '{1}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Check verbs are consistent");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterCompilationAction(OnCompilationComplete);
        }

        private static void OnCompilationComplete(CompilationAnalysisContext context)
        {

        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            foreach (var attr in namedTypeSymbol.GetAttributes())
            {
                var attrClass = attr.AttributeClass;
                var arg = attr.ConstructorArguments.First();

                var verb = arg.Value as String;

                if (verb != "Remove")
                {
                    var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name, verb);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
