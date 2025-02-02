using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Anm;

public interface IAnmClass
{
    bool TryGetAnimation(string animationName, [NotNullWhen(true)] out IAnmAnimation? animation);
}