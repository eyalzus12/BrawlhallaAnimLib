using System.Diagnostics.CodeAnalysis;
using SwfLib.Tags;

namespace BrawlhallaAnimLib.Loading;

public interface ISwfLoader
{
    void LoadSwf(string swfPath);
    bool IsSwfLoaded(string swfPath);
    bool TryGetSymbolId(string swfPath, string symbolName, out ushort symbolId);
    bool TryGetTag(string swfPath, ushort tagId, [NotNullWhen(true)] out SwfTagBase? tag);
}