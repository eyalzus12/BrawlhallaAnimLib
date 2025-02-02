namespace BrawlhallaAnimLib.Anm;

public interface IAnmAnimation
{
    uint BaseStart { get; }
    IAnmFrame[] Frames { get; }
}