namespace BrawlhallaAnimLib.Anm;

public interface IAnmAnimation
{
    uint LoopStart { get; }
    uint RecoveryStart { get; }
    uint FreeStart { get; }
    uint PreviewFrame { get; }
    uint BaseStart { get; }
    uint[] RunEndFrames { get; }
    IAnmFrame[] Frames { get; }
}