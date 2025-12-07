using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

public partial class InterfaceInheritanceGenerator : IIncrementalGenerator
{
    internal static void Analyze(InternalAnalysisContext cxt)
    {
        // InterfacesDeclaringShadowingMembers
        // @todo: bring down documentation, use attributes
        // @todo: maybe multithread? at least emitting?

        InitializeTypes_Phase1(cxt);
        InitializeTypes_Phase2(cxt);
        AnalyzeTypes_Phase1(cxt);
    }

    private static void InitializeTypes_Phase1(InternalAnalysisContext cxt)
    {
        Parallel.ForEach(
            cxt.PossiblyRelevantTopLevelRoslynTypes,
            /*new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1
            },*/
            (roslynType, state, _) =>
            {
                if (cxt.CancellationToken.IsCancellationRequested)
                {
                    state.Stop();
                }

                //var x = roslynType.IsConstructedGenericType();
                var type = CreateTypeIfEmitting(roslynType, null, cxt);
                if (type is null) return;

                lock (cxt.Data.RootTypesToEmitLock)
                {
                    cxt.Data.RootTypesToEmit.Add(type);
                }
            });

        cxt.CancellationToken.ThrowIfCancellationRequested();
    }

    private static InternalTypeSymbol? CreateTypeIfEmitting(
        INamedTypeSymbol roslynType,
        InternalTypeSymbol? containingType,
        InternalAnalysisContext cxt)
    {
        var type = CreateType(
            roslynType,
            containingType,
            AnalysisPhase.TypeInitializationPhase1,
            cxt,
            false);

        foreach (var roslynDeclaredType in roslynType.GetTypeMembers())
        {
            var declaredType = CreateTypeIfEmitting(
                roslynDeclaredType,
                type,
                cxt);
            if (declaredType is null) continue;

            type.DeclaredTypes.Add(declaredType);
        }

        if (type.DeclaredTypes.Count == 0 && !roslynType.IsPartiallyDeclared())
        {
            return null;
        }

        return type;
    }

    private static void InitializeTypes_Phase2(InternalAnalysisContext cxt)
    {
        Parallel.ForEach(
            cxt.Data.TypesCreatedDuringTypeInitializationPhase1,
            /*new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1
            },*/
            (type, state, _) =>
            {
                if (cxt.CancellationToken.IsCancellationRequested)
                {
                    state.Stop();
                }

                InitializeType_Phase2(type, cxt);
            });

        cxt.CancellationToken.ThrowIfCancellationRequested();
    }

    private static void InitializeType_Phase2(
        InternalTypeSymbol type,
        InternalAnalysisContext cxt)
    {
        //if (type.HasPhase2Initialized)
        //{
        //    return;
        //}

        InitializeTypeBaseTypesAndColonSpecifiedInterfaces(type, cxt);

        if (type.RoslynSymbol.HasAttribute<DoNotGenerateInheritancesAttribute>())
        {
            type.DoNotGenerateInheritances = true;
        }

        if (type.RoslynSymbol.Name == InterfaceDataClassName
            && type.ContainingType?.RoslynSymbol.TypeKind is TypeKind.Interface)
        {
            var invalid = false;

            if (type.RoslynSymbol.TypeKind is not TypeKind.Class)
            {
                cxt.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.InterfaceDataClassMustBeClass,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            // Check this *instead* of IsGenericType, because we don't want this to pass for
            // a class without generic parameters nested inside a class *with* generic parameters
            if (type.RoslynSymbol.TypeParameters.Length > 0)
            {
                cxt.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.InterfaceDataClassCannotHaveTypeParameters,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (type.RoslynSymbol.DeclaredAccessibility is not Accessibility.Public)
            {
                cxt.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.InterfaceDataClassMustBePublic,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            if (!type.AreSelfAndContainingTypesPartiallyDeclared())
            {
                invalid = true;
            }

            var publicConstructors = type.DeclaredConstructors.Where(
                m => m.RoslynSymbol.DeclaredAccessibility is Accessibility.Public);
            if (publicConstructors.Count() != 1)
            {
                cxt.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.InterfaceDataClassMustDeclareExactlyOnePublicConstructor,
                        type.RoslynSymbol.Locations.First()));
                invalid = true;
            }

            var publicConstructor = publicConstructors.FirstOrDefault();

            if (!invalid)
            {
                type.IsDataClass = true;
                type.ConstructorIfDataClass = publicConstructor;
                type.ContainingType!.DataClass = type;
            }
        }

        //type.HasPhase2Initialized = true;
    }

    private static void InitializeTypeBaseTypesAndColonSpecifiedInterfaces(
        InternalTypeSymbol type,
        InternalAnalysisContext cxt)
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
                cxt);
        }

        // This check is just an optimization -
        // it prevents us from loading in interfaces that we don't care about
        // (i.e. interfaces whose only connection to types that we *do* care about
        // is the fact that they're colon-specified by a class that we care about)
        // note: the above message is wrong and outdated
        //if (type.RoslynSymbol.TypeKind is TypeKind.Interface)
        //{
        foreach (var roslynInterface in type.RoslynSymbol.Interfaces)
        {
            var @interface = GetOrCreateType(
                roslynInterface,
                null,
                AnalysisPhase.TypeInitializationPhase2,
                cxt);
            type.ColonSpecifiedDirectInterfaces.Add(@interface);
        }
        //}

        //type.HasInitializedBaseTypesAndInterfaces = true;
    }

    private static void AnalyzeTypes_Phase1(InternalAnalysisContext cxt)
    {
        Parallel.ForEach(
            cxt.Data.Types,
            /*new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1
            },*/
            (type, state, _) =>
            {
                if (cxt.CancellationToken.IsCancellationRequested)
                {
                    state.Stop();
                }

                AnalyzeType_Phase1(type, cxt);
            });

        cxt.CancellationToken.ThrowIfCancellationRequested();
    }

    private static void AnalyzeType_Phase1(
        InternalTypeSymbol type,
        InternalAnalysisContext cxt)
    {
        /*if (type.HasPhase1Analyzed)
        {
            return;
        }*/

        foreach (var feature in type.DeclaredFeatures)
        {
            foreach (var @interface in type.Interfaces)
            {
                foreach (var interfaceFeature in @interface.DeclaredFeatures)
                {
                    if (feature.RoslynSymbol.MemberCollidesWith(interfaceFeature.RoslynSymbol)
                        && !feature.RoslynSymbol.IsImplicitlyDeclared)
                    {
                        // @todo: it would be even more "optimal" if we only add to this list
                        // if the feature not only collides with, but can implicitly implement interfaceFeature
                        // if this message is still here i forgot to remove it
                        if (interfaceFeature.RoslynSymbol.IsAbstract
                            && interfaceFeature.RoslynSymbol.DeclaredAccessibility is Accessibility.Public
                            && feature.RoslynSymbol.MemberCanImplicitlyImplement(interfaceFeature.RoslynSymbol))
                        {
                            feature.ImplicitlyImplementableShadowedPublicAbstractFeatures.Add(interfaceFeature);
                        }

                        feature.ShadowedFeatures.Add(interfaceFeature);
                        lock (interfaceFeature.TypesDeclaringShadowingFeaturesLock)
                        {
                            interfaceFeature.TypesDeclaringShadowingFeatures.Add(type);
                        }
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
                foreach (var @interface in type.ColonSpecifiedDirectInterfaces)
                {
                    if (@interface.DataClass is null)
                    {
                        continue;
                    }

                    cxt.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.NonDatafulInterfaceCannotImplementDatafulInterface,
                            type.RoslynSymbol.Locations.First(),
                            [type.RoslynSymbol, @interface.RoslynSymbol]));
                }
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
                var emitDefaultConstructor = true;

                if (type.RoslynSymbol.IsStatic)
                {
                    emitDefaultConstructor = false;
                }

                foreach (var @interface in type.AnchorType.DirectInterfaces)
                {
                    if (@interface.DataClass is not null
                        && !@interface.DataClass.ConstructorIfDataClass!.RoslynSymbol.CanBeCalledWithZeroArgumentsInSource())
                    {
                        cxt.ReportDiagnostic(
                            Diagnostic.Create(
                                DiagnosticDescriptors.TypeMustDeclareConstructor,
                                type.RoslynSymbol.Locations.First(),
                                [type.RoslynSymbol, @interface.RoslynSymbol]));
                        emitDefaultConstructor = false;
                    }
                }

                if (type.BaseType is not null)
                {
                    if (!type.BaseType.DeclaredConstructors.Any(
                        c => c.RoslynSymbol.IsAccessibleWithin(type.RoslynSymbol, cxt.Compilation)
                            && c.RoslynSymbol.CanBeCalledWithZeroArgumentsInSource()))
                    {
                        emitDefaultConstructor = false;
                    }
                }

                type.EmitDefaultConstructor = emitDefaultConstructor;
            }
        }

        /*if (type.BaseType is not null)
        {
            AnalyzeType_Phase1(type.BaseType, cxt);
        }

        foreach (var @interface in type.DirectInterfaces)
        {
            AnalyzeType_Phase1(@interface, cxt);
        }*/

        //type.HasPhase1Analyzed = true;
    }

    private static InternalTypeSymbol GetOrCreateType(
        INamedTypeSymbol roslynType,
        InternalTypeSymbol? containingType,
        AnalysisPhase phase,
        InternalAnalysisContext cxt)
    {
        lock (cxt.Data.TypeCreationLock)
        {
            if (cxt.Data.TypesByRoslynType.TryGetValue(roslynType, out var type))
            {
                return type;
            }

            return CreateType(roslynType, containingType, phase, cxt, false);
        }
    }

    private static InternalTypeSymbol CreateType(
        INamedTypeSymbol roslynType,
        InternalTypeSymbol? containingType,
        AnalysisPhase phase,
        InternalAnalysisContext cxt,
        bool isADataClassFromMetadata)
    {
        var type = new InternalTypeSymbol(roslynType, containingType, cxt);

        foreach (var roslynFeature in roslynType.GetMembers())
        {
            InternalFeatureSymbol feature;
            if (roslynFeature is IPropertySymbol roslynProperty)
            {
                feature = new InternalPropertySymbol(roslynProperty, cxt);
            }
            else if (roslynFeature is IFieldSymbol roslynField)
            {
                feature = new InternalFieldSymbol(roslynField, cxt);
            }
            else if (roslynFeature is IMethodSymbol roslynMethod)
            {
                feature = new InternalMethodSymbol(roslynMethod, cxt);
            }
            else if (roslynFeature is IEventSymbol roslynEvent)
            {
                feature = new InternalEventSymbol(roslynEvent, cxt);
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
                lock (cxt.Data.TypeCreationLock)
                {
                    cxt.Data.TypesCreatedDuringTypeInitializationPhase1.Add(type);
                }
                break;
            case AnalysisPhase.TypeInitializationPhase2:
                if ((!roslynType.ContainingAssembly.CorrectEquals(cxt.Compilation.Assembly)
                     && !isADataClassFromMetadata)
                    || roslynType.IsConstructedGenericTypeOrWithinConstructedGenericType())
                {
                    InitializeTypeBaseTypesAndColonSpecifiedInterfaces(type, cxt);

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
                                cxt,
                                true);
                            dataClass.IsDataClass = true;
                            dataClass.ConstructorIfDataClass = dataClass.DeclaredConstructors.First();

                            type.DataClass = dataClass;
                            // We could also add it to the type's DeclaredTypes list for consistency,
                            // but there's no reason to
                        }
                    }
                }

                lock (cxt.Data.TypeCreationLock)
                {
                    cxt.Data.TypesCreatedDuringTypeInitializationPhase2.Add(type);
                }
                break;
        }

        lock (cxt.Data.TypeCreationLock)
        {
            cxt.Data.TypesByRoslynType[roslynType] = type;
        }

        return type;
    }
}
