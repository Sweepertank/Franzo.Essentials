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

    public partial class Data_
    {
        public int DatafulProperty { get; set; }
    }
}

public partial class Yorp : IDataless, IDataful
{
    public Yorp()
    {
        DatafulData = ConstructDataful();

        DatalessMethod();
    }
}
