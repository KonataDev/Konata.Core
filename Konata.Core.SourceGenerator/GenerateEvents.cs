using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Konata.Core.SourceGenerator.Utilities;

namespace Konata.Core.SourceGenerator;

internal static partial class TypeWithAttributeDelegates
{
    public static string? GenerateEvents(TypeDeclarationSyntax typeDeclaration, INamedTypeSymbol typeSymbol, List<AttributeData> attributeList)
    {
        var specifiedAttribute = attributeList[0];
        var namespaces = new HashSet<string> { "System", "System.Collections.Generic" };
        var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        var stringBuilder = new StringBuilder().AppendLine("#nullable enable\n");
        var classBegin = new StringBuilder().AppendLine($@"
namespace {typeSymbol.ContainingNamespace.ToDisplayString()};

partial class {typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}
{{
    private Dictionary<Type, Action<BaseEvent>> _dict;

    public delegate void KonataEvent<in TArgs>(Bot sender, TArgs args);
");
        var initializeHandlers = new StringBuilder().Append($@"
    internal void InitializeHandlers()
    {{
        _dict = new()
        {{
");
        const string end = @$"
        }};
    }}
}}";

        foreach (var t in specifiedAttribute.ConstructorArguments[0].Values.Select(tc => (INamedTypeSymbol)tc.Value!))
        {
            // 去掉Event
            var name = "On" + t.Name.Substring(0, t.Name.Length - 5);
            namespaces.UseNamespace(usedTypes, typeSymbol, t);
            initializeHandlers.AppendLine(AddEventDictionary(t, name));
            classBegin.AppendLine(AddEventProperty(t, name));
        }

        // 去掉",\r\n"
        initializeHandlers.Remove(initializeHandlers.Length - 3, 3);

        foreach (var ns in namespaces)
            stringBuilder.AppendLine($"using {ns};");
        stringBuilder
            .Append(classBegin)
            .Append(initializeHandlers)
            .Append(end);

        return stringBuilder.ToString();
    }

    private static string AddEventDictionary(INamedTypeSymbol type, string name) => $"{Spacing(3)}{{ typeof({type.Name}), e => {name}?.Invoke(this, ({type.Name})e) }},";

    private static string AddEventProperty(INamedTypeSymbol type, string name) => $"{Spacing(1)}public event KonataEvent<{type.Name}> {name};\n";
}