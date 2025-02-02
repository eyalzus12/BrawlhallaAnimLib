using System.Diagnostics.CodeAnalysis;
using WallyAnmSpinzor;

namespace BrawlhallaAnimLib.Loading;

public interface IAnmLoader
{
    void LoadAnm(string anmPath);
    bool IsAnmLoaded(string anmPath);
    bool TryGetAnmClass(string classIdentifier, [NotNullWhen(true)] out AnmClass? anmClass);
}