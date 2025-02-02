using System;
using System.Collections.Generic;
using System.Linq;

using WallyAnmSpinzor;

using BrawlhallaAnimLib.Animation;
using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;
using BrawlhallaAnimLib.Loading;

namespace BrawlhallaAnimLib;

public sealed class AnimationBuilder(ISwfLoader swfLoader, IAnmLoader anmLoader, IBoneDataLoader boneDataLoader)
{
    // TODO: needs to be done using BoneSources.xml
    // although there are no swf files where this temp mapping doesn't work
    private static string GetRealSwfPath(string path)
    {
        if (path.StartsWith("Animation_"))
            return $"bones/Bones_{path["Animation_".Length..]}";
        return path;
    }

    private static readonly string[] AnimationPrefixes = ["Animation_", "a_Animation_EB_", "a__LootBox", "a__PodiumRig"];
    private static bool IsAnmAnimation(string path)
    {
        return path == "UI_TooltipAnimations.swf" || AnimationPrefixes.Any(path.StartsWith);
    }

    // null if not loaded yet
    public BoneSprite[]? BuildAnim(IGfxType gfx, string animName, long frame, Transform2D transform)
    {
        string animFile = GetRealSwfPath(gfx.AnimFile);

        // anm animation
        if (IsAnmAnimation(gfx.AnimFile))
        {
            anmLoader.LoadAnm(animFile);
            if (!anmLoader.IsAnmLoaded(animFile))
                return null;
            if (!anmLoader.TryGetAnmClass($"{gfx.AnimFile}/{gfx.AnimClass}", out AnmClass? anmClass))
                throw new ArgumentException($"Could not find anim class {gfx.AnimClass} in {animFile}");
            if (!anmClass.Animations.TryGetValue(animName, out AnmAnimation? animation))
                throw new ArgumentException($"No animation {animName} in anim class {gfx.AnimClass}");
            if (animation.Frames.Length == 0)
                throw new ArgumentException($"Animation {animName} has no frames");
            long frameIndex = MathUtils.SafeMod(frame + animation.BaseStart, animation.Frames.Length);
            AnmFrame anmFrame = animation.Frames[frameIndex];

            List<BoneInstance>? bones = GetBoneInstances(anmFrame.Bones, gfx);
            if (bones is null) return null;

            SetAsymBonesVisibility(bones, gfx, transform.ScaleX * transform.ScaleY < 0);

            List<BoneSprite> result = [];
            foreach (BoneInstance instance in bones)
            {
                if (!instance.Visible)
                    continue;
                string boneSwfPath = GetRealSwfPath(instance.FilePath);

                AnmBone bone = instance.Bone;
                Transform2D boneTransform = new(bone.ScaleX, bone.RotateSkew1, bone.RotateSkew0, bone.ScaleY, bone.X, bone.Y);
                result.Add(new BoneSpriteWithName()
                {
                    SwfFilePath = boneSwfPath,
                    SpriteName = instance.SpriteName,
                    Frame = bone.Frame - 1,
                    AnimScale = gfx.AnimScale,
                    Transform = transform * boneTransform,
                    Tint = gfx.Tint,
                    // TODO: implement correctly
                    ColorSwaps = [.. gfx.ColorSwaps()],
                    Opacity = bone.Opacity,
                });
            }
            return [.. result];
        }
        // swf animation
        else
        {
            BoneSprite sprite = new BoneSpriteWithName()
            {
                SwfFilePath = animFile,
                SpriteName = gfx.AnimClass,
                Frame = frame,
                AnimScale = gfx.AnimScale,
                Transform = transform,
                Tint = gfx.Tint,
                // TODO: implement correctly
                ColorSwaps = [.. gfx.ColorSwaps()],
                Opacity = 1,
            };
            return [sprite];
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

    private List<BoneInstance>? GetBoneInstances(AnmBone[] bones, IGfxType gfx)
    {
        boneDataLoader.LoadBoneTypes();
        if (!boneDataLoader.IsBoneTypesLoaded())
            return null;

        List<BoneInstance> instances = [];
        bool otherHand = false;
        string handBoneName = "";
        foreach (AnmBone bone in bones)
        {
            if (!boneDataLoader.TryGetBoneName(bone.Id, out string? boneName))
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

            if (!FindCustomArt(boneName, finalBoneName, gfx.CustomArts(), right, out ICustomArt? customArt))
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
            instances.Add(new()
            {
                FilePath = customArt?.FileName ?? gfx.AnimFile,
                SpriteName = finalBoneName + customArtSuffix,
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

        uint artType = BoneDatabase.ArtTypeDict.GetValueOrDefault(ogBoneName, 0u);
        foreach (ICustomArt ca in customArts.Reverse())
        {
            if ((right || !ca.Right) && (artType == 0 || ca.Type == 0 || ca.Type == artType))
            {
                string caPath = GetRealSwfPath(ca.FileName);
                swfLoader.LoadSwf(caPath);
                if (!swfLoader.IsSwfLoaded(caPath))
                    return false;
                if (swfLoader.TryGetSymbolId(caPath, $"{boneName}_{ca.Name}", out _))
                {
                    chosen = ca;
                    return true;
                }
            }
        }
        return true;
    }

    private static void SetAsymBonesVisibility(IReadOnlyList<BoneInstance> bones, IGfxType gfx, bool spriteMirrored)
    {
        bool useRightTorso = gfx.UseRightTorso;
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
        }
    }
}