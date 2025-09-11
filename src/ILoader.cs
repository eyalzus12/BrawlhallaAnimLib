using System.Threading.Tasks;
using SwfLib.Tags;
using BrawlhallaAnimLib.Anm;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public interface ILoader
{
    // swf
    bool SwfExists(string swfPath);
    ValueTask<ushort?> GetSymbolId(string swfPath, string symbolName);
    ValueTask<SwfTagBase?> GetTag(string swfPath, ushort tagId);
    ValueTask<uint[]?> GetScriptAVar(string swfPath, string spriteName);

    // sprite data
    ValueTask<ISpriteData?> GetSpriteData(string boneName, string setName);

    // anm
    ValueTask<IAnmClass?> GetAnmClass(string classIdentifier);

    // bone data
    ValueTask<string?> GetBoneName(short boneId);
    ValueTask<string?> GetBoneFilePath(string boneName);
    ValueTask<IBoneDatabase> GetBoneDatabase();
}