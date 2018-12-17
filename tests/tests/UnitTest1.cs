using System;
using Xunit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            SyntaxTree t = CSharpSyntaxTree.ParseText("");
        }
    }
}
