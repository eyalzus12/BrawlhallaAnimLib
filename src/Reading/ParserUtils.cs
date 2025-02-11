using System;
using System.Globalization;

namespace BrawlhallaAnimLib.Reading;

public static class ParserUtils
{
    public static uint ParseHexString(string str)
    {
        if (!str.StartsWith("0x") && !str.StartsWith("0X")) return 0;
        if (!uint.TryParse(str.AsSpan()[2..], NumberStyles.HexNumber, null, out uint result)) return 0;

        return result;
    }
}