using System.Threading.Tasks;
using SwfLib.Tags;
using BrawlhallaAnimLib.Anm;

namespace BrawlhallaAnimLib;

public interface ILoader
{
    // swf
    bool SwfExists(string swfPath);
    ValueTask<ushort?> GetSymbolId(string swfPath, string symbolName);
    ValueTask<SwfTagBase?> GetTag(string swfPath, ushort tagId);
    ValueTask<uint[]?> GetScriptAVar(string swfPath, string spriteName);

    // anm
    ValueTask<IAnmClass?> GetAnmClass(string classIdentifier);

    // bone types
    ValueTask<string?> GetBoneName(short boneId);
    ValueTask<string?> GetBoneFilePath(string boneName);
}