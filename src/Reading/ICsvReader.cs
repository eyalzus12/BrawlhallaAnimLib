using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Reading;

public interface ICsvReader
{
    bool TryGetCol(string key, [MaybeNullWhen(false)] out ICsvRow row);
}