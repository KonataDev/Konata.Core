using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Konata.Core.SourceGenerator.Utilities;

namespace Konata.Core.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string? GenerateComponents(TypeDeclarationSyntax typeDeclarationSyntax, INamedTypeSymbol typeSymbol, List<AttributeData> attributeList)
    {
        var specifiedAttribute = attributeList[0];

        if (specifiedAttribute.ConstructorArguments[0].Value is not string methodName)
            return null;

        var namespaces = new HashSet<string>();
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        var stringBuilder = new StringBuilder().AppendLine("#nullable enable\n");
        var classBegin = new StringBuilder().AppendLine($@"
namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

partial class {typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}
{{");

        foreach (var t in specifiedAttribute.ConstructorArguments[1].Values.Select(tc => (INamedTypeSymbol)tc.Value!))
        {
            namespaces.UseNamespace(usedTypes, typeSymbol, t);
            classBegin.AppendLine(AddComponentProperty(t, methodName));
        }

        // 去掉"\r\n"
        classBegin.Remove(classBegin.Length - 2, 2);

        foreach (var ns in namespaces)
            stringBuilder.AppendLine($"using {ns};");
        stringBuilder
            .Append(classBegin)
            .Append('}');

        return stringBuilder.ToString();
    }

    private static string AddComponentProperty(INamedTypeSymbol type, string methodName) => $"{Spacing(1)}internal {type.Name} {type.Name} => {methodName}<{type.Name}>();\n";
}