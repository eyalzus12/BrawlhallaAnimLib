using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public interface IBoneDatabase
{
    bool TryGetArtType(string boneName, out ArtTypeEnum artType);
    bool TryGetBoneType(string boneName, out BoneTypeEnum boneType, out bool boneDir);
    bool HasVariantFor(string boneName, BoneTypeEnum boneType);
    bool IsVariantFor(string boneName, BoneTypeEnum boneType);
    bool TryGetAsymSwap(string boneName, [MaybeNullWhen(false)] out string asymBoneName);
}