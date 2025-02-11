using System.Globalization;

namespace BrawlhallaAnimLib.Reading;

internal static class ParserUtils
{
    public static uint ParseHexString(string str)
    {
        if (uint.TryParse(str, NumberStyles.HexNumber, null, out uint result))
            return result;
        return 0;
    }
}