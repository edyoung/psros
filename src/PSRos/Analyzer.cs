using System;
using System.Linq;
using System.Collections.Immutable;
using System.Management.Automation;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PSRos
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PowerShellModuleAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "PS0001";
        private const string Category = "Naming";

        private static IImmutableSet<string> allVerbs;
        private static IImmutableSet<string> AllVerbs
        {
            get
            {
                if (null == allVerbs)
                {
                    var builder = ImmutableHashSet.CreateBuilder<string>();

                    Type[] verbTypes = new Type[] {
                        typeof(VerbsCommon),
                        typeof(VerbsCommunications),
                        typeof(VerbsData),
                        typeof(VerbsDiagnostic),
                        typeof(VerbsLifecycle),
                        typeof(VerbsOther),
                        typeof(VerbsSecurity)
                    };

                    foreach (Type type in verbTypes)
                    {
                        foreach (FieldInfo field in type.GetFields())
                        {
                            if (field.IsLiteral)
                            {
                                builder.Add(field.Name);
                            }
                        }
                    }

                    allVerbs = builder.ToImmutableHashSet();
                }
                return allVerbs;
            }
        }

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Cmdlet name not a standard verb",
            messageFormat: "Cmdlet '{0}' has a non-standard verb '{1}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
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
                if (attrClass.Name != "CmdletAttribute")
                {
                    continue;
                }

                // symbol is a [cmdlet] attribute, and must always have 2 arguments: verb, noun
                if (attr.ConstructorArguments.Count() != 2)
                {
                    continue; // compiler will tell user they have a problem
                }

                var verb = attr.ConstructorArguments[0].Value as String;
                var noun = attr.ConstructorArguments[1].Value as String;

                if (!AllVerbs.Contains(verb))
                {
                    var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name, verb);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
