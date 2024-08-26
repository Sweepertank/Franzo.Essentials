using System.Reflection;
using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class SymbolExtensions
{
    public static readonly object DuplicateSourceComparer;
    public static readonly MethodInfo ComparerEqualsMethod;
    public static readonly MethodInfo UnderlyingSymbolGetter;

    static SymbolExtensions()
    {
        var codeAnalysisCSharpAssembly = typeof(CSharpCompilation).Assembly;
        var cSharpSymbolType = codeAnalysisCSharpAssembly.GetType("Microsoft.CodeAnalysis.CSharp.Symbol");
        var memberSignatureComparerType = codeAnalysisCSharpAssembly.GetType(
            "Microsoft.CodeAnalysis.CSharp.Symbols.MemberSignatureComparer");

        DuplicateSourceComparer = memberSignatureComparerType
            .GetField("DuplicateSourceComparer")
            .GetValue(null);

        ComparerEqualsMethod = memberSignatureComparerType.GetMethod(
            "Equals",
            new Type[] { cSharpSymbolType, cSharpSymbolType });

        UnderlyingSymbolGetter = codeAnalysisCSharpAssembly
            .GetType("Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.Symbol")
            .GetProperty("UnderlyingSymbol", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetMethod;
    }

    public static bool IsAccessibleWithin(
        this ISymbol self,
        ISymbol within,
        CSharpCompilation compilation,
        ITypeSymbol? throughType = null)
    {
        return compilation.IsSymbolAccessibleWithin(self, within, throughType);
    }

    /*public static bool HasInheritInterfaceAttribute(this ISymbol self)
    {
        return self.HasAttribute<InheritInterfaceAttribute>()
            || self.HasGenericAttribute(typeof(InheritInterfaceAttribute<>));
    }*/

    /*public static bool HasInterfaceDataAttribute(this ISymbol self)
    {
        return self.HasAttribute<InterfaceDataAttribute>();
    }*/

    public static bool HasOverrideAttribute(this ISymbol self)
    {
        return self.HasAttribute<OverrideAttribute>();
    }

    /*public static AttributeData? InterfaceDataAttribute(this ISymbol self)
    {
        return self.GetAttributes<InterfaceDataAttribute>().FirstOrDefault();
    }*/

    public static bool IsAccessibleFromType(
        this ISymbol self,
        ITypeSymbol type,
        CSharpCompilation compilation)
    {
        return self.IsAccessibleWithin(type, compilation, type)
            // @Todo: Roslyn is also not correctly detecting that private interface members shouldn't be accessible from anywhere but the interface;
            // otherwise the following check is unnecessary
            && (self.DeclaredAccessibility is not Accessibility.Private
                // @Todo: this is crappy: if symbol is a type, it will return its parent type
                // And doesn't account for weirdness with e.g. whether parent type can access inner type private methods, idk
                || self.ContainingType.CorrectEquals(type));
    }

    public static bool MemberCollidesWith(
        this ISymbol self,
        ISymbol otherMember,
        CSharpCompilation compilation)
    {
        if (self is IFieldSymbol || otherMember is IFieldSymbol)
        {
            return self.Name == otherMember.Name;
        }

        return Compare(DuplicateSourceComparer, self, otherMember);
    }

    public static bool IsProtectedOrProtectedAndOrInternal(this ISymbol self)
    {
        return self.DeclaredAccessibility
            is Accessibility.Protected
            or Accessibility.ProtectedAndInternal
            or Accessibility.ProtectedOrInternal;
    }

    public static bool IsPublicOrInternal(this ISymbol self)
    {
        return self.DeclaredAccessibility
            is Accessibility.Public
            or Accessibility.Internal;
    }

    private static bool Compare(object comparer, ISymbol a, ISymbol b)
    {
        // @todo: use expressions and create a delegate?
        // https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions.memberexpression?view=net-8.0
        return (bool)ComparerEqualsMethod.Invoke(comparer, new[]
        {
            a.UnderlyingSymbol(),
            b.UnderlyingSymbol()
        });
    }

    private static object UnderlyingSymbol(this ISymbol self)
    {
        return UnderlyingSymbolGetter.Invoke(self, null);
    }
}
