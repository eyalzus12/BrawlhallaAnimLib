using System;

namespace BrawlhallaAnimLib.Gfx;

public static class GfxExtensions
{
    private static readonly string[] MouthOverrides;
    private static readonly string[] EyesOverrides;

    static GfxExtensions()
    {
        int mouthOverrideCount = 1 + (int)GfxMouthOverride.Neutral;
        MouthOverrides = new string[mouthOverrideCount];
        MouthOverrides[(int)GfxMouthOverride.Warcry] = "a_MouthWarCry";
        MouthOverrides[(int)GfxMouthOverride.Smile] = "a_MouthSmile";
        MouthOverrides[(int)GfxMouthOverride.KO] = "a_MouthKO";
        MouthOverrides[(int)GfxMouthOverride.Hit] = "a_MouthHit";
        MouthOverrides[(int)GfxMouthOverride.Growl] = "a_MouthGrowl";
        MouthOverrides[(int)GfxMouthOverride.Whistle] = "a_MouthBlow";
        MouthOverrides[(int)GfxMouthOverride.Neutral] = "a_Mouth";

        int eyeOverrideCount = 1 + (int)GfxEyesOverride.Neutral;
        EyesOverrides = new string[eyeOverrideCount];
        EyesOverrides[(int)GfxEyesOverride.LookSide] = "a_EyesTurn";
        EyesOverrides[(int)GfxEyesOverride.KO] = "a_EyesKO";
        EyesOverrides[(int)GfxEyesOverride.Hit] = "a_EyesHit";
        EyesOverrides[(int)GfxEyesOverride.LookDown] = "a_EyesDown";
        EyesOverrides[(int)GfxEyesOverride.Angry] = "a_EyesAngry";
        EyesOverrides[(int)GfxEyesOverride.Neutral] = "a_Eyes";
    }

    public static IGfxType WithMouthOverride(this IGfxType gfx, GfxMouthOverride mouthOverride)
    {
        if (mouthOverride == GfxMouthOverride.NoChange) return gfx;

        int overrideIndex = (int)mouthOverride;
        if (overrideIndex < 0 || overrideIndex >= MouthOverrides.Length)
            throw new ArgumentException($"Invalid mouth override enum value {mouthOverride}");
        string newMouth = MouthOverrides[overrideIndex];

        InternalGfxImpl newGfx = new(gfx);
        // start from 1 to skip GfxMouthOverride.NoChange
        for (int i = 1; i < MouthOverrides.Length; ++i)
        {
            string mouth = MouthOverrides[i];
            newGfx.BoneOverride[mouth] = newMouth;
        }
        return newGfx;
    }

    public static IGfxType WithEyesOverride(this IGfxType gfx, GfxEyesOverride eyesOverride)
    {
        if (eyesOverride == GfxEyesOverride.NoChange) return gfx;

        int overrideIndex = (int)eyesOverride;
        if (overrideIndex < 0 || overrideIndex >= EyesOverrides.Length)
            throw new ArgumentException($"Invalid eyes override enum value {eyesOverride}");
        string newEyes = EyesOverrides[overrideIndex];

        InternalGfxImpl newGfx = new(gfx);
        // start from 1 to skip GfxEyesOverride.NoChange
        for (int i = 1; i < EyesOverrides.Length; ++i)
        {
            string eyes = EyesOverrides[i];
            newGfx.BoneOverride[eyes] = newEyes;
        }
        return newGfx;
    }
}