using System;
using Xunit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FluentAssertions;

namespace PSRos
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var program = @"
            using System.Management.Automation;

namespace SampleModule
{
    [Cmdlet(VerbsCommon.Get, ""Foo"")]
    public class GetFooCmdlet : PSCmdlet
    {

    }
}
";
            DiagnosticAnalyzer da = new PowerShellModuleAnalyzer();
            Diagnostic[] results = DiagnosticVerifier.GetSortedDiagnostics(new[] { program }, LanguageNames.CSharp, da);
            results.Should().NotBeEmpty();
        }
    }
}
