using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Animation;

namespace BrawlhallaAnimLib.Gfx;

public interface IGfxType
{
    string AnimFile { get; set; }
    string AnimClass { get; set; }
    double AnimScale { get; set; }
    uint Tint { get; set; }

    bool HasAsymmetrySwapFlag(BoneTypeEnum flag);

    IEnumerable<ICustomArt> CustomArts();
    IEnumerable<IColorSwap> ColorSwaps();

    // these are set in costumeTypes.csv.
    bool UseRightTorso { get; set; }
    bool UseRightJaw { get; set; }
    bool UseRightEyes { get; set; }
    bool UseRightMouth { get; set; }
    bool UseRightHair { get; set; }
    bool UseRightForearm { get; set; }
    bool UseRightShoulder1 { get; set; }
    bool UseRightLeg1 { get; set; }
    bool UseRightShin { get; set; }
    bool UseTrueLeftRightHands { get; set; }
    bool TryGetBoneOverride(string bone, [MaybeNullWhen(false)] out string newBone);
    // these are set in weaponSkinType.csv
    bool UseRightGauntlet { get; set; }
    bool UseRightKatar { get; set; }
}