using Franzo.Essentials.Collections;
using Franzo.Essentials.Roslyn;
using Microsoft.CodeAnalysis;

namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal class InternalTypeSymbol : InternalSymbol, IInternalTypeSymbol
{
    public readonly InternalTypeSymbol? ContainingType;
    public InternalTypeSymbol? BaseType = null;
    public bool? CachedAreSelfAndContainingTypesPartiallyDeclared = null;
    //public bool HasPhase2Initialized = false;
    //public bool HasInitializedBaseTypesAndInterfaces = false;
    //public bool HasPhase1Analyzed = false;
    public List<InternalTypeSymbol> DeclaredTypes = new();
    public List<InternalTypeSymbol> ColonSpecifiedDirectInterfaces = new();
    public List<InternalTypeSymbol> InterfacesList = new();
    public bool HasComputedInterfacesList = false;
    public object InterfacesListComputationLock = new();
    public List<InternalFeatureSymbol> SourceDeclaredFeatures = new();
    public InternalTypeSymbol? DataClass = null;
    public bool IsDataClass = false;
    public InternalMethodSymbol? ConstructorIfDataClass = null;
    public bool EmitDefaultConstructor = false;
    public bool DoNotGenerateInheritances = false;

    public IEnumerable<InternalFeatureSymbol> DeclaredFeatures
    {
        get
        {
            IEnumerable<InternalFeatureSymbol> result = SourceDeclaredFeatures;
            if (DataClass is not null
                && RoslynSymbol.ContainingAssembly.CorrectEquals(Context.Compilation.Assembly))
            {
                result = result
                    .Concat(DataClass.DeclaredProperties.Where(p => !p.RoslynSymbol.IsIndexer))
                    .Concat(DataClass.DeclaredEvents);
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
        get => ColonSpecifiedDirectInterfaces; //AttributeSpecifiedDirectInterfaces.Concat(ColonSpecifiedDirectInterfaces);
    }

    public IEnumerable<InternalTypeSymbol> Interfaces
    {
        get
        {
            ComputeInterfacesList();
            return InterfacesList;
            //DirectInterfaces.Concat(DirectInterfaces.SelectMany(i => i.Interfaces)).Distinct();
        }
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

    public InternalTypeSymbol AnchorType
    {
        get => IsDataClass ? ContainingType! : this;
    }

    public bool HasExplicitlyDeclaredConstructor
    {
        get => DeclaredConstructors.Any(c => !c.RoslynSymbol.IsImplicitlyDeclared);
    }

    public InternalTypeSymbol(
        INamedTypeSymbol roslynSymbol,
        InternalTypeSymbol? containingType,
        InternalAnalysisContext cxt)
        : base(roslynSymbol, cxt)
    {
        ContainingType = containingType;
    }

    public bool AreSelfAndContainingTypesPartiallyDeclared()
    {
        return CachedAreSelfAndContainingTypesPartiallyDeclared
            ?? RoslynSymbol.AreSelfAndContainingTypesPartiallyDeclared();
    }

    private void ComputeInterfacesList()
    {
        lock (InterfacesListComputationLock)
        {
            if (HasComputedInterfacesList)
            {
                return;
            }

            ComputeInterfacesListCore();
            HasComputedInterfacesList = true;
        }
    }

    private void ComputeInterfacesListCore()
    {
        var encounteredInterfaces = new HashSet<InternalTypeSymbol>();
        var queue = new Queue<InternalTypeSymbol>();
        queue.EnqueueRange(DirectInterfaces);
        while (queue.Count > 0)
        {
            var @interface = queue.Dequeue();
            if (encounteredInterfaces.Contains(@interface))
            {
                continue;
            }

            InterfacesList.Add(@interface);
            queue.EnqueueRange(@interface.DirectInterfaces);
            encounteredInterfaces.Add(@interface);
        }

        /*var encounteredInterfaces = new HashSet<InternalTypeSymbol>();
        foreach (var directInterface in DirectInterfaces)
        {
            foreach (var @interface in directInterface.SelfAndInterfaces)
            {
                if (encounteredInterfaces.Contains(@interface))
                {
                    continue;
                }

                InterfacesList.Add(@interface);
                encounteredInterfaces.Add(@interface);
            }
        }*/
    }
}
