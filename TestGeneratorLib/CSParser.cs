using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGeneratorLib
{
    class CSParser
    {
        public async Task<IEnumerable<(string, string)>> GetTestCode(string sourceCode)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            IEnumerable<NamespaceDeclarationSyntax> namespaces = root.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>();

            IEnumerable<ClassDeclarationSyntax> classes = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>();



            from classDecl in classes
            select classDecl.

        }




    }
}
