using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public static class CrateColorUtils
{
    public static IColorSwap GetCrateAColorSwap(uint color) => new InternalColorSwapImpl()
    {
        ArtType = ArtTypeEnum.Pickup,
        OldColor = 0x3CFFC4,
        NewColor = color,
    };

    public static IColorSwap GetCrateBColorSwap(uint color) => new InternalColorSwapImpl()
    {
        ArtType = ArtTypeEnum.Pickup,
        OldColor = 0xBEFFEA,
        NewColor = color,
    };
}