using System.ComponentModel;

namespace Franzo.Essentials.ComponentModel;

public class NullTypeConverter : TypeConverter
{
    public static readonly NullTypeConverter Instance = new();
}
