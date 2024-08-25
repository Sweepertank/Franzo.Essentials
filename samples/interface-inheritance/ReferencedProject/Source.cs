using Franzo.Essentials.InterfaceInheritance;

namespace ReferencedProject;

public partial interface IDataless
{
    public void DatalessMethod()
    {
    }
}

public partial interface IDataful
{
    public void DatafulMethod()
    {
    }

    [InterfaceData]
    public partial class Data_
    {
        public int DatafulProperty { get; set; }
    }
}

[InheritInterface<IDataless>]
[InheritInterface<IDataful>]
public partial class Yorp : IDataless, IDataful
{
    public Yorp()
    {
        DatafulData = ConstructDataful();

        DatalessMethod();
    }
}
