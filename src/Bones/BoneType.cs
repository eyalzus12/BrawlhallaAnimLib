namespace BrawlhallaAnimLib.Bones;

// those with a _ prefix aren't settable in xml
public enum BoneTypeEnum: int
{
    HAND = 1,
    FOREARM = 2,
    ARM = 3,
    SHOULDER = 4,
    LEG = 5,
    SHIN = 6,
    FOOT = 7,
    _TORSO = 8,
    GAUNTLETHAND = 9,
    GAUNTLETFOREARM = 10,
    PISTOL = 11,
    KATAR = 12,
    _JAW = 13,
    _EYES = 14,
    _BOOTS = 15,
    _MOUTH = 16,
    _HAIR = 17,
}

internal readonly record struct BoneType(BoneTypeEnum Type, bool Dir);