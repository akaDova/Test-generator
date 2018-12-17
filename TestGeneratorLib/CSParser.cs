﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace TestGeneratorLib
{
    class CSParser
    {
        public IEnumerable<(string, string)> GetTestCode(string sourceCode)
        {
            try
            {
                Console.WriteLine("GetTestCode");
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, encoding: Encoding.UTF8);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
                Console.WriteLine("GetTestCode");
                //IEnumerable<UsingDirectiveSyntax> usingNamespaces = root.DescendantNodes()
                //    .OfType<UsingDirectiveSyntax>();

                IEnumerable<ClassDeclarationSyntax> classes = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>();



                return from classDecl in classes
                       select GenerateTest(classDecl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Уважаемый, ошибка");
                return null;
            }



        }
        //удалить
        internal (string, string) GenerateTest(ClassDeclarationSyntax classDeclaration)
        {
            string testFileName = classDeclaration.Identifier.Text + "Test.cs";



            string namespaceName = ((NamespaceDeclarationSyntax)classDeclaration.Parent).Name.ToString();
            string className = classDeclaration.Identifier.Text;

            NamespaceDeclarationSyntax @namespace = GenerateNamespace();

            ClassDeclarationSyntax @class = GenerateClass(className);

            IEnumerable<MemberDeclarationSyntax> methods = classDeclaration.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(method => method.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)))
                .Select(method => method.Identifier.Text)
                .Distinct()
                .Select(methodName => GenerateMethod(methodName))
                
                ;
            foreach (var method in methods)
            {
                @class.AddMembers(method);
            }
            string testSourceCode = CompilationUnit()
                .WithUsings(List<UsingDirectiveSyntax>(new UsingDirectiveSyntax[]
                {
                    UsingDirective(IdentifierName("System")),
                    UsingDirective(IdentifierName("Xunit")),
                    UsingDirective(IdentifierName(namespaceName))
                }))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(
                    GenerateNamespace().WithMembers(SingletonList<MemberDeclarationSyntax>(
                        GenerateClass(className).WithMembers(List(
                            methods.ToArray()
                        ))
                    ))
                )).NormalizeWhitespace().ToFullString();

            return (testFileName, testSourceCode);


        }

       

        NamespaceDeclarationSyntax GenerateNamespace()
        {
            return NamespaceDeclaration(IdentifierName("UnitTest"));
        }

        ClassDeclarationSyntax GenerateClass(string name)
        {
            return ClassDeclaration($"{name}UnitTest")
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
            
        }

        MethodDeclarationSyntax GenerateMethod(string name)
        {



            return /*CompilationUnit().WithMembers(SingletonList<MemberDeclarationSyntax>(*/
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier($"{name}Test"))
                .WithAttributeLists(SingletonList<AttributeListSyntax>(
                    AttributeList(SingletonSeparatedList<AttributeSyntax>(
                        Attribute(IdentifierName("Fact")))
                    )))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBody(Block(SingletonList<StatementSyntax>(
                    ExpressionStatement(InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Assert"), IdentifierName("True"))
                    ).WithArgumentList(
                        ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                        {
                            Argument(LiteralExpression(SyntaxKind.FalseLiteralExpression)),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                            Literal("autogenerated")))
                        })))))))/*))*/;
                            //.NormalizeWhitespace().ToFullString();
        }


    }
}
