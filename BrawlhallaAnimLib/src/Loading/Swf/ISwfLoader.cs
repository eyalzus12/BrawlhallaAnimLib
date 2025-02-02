namespace BrawlhallaAnimLib.Loading.Swf;

public interface ISwfLoader
{
    void LoadSwf(string swfPath);
    bool IsSwfLoaded(string swfPath);
    bool TryGetSymbolId(string swfPath, string symbolName, out ushort symbolId);
}