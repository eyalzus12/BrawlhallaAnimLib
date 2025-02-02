using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Loading.BoneData;

public interface IBoneDataLoader
{
    void LoadBoneTypes();
    bool IsBoneTypesLoaded();
    bool TryGetBoneName(short boneId, [NotNullWhen(true)] out string? boneName);
}