using System;
using System.Globalization;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public static class ParserUtils
{
    public static uint ParseHexString(string str)
    {
        if (!str.StartsWith("0x") && !str.StartsWith("0X")) return 0;
        if (!uint.TryParse(str.AsSpan()[2..], NumberStyles.HexNumber, null, out uint result)) return 0;

        return result;
    }

    internal static InternalCustomArtImpl ParseCustomArt(string value, bool grabType, ArtTypeEnum defaultType)
    {
        bool right = false;
        ArtTypeEnum type = defaultType;

        if (grabType)
        {
            string[] parts1 = value.Split(':', 2);
            if (parts1.Length > 1)
            {
                value = parts1[1];
                string info = parts1[0].ToUpperInvariant();
                if (info == "RIGHT") right = true;
                else if (info == "C") type = ArtTypeEnum.Costume;
                else if (info == "W") type = ArtTypeEnum.Weapon;
            }
        }

        string[] parts2 = value.Split('/');

        if (parts2.Length < 2)
            throw new ArgumentException($"Invalid CustomArt string {value}");

        string fileName = parts2[0];
        string name = parts2[1];

        return new()
        {
            Right = right,
            Type = type,
            FileName = fileName,
            Name = name,
        };
    }

    internal static InternalColorSwapImpl ParseColorSwap(string value, ArtTypeEnum artType)
    {
        string[] parts = value.Split('=');
        if (parts.Length != 2)
            throw new ArgumentException($"Invalid color swap string {value}");

        string oldColorString = parts[0];
        if (oldColorString[0] != '0')
            throw new NotImplementedException($"Color swap color must start with 0");
        uint oldColor = ParseHexString(oldColorString);

        string newColorString = parts[1];
        if (newColorString[0] != '0')
            throw new NotImplementedException($"Color swap color must start with 0");
        uint newColor = ParseHexString(newColorString);

        return new()
        {
            ArtType = artType,
            OldColor = oldColor,
            NewColor = newColor,
        };
    }

    internal static bool ParseBool(string value) => value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
}