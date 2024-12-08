namespace Franzo.Essentials;

public static class Int32Extensions
{
    public static string ToOrdinal(this int self)
    {
        // https://stackoverflow.com/questions/20156/is-there-an-easy-way-to-create-ordinals-in-c

        if (self < 0)
        {
            return self.ToString();
        }

        switch (self)
        {
            case 0:
                return "zeroth";
            case 1:
                return "first";
            case 2:
                return "second";
            case 3:
                return "third";
            case 4:
                return "fourth";
            case 5:
                return "fifth";
            case 6:
                return "sixth";
            case 7:
                return "seventh";
            case 8:
                return "eighth";
            case 9:
                return "ninth";
        }

        switch (self % 100)
        {
            case 11:
            case 12:
            case 13:
                return self + "th";
        }

        switch (self % 10)
        {
            case 1:
                return self + "st";
            case 2:
                return self + "nd";
            case 3:
                return self + "rd";
            default:
                return self + "th";
        }
    }
}
