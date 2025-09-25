using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public enum VolleyBattleTeamEnum
{
    None,
    White,
    Red,
    Blue,
}

public static class VolleyBattleUtils
{
    public static ICustomArt? GetBallCustomArt(VolleyBattleTeamEnum team, int number)
    {
        string? colorName = team switch
        {
            VolleyBattleTeamEnum.White => "White",
            VolleyBattleTeamEnum.Red => "Red",
            VolleyBattleTeamEnum.Blue => "Blue",
            _ => null,
        };
        if (colorName is null) return null;

        return new InternalCustomArtImpl()
        {
            FileName = "Gfx_Gamemodes.swf",
            Name = $"VolleyBall{colorName}{number}",
        };
    }
}