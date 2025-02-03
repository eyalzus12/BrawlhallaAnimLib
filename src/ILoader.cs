using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Anm;
using SwfLib.Tags;

namespace BrawlhallaAnimLib;

public interface ILoader
{
    // swf
    void LoadSwf(string swfPath);
    bool IsSwfLoaded(string swfPath);
    bool TryGetSymbolId(string swfPath, string symbolName, out ushort symbolId);
    bool TryGetTag(string swfPath, ushort tagId, [NotNullWhen(true)] out SwfTagBase? tag);
    // anm
    void LoadAnm(string anmPath);
    bool IsAnmLoaded(string anmPath);
    bool TryGetAnmClass(string classIdentifier, [NotNullWhen(true)] out IAnmClass? anmClass);
    // bone types
    void LoadBoneTypes();
    bool IsBoneTypesLoaded();
    bool TryGetBoneName(short boneId, [NotNullWhen(true)] out string? boneName);
}