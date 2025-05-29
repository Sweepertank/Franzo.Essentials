using System.Reflection;
using Franzo.Essentials.Reflection;
using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal static class SymbolExtensions
{
    //public static readonly object DuplicateSourceComparer;
    //public static readonly MethodInfo ComparerEqualsMethod;
    //public static readonly MethodInfo UnderlyingSymbolGetter;
    private static readonly object DuplicateSourceComparerInstance;
    private static readonly MethodInfo DuplicateSourceComparerEqualsMethod;
    private static readonly MethodInfo UnderlyingSymbolGetMethod;
    private static Func<ISymbol, ISymbol, bool> DuplicateSourceComparer;

    private static SymbolDisplayFormat FullyQualifiedWithoutGlobalNamespaceFormat =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    static SymbolExtensions()
    {
        var codeAnalysisCSharpAssembly = typeof(CSharpCompilation).Assembly;
        var cSharpSymbolType = codeAnalysisCSharpAssembly.GetType("Microsoft.CodeAnalysis.CSharp.Symbol");
        var memberSignatureComparerType = codeAnalysisCSharpAssembly.GetType(
            "Microsoft.CodeAnalysis.CSharp.Symbols.MemberSignatureComparer");

        DuplicateSourceComparerInstance = memberSignatureComparerType
            .GetField("DuplicateSourceComparer")
            .GetValue(null);

        DuplicateSourceComparerEqualsMethod = memberSignatureComparerType.GetMethod(
            "Equals",
            new Type[] { cSharpSymbolType, cSharpSymbolType });

        UnderlyingSymbolGetMethod = codeAnalysisCSharpAssembly
            .GetType("Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.Symbol")
            .GetProperty("UnderlyingSymbol", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetMethod;

        DuplicateSourceComparer = CreateDuplicateSourceComparer();
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
        ISymbol otherMember)
    {
        if (self is IFieldSymbol || otherMember is IFieldSymbol)
        {
            return self.Name == otherMember.Name;
        }

        return DuplicateSourceComparer.Invoke(self, otherMember);
    }

    public static bool IsGeneric(this ISymbol self)
    {
        return self.TypeParameters().Count() > 0;
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

    public static string ToMangledFullyQualifiedWithoutGlobalNamespaceDisplayString(
        this ISymbol self)
    {
        return self.ToDisplayString(FullyQualifiedWithoutGlobalNamespaceFormat).MangleSymbolName();
    }

    public static bool IsProtectedOrProtectedAndOrInternalAndGenericOrOriginalDefinitionIsGenericTypeContained(
        this ISymbol self)
    {
        return self.IsProtectedOrProtectedAndOrInternal()
            && (self.IsGeneric() || self.OriginalDefinition.ContainingType.IsGenericType);
    }

    public static string ToFullyQualifiedWithNullableReferenceTypeAnnotationsDisplayString(
        this ISymbol self,
        TypeEmissionContext cxt)
    {
        return cxt.MainContext.GetFullyQualifiedWithNullableReferenceTypeAnnotationsDisplayString(
            self);
    }

    private static Func<ISymbol, ISymbol, bool> CreateDuplicateSourceComparer()
    {
        var coreMethod = typeof(SymbolExtensions).GetMethod(
            nameof(CreateDuplicateSourceComparerCore),
            BindingFlags.Static | BindingFlags.NonPublic);
        var constructedCoreMethod = coreMethod.MakeGenericMethod(UnderlyingSymbolGetMethod.ReturnType);
        return (Func<ISymbol, ISymbol, bool>)constructedCoreMethod.Invoke(null, []);
    }

    private static Func<ISymbol, ISymbol, bool> CreateDuplicateSourceComparerCore<T>()
    {
        var underlyingSymbolGetter = UnderlyingSymbolGetMethod.CreateFastGetter<T>();
        var duplicateSourceComparerEqualsMethodDelegate =
            (Func<T, T, bool>)DuplicateSourceComparerEqualsMethod.CreateDelegate(
                typeof(Func<T, T, bool>),
                DuplicateSourceComparerInstance);

        return (a, b) => duplicateSourceComparerEqualsMethodDelegate.Invoke(
            underlyingSymbolGetter.Invoke(a),
            underlyingSymbolGetter.Invoke(b));
    }

    /*private static object UnderlyingSymbol(this ISymbol self)
    {
        return UnderlyingSymbolGetter.Invoke(self, null);
    }*/
}
