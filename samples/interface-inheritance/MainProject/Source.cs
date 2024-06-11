using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Franzo.Essentials.InterfaceInheritance;

namespace MainProject;

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
    public event EventHandler Corky
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    public void BloodyEllArry<T, U>(object s, [NotNull] int f) where T : class? where U : IIgnored, new()
    {

    }

    public int BloodyEllArry()
    {
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
    [InterfaceData]
    public new partial class Data_
    {
        public Data_(int foo, int bar)
        {
            //IEntityData = ConstructIEntity(foo);
            //IResourceData = ConstructIResource(bar);
        }
    }
}

[InheritInterface<IEntityResource>]
public partial class Greg
{
    public Greg()
    {
        //IEntityResourceData = ConstructIEntityResource(1, 2);
    }
}
