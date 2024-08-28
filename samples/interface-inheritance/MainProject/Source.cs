namespace MainProject;

public partial interface IGeneric<T>
{
    public partial class Data_
    {
        public int Goofy { get; }

        public Data_()
        {
        }
    }
}

public partial interface IDerivedGeneric<T> : IGeneric<T>
{
    public new partial class Data_
    {
        public int Goofy2 { get; }
    }
}

public partial class Blurb : IDerivedGeneric<int>
{
    public Blurb()
    {
        DerivedGenericData = ConstructDerivedGeneric();
    }
}

/*[InheritInterface("IDerivedGeneric<T>")]
public partial class GenericClass<T> : Blurb
{
}

public interface IIgnored
{
}

public partial interface IDontHaveData : IIgnored
{
}

public partial interface IEntity
{
    public static readonly int Yeck = 5;
    public const bool Yeck2 = true;
    protected static readonly bool Yeck3 = false;
    protected const bool Yeck4 = false;
    public event EventHandler Corky
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    protected internal event EventHandler Worky
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    public void BloodyEllArry<T, U>(object? s, [NotNull] int f) where T : class? where U : IIgnored, new()
    {
        Bark2_Invoke(this, EventArgs.Empty);
    }

    public int BloodyEllArry()
    {
        Console.WriteLine("Bloody Ell Arry, it Worked!");
        return 0;
    }

    [InterfaceData]
    public partial class Data_
    {
        /// <summary>
        /// yuck
        /// </summary>
        [TypeConverter("heck")]
        public int Foo { get; set; }

        private int Bark { get; set; }

        public event EventHandler? GoopyFloop;

        private event EventHandler? Bark2;

        public Data_(int foo)
        {
            Foo = foo;
        }
    }
}

public partial interface IResource
{
    [InterfaceData]
    public partial class Data_
    {
        [TypeConverter("heck")]
        public int Bar { get; set; }

        public Data_(int bar)
        {
            Bar = bar;
        }
    }
}

public partial interface IEntityResource : IEntity, IResource
{
    protected int ProtectedProperty
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    protected int this[string? s, char y]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public sealed void TestProxies()
    {
        Gorbus = 5;
        Console.WriteLine(Gorbus);
        Chorbus = 6;
        Console.WriteLine(Chorbus);
    }

    protected void TestInheritances1()
    {
        Console.WriteLine("TestInheritances1");
    }

    protected static readonly string TestInheritances2 = "TestInheritances2";

    protected string TestInheritances3
    {
        get
        {
            Console.WriteLine("TestInheritances3 - get");
            return "";
        }
        private protected set
        {
            Console.WriteLine("TestInheritances3 - set");
        }
    }

    protected event EventHandler TestInheritances4
    {
        add => Console.WriteLine("TestInheritances4 - add");
        remove => Console.WriteLine("TestInheritances4 - remove");
    }

    public void TestInheritances5()
    {
        Console.WriteLine("TestInheritances5");
    }

    public static readonly string TestInheritances6 = "TestInheritances6";

    public string TestInheritances7
    {
        get
        {
            Console.WriteLine("TestInheritances7 - get");
            return "";
        }
        internal set
        {
            Console.WriteLine("TestInheritances7 - set");
        }
    }

    public event EventHandler TestInheritances8
    {
        add => Console.WriteLine("TestInheritances8 - add");
        remove => Console.WriteLine("TestInheritances8 - remove");
    }

    [InterfaceData]
    public new partial class Data_
    {
        private int Gorbus { get; set; }
        protected int Chorbus { get; set; }

        protected event EventHandler<int>? Yorkifier;

        public Data_(int foo, int bar)
        {
            EntityData = ConstructEntity(foo);
            ResourceData = ConstructResource(bar);
        }
    }

    public void HeckleBeckleA()
    {
        Yorkifier_Invoke(this, 0);
    }

    protected void HeckleBeckleB()
    {
    }

    protected void HeckleBeckleC<G>(int s, string b) where G : unmanaged
    {
    }
}

[InheritInterface<IEntityResource>]
[InheritInterface<IDataless>]
[InheritInterface<IDataful>]
public partial class Greg
{
    private int Goofus { get; set; }

    public Greg()
    {
        EntityResourceData = ConstructEntityResource(1, 2);
        DatafulData = ConstructDataful();

        TestProxies();

        TestInheritances1();
        Console.WriteLine(TestInheritances2);
        TestInheritances3 = TestInheritances3;
        TestInheritances4 += null!;
        TestInheritances4 -= null!;

        TestInheritances5();
        Console.WriteLine(TestInheritances6);
        TestInheritances7 = TestInheritances7;
        TestInheritances8 += null!;
        TestInheritances8 -= null!;

        DatalessMethod();
        DatafulMethod();
        DatafulProperty = DatafulProperty;

        BloodyEllArry();
    }
}

/*using Franzo.Essentials.InterfaceInheritance;

public partial interface IBaseCoolo
{
    public void BloodyEllArry2()
    {
        Console.WriteLine("Bloody Ell Arry, it Worked2!");
    }

    [InterfaceData]
    public partial class Data_
    {
        public Data_()
        {
        }
    }
}

public partial interface ICoolo : IBaseCoolo
{
    public void BloodyEllArry()
    {
        Console.WriteLine("Bloody Ell Arry, it Worked!");
    }

    public void York();

    [InterfaceData]
    public new partial class Data_
    {
        public Data_()
        {
            BaseCooloData = ConstructBaseCoolo();
        }
    }
}

[InheritInterface<ICoolo>]
public partial class Greg
{
    public Greg()
    {
        CooloData = ConstructCoolo();
    }

    public void York()
    {
    }
}*/
