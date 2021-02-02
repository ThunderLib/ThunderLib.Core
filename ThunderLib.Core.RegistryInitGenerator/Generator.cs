// This source generator scans for all class declarations and looks for those that inherit Registry, makes sure they are properly set up, and then emits a body for the module initializer
namespace ThunderLib.Core.RegistryInitGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using ThunderLib.Core.GeneratorHelpers;

    [Generator]
    public sealed class RegistryInitGenerator : ISourceGenerator
    {
        const String registrySystemTypeName = "global::ThunderLib.Core.RegistrySystem.Registry";
        const String registrySystemInitCall = "global::ThunderLib.Core.RegistrySystem.RegistryInitializer.Init";

        public void Execute(GeneratorExecutionContext context)
        {
            static Boolean InheritsRegistry(INamedTypeSymbol nts, INamedTypeSymbol top, out ITypeSymbol defTs, out ITypeSymbol backendTs)
            {
                if(nts.IsStatic || nts.BaseType is not INamedTypeSymbol baseNts || !baseNts.IsGenericType)
                {
                    defTs = backendTs = null;
                    return false;
                }

                if(baseNts.GloballyQualifiedTypeName() == registrySystemTypeName && baseNts.TypeArguments.Length == 3 && baseNts.TypeArguments[0].GloballyQualifiedTypeName() == top.GloballyQualifiedTypeName())
                {
                    defTs = baseNts.TypeArguments[1];
                    backendTs = baseNts.TypeArguments[2];
                    return true;
                }
                return InheritsRegistry(baseNts, top, out defTs, out backendTs);
            }


            var rec = context.SyntaxReceiver as Reciever;
            List<String> generatedCalls = new();
            foreach(var candidate in rec?.candidates ?? Enumerable.Empty<ClassDeclarationSyntax>())
            {
                var model = context.Compilation.GetSemanticModel(candidate.SyntaxTree, true);
                var di = model.GetDeclaredSymbol(candidate);
                if(!di.IsSealed || di.IsAbstract || di.IsGenericType) continue;
                if(InheritsRegistry(di, di, out var defI, out var backI))
                {
                    generatedCalls.Add($"{registrySystemInitCall}<{di.GloballyQualifiedTypeName(true)}, {defI.GloballyQualifiedTypeName(true)}, {backI.GloballyQualifiedTypeName(true)}>(true);");
                }
            }

            context.AddSource("RegistrySystemInit",
$@"
namespace ThunderLib.Core.RegistrySystem._GENERATED
{{
    internal static partial class _Module
    {{
        [global::System.Runtime.CompilerServices.ModuleInitializerAttribute]
        internal static void InitRegistrySystem()
        {{
            //global::System.Console.WriteLine(""RegistrySystemInit"");
{String.Join(Environment.NewLine, generatedCalls.Select(s => $"            {s}"))}
        }}
    }}
}}
");
        }
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new Reciever());
        }


        private class Reciever : ISyntaxReceiver
        {
            internal List<ClassDeclarationSyntax> candidates = new();
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if(syntaxNode.IsKind(SyntaxKind.ClassDeclaration) && syntaxNode is ClassDeclarationSyntax classDec)
                {
                    this.candidates.Add(classDec);
                }
            }
        }
    }
}
