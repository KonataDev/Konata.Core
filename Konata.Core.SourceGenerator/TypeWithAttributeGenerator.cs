using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Konata.Core.SourceGenerator;

/// <summary>
/// References:
/// <br/> <a href="https://andrewlock.net/series/creating-a-source-generator/"/>
/// <br/> <a href="https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md/"/>
/// </summary>
[Generator]
public class TypeWithAttributeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// 对拥有某attribute的type生成代码
    /// </summary>
    /// <param name="typeDeclarationSyntax"></param>
    /// <param name="typeSymbol"></param>
    /// <param name="attributeList">该类的某种Attribute</param>
    /// <returns>生成的代码</returns>
    private delegate string? TypeWithAttribute(TypeDeclarationSyntax typeDeclarationSyntax, INamedTypeSymbol typeSymbol, List<AttributeData> attributeList);

    /// <summary>
    /// 需要生成的Attribute
    /// </summary>
    private static readonly Dictionary<string, TypeWithAttribute> Attributes = new()
    {
        { "Konata.Core.Attributes.GenerateEventsAttribute", TypeWithAttributeDelegates.GenerateEvents },
        { "Konata.Core.Attributes.GenerateComponentsAttribute", TypeWithAttributeDelegates.GenerateComponents }
    };
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeDeclarationSyntax> typeDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<TypeDeclarationSyntax> TypeDeclarationSyntaxes)> compilationAndTypes =
            context.CompilationProvider.Combine(typeDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndTypes, static (spc, source) =>
            Execute(source.Compilation, source.TypeDeclarationSyntaxes, spc));
    }

    /// <summary>
    /// 初次快速筛选（对拥有Attribute的class和record）
    /// </summary>
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is TypeDeclarationSyntax { AttributeLists.Count: > 0 }
            and (ClassDeclarationSyntax or RecordDeclarationSyntax);

    /// <summary>
    /// 获取TypeDeclarationSyntax
    /// </summary>
    private static TypeDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;
        // 不用Linq，用foreach保证速度
        foreach (var attributeListSyntax in typeDeclarationSyntax.AttributeLists)
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                if (Attributes.ContainsKey(attributeSymbol.ContainingType.ToDisplayString()))
                    return typeDeclarationSyntax;
            }

        return null;
    }

    /// <summary>
    /// 对获取的每个type和Attribute进行生成
    /// </summary>
    private static void Execute(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types, SourceProductionContext context)
    {
        if (types.IsDefaultOrEmpty)
            return;
        // 遍历每个class
        foreach (var typeDeclarationSyntax in types)
        {
            var semanticModel = compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol typeSymbol)
                continue;

            // 同种attribute只判断一遍
            var usedAttributes = new Dictionary<string, List<AttributeData>>();

            // 遍历class上每个Attribute
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                var attributeName = attribute.AttributeClass!.ToDisplayString();
                if (!Attributes.ContainsKey(attributeName))
                    continue;
                if (usedAttributes.ContainsKey(attributeName))
                    usedAttributes[attributeName].Add(attribute);
                else usedAttributes[attributeName] = new List<AttributeData> { attribute };
            }

            foreach (var usedAttribute in usedAttributes)
                if (Attributes[usedAttribute.Key](typeDeclarationSyntax, typeSymbol, usedAttribute.Value) is { } source)
                    context.AddSource(
                        // 不能重名
                        $"{typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))}_{usedAttribute.Key}.g.cs",
                        source);
        }
    }
}