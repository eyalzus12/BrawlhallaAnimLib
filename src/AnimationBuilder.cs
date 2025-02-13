using System;
using System.Collections.Generic;
using System.Linq;

using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;
using BrawlhallaAnimLib.Anm;

namespace BrawlhallaAnimLib;

public sealed class AnimationBuilder(ILoader loader)
{
    private static readonly string[] AnimationPrefixes = ["Animation_", "a_Animation_EB_", "a__LootBox", "a__PodiumRig"];
    private static bool IsAnmAnimation(string path)
    {
        return path == "UI_TooltipAnimations.swf" || AnimationPrefixes.Any(path.StartsWith);
    }

    // null if not loaded yet
    public BoneSpriteWithName[]? BuildAnim(IGfxType gfx, string animName, long frame, Transform2D transform, bool isTooltip = false)
    {
        transform *= Transform2D.CreateScale(gfx.AnimScale, gfx.AnimScale);

        // anm animation
        if (IsAnmAnimation(gfx.AnimFile))
        {
            if (!loader.TryGetAnmClass($"{gfx.AnimFile}/{gfx.AnimClass}", out IAnmClass? anmClass))
            {
                return null;
                //throw new ArgumentException($"Could not find anim class {gfx.AnimClass} in {gfx.AnimFile}. Make sure you loaded it.");
            }
            if (!anmClass.TryGetAnimation(animName, out IAnmAnimation? animation))
                throw new ArgumentException($"No animation {animName} in anim class {gfx.AnimClass}");
            if (animation.Frames.Length == 0)
                throw new ArgumentException($"Animation {animName} has no frames");
            long frameIndex = MathUtils.SafeMod(frame + animation.BaseStart, animation.Frames.Length);
            IAnmFrame anmFrame = animation.Frames[frameIndex];

            List<BoneInstance>? bones = GetBoneInstances(anmFrame.Bones, gfx);
            if (bones is null) return null;

            SetAsymBonesVisibility(bones, gfx, transform.ScaleX * transform.ScaleY < 0, isTooltip);

            List<BoneSpriteWithName> result = [];
            foreach (BoneInstance instance in bones)
            {
                if (!instance.Visible)
                    continue;

                IAnmBone bone = instance.Bone;
                Transform2D boneTransform = new(bone.ScaleX, bone.RotateSkew1, bone.RotateSkew0, bone.ScaleY, bone.X, bone.Y);

                BoneSpriteWithName boneSprite = new()
                {
                    SwfFilePath = instance.FilePath,
                    SpriteName = instance.SpriteName,
                    AnimScale = gfx.AnimScale,
                    Transform = transform * boneTransform,
                    Tint = gfx.Tint,
                    Opacity = bone.Opacity,
                };

                if (!BuildColorMap(boneSprite, instance, gfx.ColorSwaps))
                    return null;

                result.Add(boneSprite);
            }
            return [.. result];
        }
        // swf animation
        else
        {
            BoneInstance fakeInstance = new()
            {
                FilePath = gfx.AnimFile,
                OgBoneName = null!,
                SpriteName = gfx.AnimClass,
                Bone = null!,
                Visible = true,
            };
            BoneSpriteWithName boneSprite = new()
            {
                SwfFilePath = gfx.AnimFile,
                SpriteName = gfx.AnimClass,
                AnimScale = gfx.AnimScale,
                Transform = transform,
                Tint = gfx.Tint,
                Opacity = 1,
            };
            if (!BuildColorMap(boneSprite, fakeInstance, gfx.ColorSwaps)) return null;

            return [boneSprite];
        }
    }

    private static readonly HashSet<BoneTypeEnum> MirroredBoneTypes = [
        BoneTypeEnum.HAND,
        BoneTypeEnum._TORSO,
        BoneTypeEnum.GAUNTLETHAND,
        BoneTypeEnum._JAW,
        BoneTypeEnum._EYES,
        BoneTypeEnum._BOOTS,
        BoneTypeEnum._MOUTH,
        BoneTypeEnum._HAIR,
    ];

    private List<BoneInstance>? GetBoneInstances(IAnmBone[] bones, IGfxType gfx)
    {
        if (!loader.LoadBoneTypes())
            return null;

        List<BoneInstance> instances = [];
        bool otherHand = false;
        string handBoneName = "";
        foreach (IAnmBone bone in bones)
        {
            if (!loader.TryGetBoneName(bone.Id, out string? boneName))
                throw new ArgumentException($"Could not find bone name for id {bone.Id}");

            BoneType? boneType;
            if (BoneDatabase.BoneTypeDict.TryGetValue(boneName, out BoneType boneType_))
                boneType = boneType_;
            else
                boneType = null;

            bool mirrored;
            if (boneType is null)
            {
                mirrored = false;
            }
            else
            {
                if (MirroredBoneTypes.Contains(boneType.Value.Type))
                {
                    double det = bone.ScaleX * bone.ScaleY - bone.RotateSkew0 * bone.RotateSkew1;
                    mirrored = (det < 0) != boneType.Value.Dir;
                }
                else
                    mirrored = false;
            }

            string finalBoneName = boneName;
            if (gfx.TryGetBoneOverride(boneName, out string? overridenBoneName))
            {
                finalBoneName = overridenBoneName;
            }
            else if (
                BoneDatabase.AsymSwapDict.TryGetValue(boneName, out string? otherBoneName) &&
                (boneType is null || !gfx.HasAsymmetrySwapFlag(boneType.Value.Type))
            )
            {
                finalBoneName = otherBoneName;
            }

            bool isHand = boneType is not null && boneType.Value.Type == BoneTypeEnum.HAND;
            bool right = isHand && (otherHand ? !mirrored : mirrored);
            bool isOtherHand = false;
            if (isHand)
            {
                isOtherHand = otherHand && handBoneName == finalBoneName;
                handBoneName = isOtherHand ? "" : finalBoneName;
                otherHand = !otherHand;
            }
            else
            {
                otherHand = false;
                handBoneName = "";
            }

            if (!FindCustomArt(boneName, finalBoneName, gfx.CustomArts, right, out ICustomArt? customArt))
                return null;

            string customArtSuffix = customArt is not null ? $"_{customArt.Name}" : "";
            bool visible = boneType switch
            {
                null => true,
                _ => boneType.Value.Type switch
                {
                    BoneTypeEnum.HAND when isOtherHand => false,
                    BoneTypeEnum.FOREARM when BoneDatabase.ForearmVariantDict.ContainsValue(finalBoneName) => false,
                    BoneTypeEnum.SHOULDER when finalBoneName == "a_Shoulder1R" || finalBoneName == "a_Shoulder1RightR" => false,
                    BoneTypeEnum.LEG when finalBoneName == "a_Leg1R" || finalBoneName == "a_Leg1RightR" => false,
                    BoneTypeEnum.SHIN when BoneDatabase.ShinVariantDict.ContainsValue(finalBoneName) => false,
                    BoneTypeEnum._TORSO when finalBoneName == "a_Torso1R" || finalBoneName == "a_BotTorsoR" => false,
                    BoneTypeEnum.GAUNTLETFOREARM when finalBoneName == "a_WeaponFistsForearmR" || finalBoneName == "a_WeaponFistsForearmRightR" => false,
                    BoneTypeEnum.KATAR when BoneDatabase.KatarVariantDict.ContainsValue(finalBoneName) => false,
                    _ => true,
                }
            };

            string trueSpriteName = finalBoneName + customArtSuffix;
            string trueFilePath = customArt?.FileName ?? gfx.AnimFile;
            if (!loader.LoadBoneSources())
                return null;
            if (loader.TryGetBoneFilePath(trueSpriteName, out string? boneSource))
                trueFilePath = boneSource;

            if (!loader.SwfExists(trueFilePath))
                continue;

            instances.Add(new()
            {
                FilePath = trueFilePath,
                SpriteName = trueSpriteName,
                OgBoneName = boneName,
                Bone = bone,
                Visible = visible,
            });
        }
        return instances;
    }

    // returns false if swf not loaded yet
    private bool FindCustomArt(string ogBoneName, string boneName, IEnumerable<ICustomArt> customArts, bool right, out ICustomArt? chosen)
    {
        chosen = null;

        ArtTypeEnum artType = BoneDatabase.ArtTypeDict.GetValueOrDefault(ogBoneName, ArtTypeEnum.None);
        foreach (ICustomArt ca in customArts.Reverse())
        {
            bool rightMatches = right || !ca.Right;
            bool artTypeMatches = artType == ArtTypeEnum.None || ca.Type == ArtTypeEnum.None || ca.Type == artType;
            if (rightMatches && artTypeMatches)
            {
                string truePath = ca.FileName;
                string newBoneName = boneName + '_' + ca.Name;

                if (!loader.LoadBoneSources())
                    return false;
                if (loader.TryGetBoneFilePath(newBoneName, out string? boneSource))
                    truePath = boneSource;

                if (!loader.SwfExists(truePath))
                    continue;

                if (!loader.LoadSwf(truePath))
                    return false;
                if (loader.TryGetSymbolId(truePath, newBoneName, out _))
                {
                    chosen = ca;
                    return true;
                }
            }
        }
        return true;
    }

    private static void SetAsymBonesVisibility(List<BoneInstance> bones, IGfxType gfx, bool spriteMirrored, bool isTooltip = false)
    {
        bool useRightTorso = gfx.UseRightTorso;
        bool useTrueLeftRightTorso = gfx.UseTrueLeftRightTorso;
        bool useRightJaw = gfx.UseRightJaw;
        bool useRightEyes = gfx.UseRightEyes;
        bool useRightMouth = gfx.UseRightMouth;
        bool useRightHair = gfx.UseRightHair;
        bool useRightGauntlet = gfx.UseRightGauntlet;
        bool useRightGauntletRight = gfx.UseRightGauntlet;
        int rightKatarUses = gfx.UseRightKatar ? 2 : 0;
        int rightForearmUses = gfx.UseRightForearm ? 2 : 0;
        int trueLeftRightHandsUses = gfx.UseTrueLeftRightHands ? 4 : 0;
        bool useRightShoulder1 = gfx.UseRightShoulder1;
        bool useRightShoulder1Right = gfx.UseRightShoulder1;
        int rightShinUses = gfx.UseRightShin ? 2 : 0;
        bool useRightLeg1 = gfx.UseRightLeg1;
        bool useRightLeg1Right = gfx.UseRightLeg1;
        bool hideRightPistol2D = isTooltip && gfx.HidePaperDollRightPistol && gfx.HideRightPistol2D;
        for (int i = 0; i < bones.Count; ++i)
        {
            BoneInstance instance = bones[i];
            bool mirrored = false;
            bool hand = false;
            if (BoneDatabase.BoneTypeDict.TryGetValue(instance.OgBoneName, out BoneType boneType))
            {
                if (MirroredBoneTypes.Contains(boneType.Type))
                {
                    double det = instance.Bone.ScaleX * instance.Bone.ScaleY - instance.Bone.RotateSkew0 * instance.Bone.RotateSkew1;
                    mirrored = (det < 0) != boneType.Dir;
                }
                hand = boneType.Type == BoneTypeEnum.HAND;
            }

            void doVisibilitySwap()
            {
                if (i < bones.Count - 1)
                {
                    bones[i].Visible = mirrored == spriteMirrored;
                    bones[i + 1].Visible = mirrored != spriteMirrored;
                }
            }

            if (useRightTorso && instance.OgBoneName == "a_Torso1")
            {
                doVisibilitySwap();
                useRightTorso = false;
            }
            else if (useTrueLeftRightTorso && instance.OgBoneName == "a_BotTorso")
            {
                doVisibilitySwap();
                useTrueLeftRightTorso = false;
            }
            else if (useRightJaw && instance.OgBoneName == "a_Jaw")
            {
                doVisibilitySwap();
                useRightJaw = false;
            }
            else if (useRightEyes && instance.OgBoneName.StartsWith("a_Eyes"))
            {
                doVisibilitySwap();
                useRightEyes = false;
            }
            else if (useRightMouth && instance.OgBoneName.StartsWith("a_Mouth"))
            {
                doVisibilitySwap();
                useRightMouth = false;
            }
            else if (useRightHair && instance.OgBoneName.StartsWith("a_Hair"))
            {
                doVisibilitySwap();
                useRightHair = false;
            }
            else if (useRightGauntlet && instance.OgBoneName == "a_WeaponFistsForearm")
            {
                doVisibilitySwap();
                useRightGauntlet = false;
            }
            else if (useRightGauntletRight && instance.OgBoneName == "a_WeaponFistsForearmRight")
            {
                doVisibilitySwap();
                useRightGauntletRight = false;
            }
            else if (rightKatarUses > 0 && BoneDatabase.KatarVariantDict.ContainsKey(instance.OgBoneName))
            {
                doVisibilitySwap();
                rightKatarUses--;
            }
            else if (rightForearmUses > 0 && BoneDatabase.ForearmVariantDict.ContainsKey(instance.OgBoneName))
            {
                doVisibilitySwap();
                rightForearmUses--;
            }
            else if (trueLeftRightHandsUses > 0 && hand)
            {
                bones[i].Visible = (i % 2 == 0) ? !spriteMirrored : spriteMirrored;
                trueLeftRightHandsUses--;
            }
            else if (useRightShoulder1 && instance.OgBoneName == "a_Shoulder1")
            {
                doVisibilitySwap();
                useRightShoulder1 = false;
            }
            else if (useRightShoulder1Right && instance.OgBoneName == "a_Shoulder1Right")
            {
                doVisibilitySwap();
                useRightShoulder1Right = false;
            }
            else if (useRightLeg1 && instance.OgBoneName == "a_Leg1")
            {
                doVisibilitySwap();
                useRightLeg1 = false;
            }
            else if (useRightLeg1Right && instance.OgBoneName == "a_Leg1Right")
            {
                doVisibilitySwap();
                useRightLeg1Right = false;
            }
            else if (rightShinUses > 0 && BoneDatabase.ShinVariantDict.ContainsKey(instance.OgBoneName))
            {
                doVisibilitySwap();
                rightShinUses--;
            }
            else if (hideRightPistol2D)
            {
                bool rightPistol = instance.OgBoneName == "a_WeaponPistolRight";
                bool firstPistol = instance.OgBoneName == "a_WeaponPistol" && bones[i + 1].OgBoneName == "a_WeaponPistol";
                if (rightPistol || firstPistol)
                {
                    instance.Visible = false;
                    hideRightPistol2D = false;
                }
            }
        }
    }

    private bool BuildColorMap(BoneSpriteWithName sprite, BoneInstance instance, IEnumerable<IColorSwap> colorSwaps)
    {
        ArtTypeEnum artType = BoneDatabase.ArtTypeDict.GetValueOrDefault(instance.OgBoneName, ArtTypeEnum.None);

        List<IColorSwap> matchingColorSwaps = [];
        foreach (IColorSwap colorSwap in colorSwaps)
        {
            if (colorSwap.NewColor == 0) continue;
            if (colorSwap.ArtType != ArtTypeEnum.None && colorSwap.ArtType != artType) continue;

            matchingColorSwaps.Add(colorSwap);
        }

        // no swaps left
        if (matchingColorSwaps.Count == 0) return true;

        // now get .a

        string boneSwfPath = sprite.SwfFilePath;
        if (!loader.LoadSwf(boneSwfPath))
            return false;

        // no .a
        if (!loader.TryGetScriptAVar(boneSwfPath, instance.SpriteName, out uint[]? a))
            return true;

        HashSet<uint> oldColorsWithArt = [];

        // filter to those that have a source in .a
        HashSet<uint> possibleSourceColors = [.. a];
        for (int i = matchingColorSwaps.Count - 1; i >= 0; --i)
        {
            IColorSwap colorSwap = matchingColorSwaps[i];

            if (!possibleSourceColors.Contains(colorSwap.OldColor))
                continue;

            // color swaps with art type take priority over those without
            if (colorSwap.ArtType == ArtTypeEnum.None && oldColorsWithArt.Contains(colorSwap.OldColor))
                continue;

            sprite.ColorSwapDict[colorSwap.OldColor] = colorSwap.NewColor;

            if (colorSwap.ArtType != ArtTypeEnum.None)
                oldColorsWithArt.Add(colorSwap.OldColor);
        }

        return true;
    }
}