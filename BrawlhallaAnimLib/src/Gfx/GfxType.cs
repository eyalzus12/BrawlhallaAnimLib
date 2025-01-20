using System.Collections.Generic;

namespace BrawlhallaAnimLib.Gfx;

public class Gfx
{
    public enum AsymmetrySwapFlagEnum
    {
        HAND = 1,
        FOREARM = 2,
        ARM = 3,
        SHOULDER = 4,
        LEG = 5,
        SHIN = 6,
        FOOT = 7,
        // 8 isn't settable through xml. it is TORSO.
        GAUNTLETHAND = 9,
        GAUNTLETFOREARM = 10,
        PISTOL = 11,
        KATAR = 12,
        // 13 isn't settable through xml. it is JAW.
        // 14 isn't settable through xml. it is EYES.
        // 15 isn't settable through xml. might be BOOTS.
        // 16 isn't settable through xml. it is MOUTH.
        // 17 isn't settable through xml. it is HAIR.
    }

    public string AnimFile { get; set; } = "";
    public string AnimClass { get; set; } = "a__Animation";
    public double AnimScale { get; set; } = 1;
    public double MoveAnimSpeed { get; set; } = 1;
    public string BaseAnim { get; set; } = "Ready";
    public string RunAnim { get; set; } = "Run";
    public bool FlipAnim { get; set; } = false;
    public bool FireAndForget { get; set; } = false;
    public bool RandomFrameStart { get; set; } = false;
    public bool Desynch { get; set; } = false; //yes it's actually called Desynch
    public bool IgnoreCachedWeapon { get; set; } = false;
    public uint Tint { get; set; } = 0; //packed
    public AsymmetrySwapFlagEnum[] AsymmetrySwapFlags { get; set; } = [];
    public CustomArt[] CustomArts { get; set; } = [];
    public ColorSwap[] ColorSwaps { get; set; } = [];

    // these are set in costumeTypes.csv.
    public bool UseRightTorso { get; set; } = false;
    public bool UseRightJaw { get; set; } = false;
    public bool UseRightEyes { get; set; } = false;
    public bool UseRightMouth { get; set; } = false;
    public bool UseRightHair { get; set; } = false;
    public bool UseRightForearm { get; set; } = false;
    public bool UseRightShoulder1 { get; set; } = false;
    public bool UseRightLeg1 { get; set; } = false;
    public bool UseRightShin { get; set; } = false;
    public bool UseTrueLeftRightHands { get; set; } = false;
    public Dictionary<string, string> BoneOverrides { get; set; } = [];
    // these are set in weaponSkinType.csv
    public bool UseRightGauntlet { get; set; } = false;
    public bool UseRightKatar { get; set; } = false;
}