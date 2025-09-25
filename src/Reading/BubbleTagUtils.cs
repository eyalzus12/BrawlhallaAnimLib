using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public enum BubbleTagTeamEnum
{
    Red,
    Blue,
}

public static class BubbleTagUtils
{
    public static ICustomArt? GetBubbleCustomArt(BubbleTagTeamEnum team)
    {
        if (team != BubbleTagTeamEnum.Blue) return null;

        return new InternalCustomArtImpl()
        {
            FileName = "Animation_GameModes.swf",
            Name = "SwapBlueBubble",
        };
    }
}