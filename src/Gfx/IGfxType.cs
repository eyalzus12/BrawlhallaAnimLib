using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Bones;

namespace BrawlhallaAnimLib.Gfx;

public interface IGfxType
{
    string AnimFile { get; }
    string AnimClass { get; }
    double AnimScale { get; }
    uint Tint { get; } // u24

    bool HasAsymmetrySwapFlag(BoneTypeEnum flag);

    IEnumerable<ICustomArt> CustomArts { get; }
    IEnumerable<IColorSwap> ColorSwaps { get; }

    // these are set in costumeTypes.csv.
    bool UseRightTorso { get; }
    bool UseRightJaw { get; }
    bool UseRightEyes { get; }
    bool UseRightMouth { get; }
    bool UseRightHair { get; }
    bool UseRightForearm { get; }
    bool UseRightShoulder1 { get; }
    bool UseRightLeg1 { get; }
    bool UseRightShin { get; }
    bool UseTrueLeftRightHands { get; }
    bool TryGetBoneOverride(string bone, [MaybeNullWhen(false)] out string newBone);
    // these are set in weaponSkinType.csv
    bool UseRightGauntlet { get; }
    bool UseRightKatar { get; }
    bool HideRightPistol2D { get; }
    // these are set in SpawnBotTypes.xml
    bool UseTrueLeftRightTorso { get; }
}