using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

public partial class InterfaceInheritanceGenerator : IIncrementalGenerator
{
    internal static void Analyze(InternalAnalysisContext context)
    {
        // InterfacesDeclaringShadowingMembers
        // @todo: bring down documentation, use attributes
        // @todo: maybe multithread? at least emitting?

        InitializeTypes_Phase1(context);
        InitializeTypes_Phase2(context);
        AnalyzeTypes_Phase1(context);
    }

    private static void InitializeTypes_Phase1(InternalAnalysisContext context)
    {
        foreach (var roslynType in context.PossiblyRelevantTopLevelRoslynTypes)
        {
            var x = roslynType.IsConstructedGenericType();
            var type = CreateTypeIfEmitting(roslynType, null, context);
            if (type is null) continue;

            context.Data.RootTypesToEmit.Add(type);
        }
    }

    private static InternalTypeSymbol? CreateTypeIfEmitting(
        INamedTypeSymbol roslynType,
        InternalTypeSymbol? containingType,
        InternalAnalysisContext context)
    {
        var type = CreateType(
            roslynType,
            containingType,
            AnalysisPhase.TypeInitializationPhase1,
            context,
            false);

        foreach (var roslynDeclaredType in roslynType.GetTypeMembers())
        {
            var declaredType = CreateTypeIfEmitting(
                roslynDeclaredType,
                type,
                context);
            if (declaredType is null) continue;

            type.DeclaredTypes.Add(declaredType);
        }

        if (type.DeclaredTypes.Count == 0 && !roslynType.IsPartiallyDeclared())
        {
            return null;
        }

        return type;
    }

    private static void InitializeTypes_Phase2(InternalAnalysisContext context)
    {
        foreach (var type in context.Data.TypesCreatedDuringTypeInitializationPhase1)
        {
            InitializeType_Phase2(type, context);
        }
    }

    private static void InitializeType_Phase2(
        InternalTypeSymbol type,
        InternalAnalysisContext context)
    {
        if (type.HasPhase2Initialized)
        {
            return;
        }

        InitializeTypeBaseTypesAndColonSpecifiedInterfaces(type, context);

        if (type.RoslynSymbol.Name == InterfaceDataClassName)
        {
            var invalid = false;

            if (type.RoslynSymbol.TypeKind is not TypeKind.Class)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            // Check this *instead* of IsGenericType, because we don't want this to pass for
            // a class without generic parameters nested inside a class *with* generic parameters
            if (type.RoslynSymbol.TypeParameters.Length > 0)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (type.RoslynSymbol.DeclaredAccessibility is not Accessibility.Public)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (type.ContainingType?.RoslynSymbol.TypeKind is not TypeKind.Interface)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (type.RoslynSymbol.Name != InterfaceDataClassName)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (!type.AreSelfAndContainingTypesPartiallyDeclared())
            {
                invalid = true;
            }

            var publicConstructors = type.DeclaredConstructors.Where(
                m => m.RoslynSymbol.DeclaredAccessibility is Accessibility.Public);
            if (publicConstructors.Count() > 1)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            var publicConstructor = publicConstructors.FirstOrDefault();
            if (publicConstructor is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (!invalid)
            {
                type.IsDataClass = true;
                type.ConstructorIfDataClass = publicConstructor;
                type.ContainingType!.DataClass = type;
            }
        }

        /*foreach (var attribute in type.RoslynSymbol.GetGenericAttributes(typeof(InheritInterfaceAttribute<>)))
        {
            var invalid = false;

            if (type.IsDataClass)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            var typeArgument = attribute.AttributeClass?.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
            if (typeArgument is null)
            {
                invalid = true;
            }

            if (type.RoslynSymbol.TypeKind is not TypeKind.Class)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            if (!type.AreSelfAndContainingTypesPartiallyDeclared())
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            if (typeArgument is not null)
            {
                if (typeArgument.TypeKind is not TypeKind.Interface)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                    invalid = true;
                }

                if (!type.RoslynSymbol.Interfaces.Contains(
                    typeArgument,
                    SymbolEqualityComparer.Default))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                    invalid = true;
                }
            }

            if (invalid) continue;

            var @interface = GetOrCreateType(
                typeArgument!,
                null,
                AnalysisPhase.TypeInitializationPhase2,
                context);

            if (type.AttributeSpecifiedDirectInterfaces.Contains(@interface))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            if (invalid) continue;

            type.AttributeSpecifiedDirectInterfaces.Add(@interface);
        }

        foreach (var attribute in type.RoslynSymbol.GetAttributes<InheritInterfaceAttribute>())
        {
            var invalid = false;

            if (type.IsDataClass)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            INamedTypeSymbol? roslynInterface = null;
            if (!attribute.TryGetReadableValueFromConstructorArgument<string>(
                0,
                out var interfaceName))
            {
                invalid = true;
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
            }
            else if (interfaceName is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }
            else if (!interfaceName.TryBindToType(
                type.RoslynSymbol,
                context.Compilation,
                out var boundType))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }
            else if (boundType!.TypeKind is not TypeKind.Interface)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }
            else if (!type.RoslynSymbol.Interfaces.Contains(
                boundType,
                SymbolEqualityComparer.Default))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }
            else
            {
                roslynInterface = (INamedTypeSymbol)boundType;
            }

            if (type.RoslynSymbol.TypeKind is not TypeKind.Class)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            if (!type.AreSelfAndContainingTypesPartiallyDeclared())
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            if (invalid) continue;

            var @interface = GetOrCreateType(
                roslynInterface!,
                null,
                AnalysisPhase.TypeInitializationPhase2,
                context);

            if (type.AttributeSpecifiedDirectInterfaces.Contains(@interface))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptors.Dummy, attribute.Location()));
                invalid = true;
            }

            if (invalid) continue;

            type.AttributeSpecifiedDirectInterfaces.Add(@interface);
        }*/

        type.HasPhase2Initialized = true;
    }

    private static void InitializeTypeBaseTypesAndColonSpecifiedInterfaces(
        InternalTypeSymbol type,
        InternalAnalysisContext context)
    {
        /*if (type.HasInitializedBaseTypesAndInterfaces)
        {
            return;
        }*/

        if (type.RoslynSymbol.BaseType is not null)
        {
            type.BaseType = GetOrCreateType(
                type.RoslynSymbol.BaseType,
                null,
                AnalysisPhase.TypeInitializationPhase2,
                context);
        }

        // This check is just an optimization -
        // it prevents us from loading in interfaces that we don't care about
        // (i.e. interfaces whose only connection to types that we *do* care about
        // is the fact that they're colon-specified by a class that we care about)
        //if (type.RoslynSymbol.TypeKind is TypeKind.Interface)
        //{
        foreach (var roslynInterface in type.RoslynSymbol.Interfaces)
        {
            var @interface = GetOrCreateType(
                roslynInterface,
                null,
                AnalysisPhase.TypeInitializationPhase2,
                context);
            type.ColonSpecifiedDirectInterfaces.Add(@interface);
        }
        //}

        //type.HasInitializedBaseTypesAndInterfaces = true;
    }

    private static void AnalyzeTypes_Phase1(InternalAnalysisContext context)
    {
        foreach (var type in context.Data.TypesCreatedDuringTypeInitializationPhase1)
        {
            AnalyzeType_Phase1(type, context);
        }
    }

    private static void AnalyzeType_Phase1(
        InternalTypeSymbol type,
        InternalAnalysisContext context)
    {
        if (type.HasPhase1Analyzed)
        {
            return;
        }

        foreach (var feature in type.DeclaredFeatures)
        {
            foreach (var @interface in type.Interfaces)
            {
                foreach (var interfaceFeature in @interface.DeclaredFeatures)
                {
                    if (feature.RoslynSymbol.MemberCollidesWith(
                        interfaceFeature.RoslynSymbol,
                        context.Compilation)
                        && !feature.RoslynSymbol.IsImplicitlyDeclared)
                    {
                        interfaceFeature.TypesDeclaringShadowingFeatures.Add(type);
                    }
                }
            }
        }

        if (type.RoslynSymbol.TypeKind.IsClassOrStruct())
        {
        }
        else
        {
            if (type.DataClass is null
                && type.ColonSpecifiedDirectInterfaces.Any(i => i.DataClass is not null))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Dummy,
                        type.RoslynSymbol.Locations.First()));
            }

            foreach (var feature in type.DeclaredFeatures)
            {
                if (feature.RoslynSymbol.HasOverrideAttribute())
                {
                    // @todo: check if the feature is sealed or not, and if it isn't,
                    // report an error
                    // but there's no consistent way to check this (interface members outside of compiling assembly
                    // always have IsVirtual = false, even if they have the 'sealed' keyword)
                    feature.HasOverrideAttribute = true;
                }
            }
        }

        if (type.RoslynSymbol.TypeKind.IsClassOrStruct())
        {
            if (!type.HasExplicitlyDeclaredConstructor)
            {
                var enableEmitDefaultConstructor = false;
                foreach (var @interface in type.AnchorType.DirectInterfaces)
                {
                    if (@interface.DataClass is not null
                        && !@interface.DataClass.ConstructorIfDataClass!.RoslynSymbol.CanBeCalledWithZeroArgumentsInSource())
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                DiagnosticDescriptors.Dummy,
                                type.RoslynSymbol.Locations.First()));
                        enableEmitDefaultConstructor = true;
                        break;
                    }
                }

                if (type.BaseType is not null)
                {
                    if (!type.BaseType.DeclaredConstructors.Any(
                        c => c.RoslynSymbol.IsAccessibleWithin(type.RoslynSymbol, context.Compilation)
                            && c.RoslynSymbol.CanBeCalledWithZeroArgumentsInSource()))
                    {
                        enableEmitDefaultConstructor = true;
                    }
                }

                if (!enableEmitDefaultConstructor)
                {
                    type.EmitDefaultConstructor = true;
                }
            }
        }

        if (type.BaseType is not null)
        {
            AnalyzeType_Phase1(type.BaseType, context);
        }

        foreach (var @interface in type.DirectInterfaces)
        {
            AnalyzeType_Phase1(@interface, context);
        }

        type.HasPhase1Analyzed = true;
    }

    private static InternalTypeSymbol GetOrCreateType(
        INamedTypeSymbol roslynType,
        InternalTypeSymbol? containingType,
        AnalysisPhase phase,
        InternalAnalysisContext context)
    {
        if (context.Data.TypesByRoslynType.TryGetValue(roslynType, out var type))
        {
            return type;
        }

        return CreateType(roslynType, containingType, phase, context, false);
    }

    private static InternalTypeSymbol CreateType(
        INamedTypeSymbol roslynType,
        InternalTypeSymbol? containingType,
        AnalysisPhase phase,
        InternalAnalysisContext context,
        bool isADataClassFromMetadata)
    {
        var type = new InternalTypeSymbol(roslynType, containingType, context);

        foreach (var roslynFeature in roslynType.GetMembers())
        {
            InternalFeatureSymbol feature;
            if (roslynFeature is IPropertySymbol roslynProperty)
            {
                feature = new InternalPropertySymbol(roslynProperty, context);
            }
            else if (roslynFeature is IFieldSymbol roslynField)
            {
                feature = new InternalFieldSymbol(roslynField, context);
            }
            else if (roslynFeature is IMethodSymbol roslynMethod)
            {
                feature = new InternalMethodSymbol(roslynMethod, context);
            }
            else if (roslynFeature is IEventSymbol roslynEvent)
            {
                feature = new InternalEventSymbol(roslynEvent, context);
            }
            else
            {
                continue;
            }

            type.SourceDeclaredFeatures.Add(feature);
        }

        switch (phase)
        {
            case AnalysisPhase.TypeInitializationPhase1:
                context.Data.TypesCreatedDuringTypeInitializationPhase1.Add(type);
                break;
            case AnalysisPhase.TypeInitializationPhase2:
                if ((!roslynType.ContainingAssembly.CorrectEquals(context.Compilation.Assembly)
                     && !isADataClassFromMetadata)
                    || roslynType.IsConstructedGenericTypeOrWithinConstructedGenericType())
                {
                    InitializeTypeBaseTypesAndColonSpecifiedInterfaces(type, context);

                    if (roslynType.TypeKind is TypeKind.Interface)
                    {
                        var roslynDataClass = type.RoslynSymbol
                            .GetTypeMembers()
                            .FirstOrDefault(t => t.Name == InterfaceDataClassName);
                        if (roslynDataClass is not null)
                        {
                            var dataClass = CreateType(
                                roslynDataClass,
                                type,
                                AnalysisPhase.TypeInitializationPhase2,
                                context,
                                true);
                            dataClass.IsDataClass = true;
                            dataClass.ConstructorIfDataClass = dataClass.DeclaredConstructors.First();

                            type.DataClass = dataClass;
                            // We could also add it to the type's DeclaredTypes list for consistency,
                            // but there's no reason to
                        }
                    }
                }

                context.Data.TypesCreatedDuringTypeInitializationPhase2.Add(type);
                break;
        };

        context.Data.TypesByRoslynType[roslynType] = type;

        return type;
    }
}
