namespace ThunderLib.Core.GeneratorHelpers
{
    using System;
    using System.Linq;

    using Microsoft.CodeAnalysis;

    public static class TypeSymbolXtn
    {
        public static String GloballyQualifiedTypeName(this ITypeSymbol symbol, Boolean withGenerics = false)
        {
            static String RecName(INamespaceOrTypeSymbol symbol)
            {
                if(symbol is ITypeSymbol ts)
                {
                    if(ts.ContainingType is null)
                    {
                        if(ts.ContainingNamespace is null)
                        {
                            return ts.Name;
                        }
                        return $"{RecName(ts.ContainingNamespace)}.{ts.Name}";
                    } else
                    {
                        return $"{RecName(symbol.ContainingType)}.{ts.Name}";
                    }
                }
                if(symbol is INamespaceSymbol ns)
                {
                    if(ns.ContainingNamespace is null || String.IsNullOrWhiteSpace(ns.ContainingNamespace.Name))
                    {
                        return ns.Name;
                    }
                    return $"{RecName(ns.ContainingNamespace)}.{ns.Name}";
                }
                return null;
            }

            return $"global::{RecName(symbol)}{(withGenerics ? symbol.GetGenerics() : "")}";
        }

        public static String GetGenerics(this ITypeSymbol symbol)
        {
            if(symbol is not INamedTypeSymbol nts || nts.TypeArguments.Length <= 0 || !nts.IsGenericType || nts.TypeParameters.Length <= 0) return "";

            return $"<{String.Join(", ", nts.TypeArguments.Select(s => s.GloballyQualifiedTypeName(true)))}>";
        }
    }
}
