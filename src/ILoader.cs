using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Anm;
using SwfLib.Tags;

namespace BrawlhallaAnimLib;

public interface ILoader
{
    // swf
    bool SwfExists(string swfPath);
    bool LoadSwf(string swfPath);
    bool TryGetSymbolId(string swfPath, string symbolName, out ushort symbolId);
    bool TryGetTag(string swfPath, ushort tagId, [MaybeNullWhen(false)] out SwfTagBase tag);
    bool TryGetScriptAVar(string swfPath, string spriteName, [MaybeNullWhen(false)] out uint[] a);
    // anm
    bool LoadAnms();
    bool TryGetAnmClass(string classIdentifier, [MaybeNullWhen(false)] out IAnmClass anmClass);
    // bone types
    bool LoadBoneTypes();
    bool TryGetBoneName(short boneId, [MaybeNullWhen(false)] out string boneName);
    bool LoadBoneSources();
    bool TryGetBoneFilePath(string boneName, [MaybeNullWhen(false)] out string bonePath);
}