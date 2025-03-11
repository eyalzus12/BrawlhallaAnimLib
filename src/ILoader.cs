using System.Threading.Tasks;
using SwfLib.Tags;
using BrawlhallaAnimLib.Anm;

namespace BrawlhallaAnimLib;

public interface ILoader
{
    // swf
    bool SwfExists(string swfPath);
    Task<ushort?> GetSymbolId(string swfPath, string symbolName);
    Task<SwfTagBase?> GetTag(string swfPath, ushort tagId);
    Task<uint[]?> GetScriptAVar(string swfPath, string spriteName);

    // anm
    Task<IAnmClass?> GetAnmClass(string classIdentifier);

    // bone types
    Task<string?> GetBoneName(short boneId);
    Task<string?> GetBoneFilePath(string boneName);
}