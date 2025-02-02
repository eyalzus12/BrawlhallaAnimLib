namespace BrawlhallaAnimLib.Gfx;

public interface ICustomArt
{
    bool Right { get; set; }

    /*
    types:
    0 - none
    1 - weapon
    2 - costume
    3 - pickup (can't be set in xml)
    4 - flag? (can't be set in xml)
    5 - bot? (can't be set in xml)
    */
    int Type { get; set; }

    string FileName { get; set; }
    string Name { get; set; }
}