using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public enum HordeTypeEnum
{
    Standard,
    Nightmare,
}

public static class HordeUtils
{
    public static ICustomArt? GetDemonCustomArt(HordeTypeEnum type)
    {
        if (type != HordeTypeEnum.Nightmare) return null;

        return new InternalCustomArtImpl()
        {
            FileName = "Animation_GameModes.swf",
            Name = "SwapGargoyle",
        };
    }
}