namespace BrawlhallaAnimLib.Gfx;

public class CustomArt
{
    public bool Right { get; set; } = false;
    /*
    types:
    0 - none
    1 - weapon
    2 - costume
    3 - pickup (can't be set in xml)
    4 - flag? (can't be set in xml)
    5 - bot? (can't be set in xml)
    */
    public int Type { get; set; } = 0;
    public required string FileName { get; set; }
    public required string Name { get; set; }
}