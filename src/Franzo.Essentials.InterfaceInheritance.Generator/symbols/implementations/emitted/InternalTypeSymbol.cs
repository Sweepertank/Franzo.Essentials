using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class InternalTypeSymbol : InternalSymbol, IInternalTypeSymbol
{
    public readonly InternalTypeSymbol? ContainingType;
    public InternalTypeSymbol? BaseType = null;
    public bool? CachedAreSelfAndContainingTypesPartiallyDeclared = null;
    public bool HasPhase2Initialized = false;
    //public bool HasInitializedBaseTypesAndInterfaces = false;
    public bool HasPhase1Analyzed = false;
    public List<InternalTypeSymbol> DeclaredTypes = new();
    public List<InternalTypeSymbol> AttributeSpecifiedDirectInterfaces = new();
    public List<InternalTypeSymbol> ColonSpecifiedDirectInterfaces = new();
    public List<InternalFeatureSymbol> SourceDeclaredFeatures = new();
    public InternalTypeSymbol? DataClass = null;
    public bool IsDataClass = false;
    public InternalMethodSymbol? ConstructorIfDataClass = null;

    public IEnumerable<InternalFeatureSymbol> DeclaredFeatures
    {
        get
        {
            IEnumerable<InternalFeatureSymbol> result = SourceDeclaredFeatures;
            if (DataClass is not null)
            {
                result = result.Concat(
                    DataClass.DeclaredProperties.Where(p => !p.RoslynSymbol.IsIndexer));
            }

            return result;
        }
    }

    public IEnumerable<InternalPropertySymbol> DeclaredProperties
    {
        get => DeclaredFeatures.OfType<InternalPropertySymbol>();
    }

    public IEnumerable<InternalEventSymbol> DeclaredEvents
    {
        get => DeclaredFeatures.OfType<InternalEventSymbol>();
    }

    public IEnumerable<InternalMethodSymbol> DeclaredMethods
    {
        get => DeclaredFeatures.OfType<InternalMethodSymbol>();
    }

    public IEnumerable<InternalMethodSymbol> DeclaredConstructors
    {
        get => DeclaredMethods.Where(m => m.RoslynSymbol.MethodKind is MethodKind.Constructor);
    }

    public new INamedTypeSymbol RoslynSymbol
    {
        get => (INamedTypeSymbol)base.RoslynSymbol;
    }

    public IEnumerable<InternalTypeSymbol> DirectInterfaces
    {
        get => AttributeSpecifiedDirectInterfaces.Concat(ColonSpecifiedDirectInterfaces);
    }

    public IEnumerable<InternalTypeSymbol> Interfaces
    {
        get => DirectInterfaces.Concat(DirectInterfaces.SelectMany(i => i.Interfaces)).Distinct();
    }

    public IEnumerable<InternalTypeSymbol> SelfAndInterfaces
    {
        get
        {
            yield return this;
            foreach (var @interface in Interfaces)
            {
                yield return @interface;
            }
        }
    }

    public IEnumerable<InternalTypeSymbol> InterfacesInheriting
    {
        get => AttributeSpecifiedDirectInterfaces.Concat(
            AttributeSpecifiedDirectInterfaces.SelectMany(i => i.Interfaces)).Distinct();
    }

    public InternalTypeSymbol(INamedTypeSymbol roslynSymbol, InternalTypeSymbol? containingType)
        : base(roslynSymbol)
    {
        ContainingType = containingType;
    }

    public bool AreSelfAndContainingTypesPartiallyDeclared()
    {
        return CachedAreSelfAndContainingTypesPartiallyDeclared
            ?? RoslynSymbol.AreSelfAndContainingTypesPartiallyDeclared();
    }
}
