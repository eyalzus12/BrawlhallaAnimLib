using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public static class ScoreFontUtils
{
    public static string[] KnownFonts { get; } = [
        "SwapWhite",
        "", // swap red
        "SwapBlue",
        "SwapCTF",
        "SwapSynthRed",
        "SwapSynthBlue",
        "SwapJotunRed",
        "SwapJotunBlue",
        "SwapZombie",
    ];

    public static ICustomArt? GetScoreFontCustomArt(string? font)
    {
        if (string.IsNullOrEmpty(font)) return null;

        return new InternalCustomArtImpl()
        {
            FileName = "Animation_GameModes.swf",
            Name = font,
        };
    }
}