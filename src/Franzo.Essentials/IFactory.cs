namespace Franzo.Essentials;

[Obsolete]
public interface IFactory<out TProduct>
{
    TProduct Create();
}

[Obsolete]
public interface IFactory<out TProduct, in TParam0>
{
    TProduct Create(TParam0 param0);
}
