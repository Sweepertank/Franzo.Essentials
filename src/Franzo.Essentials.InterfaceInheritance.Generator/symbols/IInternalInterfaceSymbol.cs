namespace Franzo.Essentials.InterfaceInheritance.Generator;

internal interface IInternalInterfaceSymbol : IInternalTypeSymbol
{
    public string FakeDataFieldName { get; }
    public string RealDataFieldName { get; }
}
