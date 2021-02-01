// This source generator scans for all class declarations and looks for those that inherit Registry, makes sure they are properly set up, and then emits a body for the module initializer
namespace ThunderLib.Core.ModuleInitGenerator
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
        const String registrySystemTypeName = "global::ThunderLib.Core.ModuleSystem.Module";
        const String registrySystemInitCall = "global::ThunderLib.Core.ModuleSystem.ModuleInitializer.Init";

        public void Execute(GeneratorExecutionContext context)
        {
            static Boolean InheritsModule(INamedTypeSymbol nts, INamedTypeSymbol top)
            {
                if(nts.IsStatic || nts.BaseType is not INamedTypeSymbol baseNts || !baseNts.IsGenericType)
                {
                    return false;
                }

                if(baseNts.GloballyQualifiedTypeName() == registrySystemTypeName && baseNts.TypeArguments.Length == 1 && baseNts.TypeArguments[0].GloballyQualifiedTypeName() == top.GloballyQualifiedTypeName())
                {
                    return true;
                }
                return InheritsModule(baseNts, top);
            }


            var rec = context.SyntaxReceiver as Reciever;
            List<String> generatedCalls = new();
            foreach(var candidate in rec?.candidates ?? Enumerable.Empty<ClassDeclarationSyntax>())
            {
                var model = context.Compilation.GetSemanticModel(candidate.SyntaxTree, true);
                var di = model.GetDeclaredSymbol(candidate);
                if(!di.IsSealed || di.IsAbstract || di.IsGenericType) continue;
                if(InheritsModule(di, di))
                {
                    generatedCalls.Add($"{registrySystemInitCall}<{di.GloballyQualifiedTypeName(true)}>(true);");
                }
            }

            context.AddSource("ModuleSystemInit",
$@"
namespace ThunderLib.Core.ModuleSystem._GENERATED
{{
    internal static partial class _Module
    {{
        [global::System.Runtime.CompilerServices.ModuleInitializerAttribute]
        internal static void InitModuleSystem()
        {{
            //global::System.Console.WriteLine(""ModuleSystemInit"");
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
