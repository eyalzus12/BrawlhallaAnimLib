using System.Collections.Generic;

namespace BrawlhallaAnimLib.Anm;

public interface IAnmFrame
{
    IEnumerable<IAnmBone> Bones { get; }
}