using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SwfLib.Tags;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;
using BrawlhallaAnimLib.Anm;

namespace BrawlhallaAnimLib;

[Flags]
public enum AnimationBuilderOptions
{
    None = 0,
    WeaponTooltip = 1 << 0,
    BigHeadMode = 1 << 1,
}

public readonly struct AnimationData
{
    public required long FrameCount { get; init; }
    public required uint LoopStart { get; init; }
    public required uint RecoveryStart { get; init; }
    public required uint FreeStart { get; init; }
    public required uint PreviewFrame { get; init; }
    public required uint BaseStart { get; init; }
    public required uint[] RunEndFrames { get; init; }
}

public static class AnimationBuilder
{
    private static readonly string[] AnimClassPrefixes = ["a_Animation_EB_", "a__LootBox", "a__PodiumRig"];
    private static bool IsAnmAnimation(string animFile, string animClass)
    {
        return
            animFile == "UI_TooltipAnimations.swf" ||
            animFile.StartsWith("Animation_") ||
            AnimClassPrefixes.Any(animClass.StartsWith);
    }

    private static readonly HashSet<string> BigHeadBones = [
        "a_Hair",
        "a_HairBack",
        "a_HairR",
        "a_HairRBack",
        "a_Jaw",
        "a_JawR",
        "a_Eyes",
        "a_EyesAngry",
        "a_EyesDown",
        "a_EyesHit",
        "a_EyesKO",
        "a_EyesTurn",
        "a_EyesR",
        "a_EyesRAngry",
        "a_EyesRDown",
        "a_EyesRHit",
        "a_EyesRKO",
        "a_EyesRTurn",
        "a_Mouth",
        "a_MouthBlow",
        "a_MouthGrowl",
        "a_MouthHit",
        "a_MouthKO",
        "a_MouthSmile",
        "a_MouthWarCry",
        "a_MouthR",
        "a_MouthRBlow",
        "a_MouthRGrowl",
        "a_MouthRHit",
        "a_MouthRKO",
        "a_MouthRSmile",
        "a_MouthRWarCry",
        "a_Nose",
        "a_Accent",
        "a_AccentAngry",
        "a_AccentDown",
        "a_AccentHit",
        "a_AccentKO",
        "a_AccentTurn",
        "a_Helmet",
    ];

    public static async ValueTask<AnimationData> GetAnimData(ILoader loader, string animFile, string animClass, string animName)
    {
        if (IsAnmAnimation(animFile, animClass))
        {
            IAnmClass? anmClass = await loader.GetAnmClass($"{animFile}/{animClass}") ?? throw new ArgumentException($"Could not find anim class {animClass} in {animFile}. Make sure you loaded it.");

            if (!anmClass.TryGetAnimation(animName, out IAnmAnimation? animation))
                throw new ArgumentException($"No animation {animName} in anim class {animClass}");

            return new()
            {
                FrameCount = animation.Frames.Length,
                LoopStart = animation.LoopStart,
                RecoveryStart = animation.RecoveryStart,
                FreeStart = animation.FreeStart,
                PreviewFrame = animation.PreviewFrame,
                BaseStart = animation.BaseStart,
                RunEndFrames = animation.RunEndFrames,
            };
        }
        else
        {
            ushort spriteId = await loader.GetSymbolId(animFile, animClass) ?? throw new ArgumentException($"Sprite {animClass} not found in {animFile}");
            SwfTagBase? tag = await loader.GetTag(animFile, spriteId) ?? throw new ArgumentException($"Tag id {spriteId} for sprite {animClass} not found in {animFile}");

            if (tag is not DefineSpriteTag sprite)
                throw new ArgumentException($"Tag id {spriteId} does not point to a sprite in {animFile}");

            return new()
            {
                FrameCount = sprite.FramesCount,
                LoopStart = 0,
                RecoveryStart = sprite.FramesCount,
                BaseStart = 0,
                FreeStart = sprite.FramesCount,
                PreviewFrame = 0,
                RunEndFrames = [],
            };
        }
    }

    public static async IAsyncEnumerable<BoneSprite> BuildAnim(ILoader loader, IGfxType gfx, string animName, long frame, Transform2D transform, AnimationBuilderOptions options = AnimationBuilderOptions.None)
    {
        Transform2D scaleTransform = Transform2D.CreateScale(gfx.AnimScale, gfx.AnimScale);
        Transform2D realTransform = transform * scaleTransform;


        // anm animation
        if (IsAnmAnimation(gfx.AnimFile, gfx.AnimClass))
        {
            IAnmClass? anmClass = await loader.GetAnmClass($"{gfx.AnimFile}/{gfx.AnimClass}") ?? throw new ArgumentException($"Could not find anim class {gfx.AnimClass} in {gfx.AnimFile}. Make sure you loaded it.");

            if (!anmClass.TryGetAnimation(animName, out IAnmAnimation? animation))
                throw new ArgumentException($"No animation {animName} in anim class {gfx.AnimClass}");
            if (animation.Frames.Length == 0)
                throw new ArgumentException($"Animation {animName} has no frames");

            long frameIndex = MathUtils.SafeMod(frame, animation.Frames.Length);
            IAnmFrame anmFrame = animation.Frames[frameIndex];

            bool isTooltip = options.HasFlag(AnimationBuilderOptions.WeaponTooltip);
            IAsyncEnumerable<BoneInstance> bones = GetBoneInstances(loader, anmFrame.Bones, gfx);
            IBoneDatabase boneDatabase = await loader.GetBoneDatabase();
            bones = SetAsymBonesVisibility(boneDatabase, bones, gfx, realTransform.ScaleX * realTransform.ScaleY < 0, isTooltip);

            bool bigHeadMode = options.HasFlag(AnimationBuilderOptions.BigHeadMode);

            await foreach (BoneInstance instance in bones)
            {
                if (!instance.Visible)
                    continue;

                // apply big head mode
                Transform2D bigHeadTransform = Transform2D.IDENTITY;
                double animScale = gfx.AnimScale;
                if (bigHeadMode && BigHeadBones.Contains(instance.OgBoneName))
                {
                    bigHeadTransform = Transform2D.CreateScale(2, 2);
                    animScale *= 2;
                }

                IAnmBone bone = instance.Bone;
                Transform2D boneTransform = new(bone.ScaleX, bone.RotateSkew1, bone.RotateSkew0, bone.ScaleY, bone.X, bone.Y);

                if (instance is SwfBoneInstance swfInstance)
                {
                    SwfBoneSpriteWithName boneSprite = new()
                    {
                        SwfFilePath = swfInstance.SwfFilePath,
                        SpriteName = swfInstance.SpriteName,
                        Frame = bone.Frame - 1,
                        AnimScale = animScale,
                        Transform = realTransform * boneTransform * bigHeadTransform,
                        Tint = gfx.Tint,
                        Opacity = bone.Opacity,
                    };
                    await BuildColorMap(loader, boneSprite, swfInstance, gfx.ColorSwaps);
                    yield return boneSprite;
                }
                else if (instance is BitmapBoneInstance bitmapInstance)
                {
                    BitmapBoneSprite boneSprite = new()
                    {
                        SpriteData = bitmapInstance.SpriteData,
                        Transform = realTransform * boneTransform * bigHeadTransform,
                        Tint = gfx.Tint,
                        Opacity = bone.Opacity,
                    };
                    yield return boneSprite;
                }
            }
        }
        // swf animation
        else
        {
            SwfBoneInstance fakeInstance = new()
            {
                SwfFilePath = gfx.AnimFile,
                OgBoneName = gfx.AnimClass,
                SpriteName = gfx.AnimClass,
                Bone = null!,
                Visible = true,
            };
            SwfBoneSpriteWithName boneSprite = new()
            {
                SwfFilePath = gfx.AnimFile,
                SpriteName = gfx.AnimClass,
                Frame = frame,
                AnimScale = gfx.AnimScale,
                Transform = scaleTransform,
                Tint = gfx.Tint,
                Opacity = 1,
            };
            await BuildColorMap(loader, boneSprite, fakeInstance, gfx.ColorSwaps);

            yield return boneSprite;
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

    // cannot be Task.WhenAll'ed because of otherHand and handBoneName
    private static async IAsyncEnumerable<BoneInstance> GetBoneInstances(ILoader loader, IEnumerable<IAnmBone> bones, IGfxType gfx)
    {
        IBoneDatabase boneDatabase = await loader.GetBoneDatabase();

        bool otherHand = false;
        string handBoneName = "";
        foreach (IAnmBone bone in bones)
        {
            string boneName = await loader.GetBoneName(bone.Id) ?? throw new ArgumentException($"Could not find bone name for id {bone.Id}");

            BoneType? boneType;
            if (boneDatabase.TryGetBoneType(boneName, out BoneTypeEnum type, out bool dir))
                boneType = new(type, dir);
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
            else if ((boneType is null || !gfx.HasAsymmetrySwapFlag(boneType.Value.Type)) && boneDatabase.TryGetAsymSwap(boneName, out string? otherBoneName))
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

            List<ICustomArt> customArts = [.. gfx.CustomArts];
            ISpriteData? spriteData = await FindSpriteData(loader, boneName, customArts);
            ICustomArt? customArt = spriteData is not null
                ? null
                : await FindCustomArt(loader, boneName, finalBoneName, customArts, isHand ? right : mirrored);

            bool visible = boneType switch
            {
                null => true,
                BoneType boneInfo => boneInfo.Type switch
                {
                    BoneTypeEnum.HAND when isOtherHand => false,
                    BoneTypeEnum.FOREARM when boneDatabase.IsVariantFor(finalBoneName, boneInfo.Type) => false,
                    BoneTypeEnum.SHOULDER when finalBoneName == "a_Shoulder1R" || finalBoneName == "a_Shoulder1RightR" => false,
                    BoneTypeEnum.LEG when finalBoneName == "a_Leg1R" || finalBoneName == "a_Leg1RightR" => false,
                    BoneTypeEnum.SHIN when boneDatabase.IsVariantFor(finalBoneName, boneInfo.Type) => false,
                    BoneTypeEnum._TORSO when finalBoneName == "a_Torso1R" || finalBoneName == "a_BotTorsoR" => false,
                    BoneTypeEnum.GAUNTLETFOREARM when finalBoneName == "a_WeaponFistsForearmR" || finalBoneName == "a_WeaponFistsForearmRightR" => false,
                    BoneTypeEnum.KATAR when boneDatabase.IsVariantFor(finalBoneName, boneInfo.Type) => false,
                    _ => true,
                }
            };

            if (spriteData is not null)
            {
                yield return new BitmapBoneInstance()
                {
                    SpriteData = spriteData,
                    OgBoneName = boneName,
                    Bone = bone,
                    Visible = visible,
                };
            }
            else
            {
                string customArtSuffix = customArt is not null ? $"_{customArt.Name}" : "";
                string trueSpriteName = finalBoneName + customArtSuffix;
                string trueFilePath = customArt?.FileName ?? gfx.AnimFile;

                string? boneSource = await loader.GetBoneFilePath(trueSpriteName);
                if (boneSource is not null)
                    trueFilePath = boneSource;

                if (!loader.SwfExists(trueFilePath))
                    continue;

                yield return new SwfBoneInstance()
                {
                    SwfFilePath = trueFilePath,
                    SpriteName = trueSpriteName,
                    OgBoneName = boneName,
                    Bone = bone,
                    Visible = visible,
                };
            }
        }
    }

    private static async ValueTask<ISpriteData?> FindSpriteData(ILoader loader, string boneName, List<ICustomArt> customArts)
    {
        for (int i = customArts.Count - 1; i >= 0; --i)
        {
            ICustomArt ca = customArts[i];
            ISpriteData? spriteData = await loader.GetSpriteData(boneName, ca.Name);
            if (spriteData is not null) return spriteData;
        }

        return await loader.GetSpriteData(boneName, "");
    }

    private static async ValueTask<ICustomArt?> FindCustomArt(ILoader loader, string ogBoneName, string boneName, List<ICustomArt> customArts, bool right)
    {
        IBoneDatabase boneDatabase = await loader.GetBoneDatabase();
        ArtTypeEnum artType = ArtTypeEnum.None;
        if (boneDatabase.TryGetArtType(ogBoneName, out ArtTypeEnum artType_))
            artType = artType_;

        for (int i = customArts.Count - 1; i >= 0; --i)
        {
            ICustomArt ca = customArts[i];
            bool rightMatches = !ca.Right || right;
            bool artTypeMatches = artType == ArtTypeEnum.None || ca.Type == ArtTypeEnum.None || ca.Type == artType;
            if (rightMatches && artTypeMatches)
            {
                string truePath = ca.FileName;
                string newBoneName = boneName + '_' + ca.Name;

                string? boneSource = await loader.GetBoneFilePath(newBoneName);
                if (boneSource is not null)
                    truePath = boneSource;

                if (!loader.SwfExists(truePath))
                    continue;

                if (await loader.GetSymbolId(truePath, newBoneName) is not null)
                    return ca;
            }
        }
        return null;
    }

    // cannot be Task.WhenAll'ed because we have to read and manipulate following bones
    private static async IAsyncEnumerable<BoneInstance> SetAsymBonesVisibility(IBoneDatabase boneDatabase, IAsyncEnumerable<BoneInstance> bones, IGfxType gfx, bool spriteMirrored, bool isTooltip = false)
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

        int i = 0;
        await foreach ((BoneInstance instance, BoneInstance? next) in bones.Pairs())
        {
            bool mirrored = false;
            bool hand = false;
            if (boneDatabase.TryGetBoneType(instance.OgBoneName, out BoneTypeEnum boneType, out bool boneDir))
            {
                if (MirroredBoneTypes.Contains(boneType))
                {
                    float det = instance.Bone.ScaleX * instance.Bone.ScaleY - instance.Bone.RotateSkew0 * instance.Bone.RotateSkew1;
                    mirrored = (det < 0) != boneDir;
                }
                hand = boneType == BoneTypeEnum.HAND;
            }

            void doVisibilitySwap()
            {
                if (next is not null)
                {
                    instance.Visible = mirrored == spriteMirrored;
                    next.Visible = mirrored != spriteMirrored;
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
            else if (rightKatarUses > 0 && boneDatabase.HasVariantFor(instance.OgBoneName, BoneTypeEnum.KATAR))
            {
                doVisibilitySwap();
                rightKatarUses--;
            }
            else if (rightForearmUses > 0 && boneDatabase.HasVariantFor(instance.OgBoneName, BoneTypeEnum.FOREARM))
            {
                doVisibilitySwap();
                rightForearmUses--;
            }
            else if (trueLeftRightHandsUses > 0 && hand)
            {
                instance.Visible = (i % 2 == 0) ? !spriteMirrored : spriteMirrored;
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
            else if (rightShinUses > 0 && boneDatabase.HasVariantFor(instance.OgBoneName, BoneTypeEnum.SHIN))
            {
                doVisibilitySwap();
                rightShinUses--;
            }
            else if (hideRightPistol2D)
            {
                bool rightPistol = instance.OgBoneName == "a_WeaponPistolRight";
                bool firstPistol = next is not null && instance.OgBoneName == "a_WeaponPistol" && next.OgBoneName == "a_WeaponPistol";
                if (rightPistol || firstPistol)
                {
                    instance.Visible = false;
                    hideRightPistol2D = false;
                }
            }

            yield return instance;
            ++i;
        }
    }

    private static async ValueTask BuildColorMap(ILoader loader, SwfBoneSpriteWithName sprite, SwfBoneInstance instance, IEnumerable<IColorSwap> colorSwaps)
    {
        IBoneDatabase boneDatabase = await loader.GetBoneDatabase();

        ArtTypeEnum artType = ArtTypeEnum.None;
        if (boneDatabase.TryGetArtType(instance.OgBoneName, out ArtTypeEnum artType_))
            artType = artType_;

        HashSet<uint>? aSet = null;
        bool canColorSwap = false;
        foreach (IColorSwap colorSwap in colorSwaps)
        {
            // art mismatch
            if (colorSwap.ArtType != ArtTypeEnum.None && colorSwap.ArtType != artType)
                continue;

            // lazy compute
            if (aSet is null)
            {
                // get .a
                string boneSwfPath = sprite.SwfFilePath;
                uint[]? a = await loader.GetScriptAVar(boneSwfPath, instance.SpriteName);
                // no .a
                if (a is null || a.Length == 0)
                    return;
                aSet = [.. a];
            }

            // .a mismatch
            if (!aSet.Contains(colorSwap.OldColor)) continue;

            // if we reached here, we have a valid color swap
            canColorSwap = true;
            break;
        }
        if (!canColorSwap) return;

        // now we create the actual color swap dict

        HashSet<uint> oldColorsWithArt = [];
        foreach (IColorSwap colorSwap in colorSwaps.Reverse())
        {
            // filter out bad art types
            if (colorSwap.ArtType != ArtTypeEnum.None && colorSwap.ArtType != artType)
                continue;

            // later color swaps override earlier, but color swaps that match the art get priority
            bool artPriority = colorSwap.ArtType != ArtTypeEnum.None && colorSwap.ArtType == artType;
            if (artPriority || !oldColorsWithArt.Contains(colorSwap.OldColor))
            {
                sprite.ColorSwapDict[colorSwap.OldColor] = colorSwap.NewColor;
                if (artPriority)
                {
                    oldColorsWithArt.Add(colorSwap.OldColor);
                }
            }
        }
    }

    private static async IAsyncEnumerable<(T, T?)> Pairs<T>(this IAsyncEnumerable<T> enumerable)
    {
        await using IAsyncEnumerator<T> enumerator = enumerable.GetAsyncEnumerator();
        if (!await enumerator.MoveNextAsync())
            yield break;
        T prev = enumerator.Current;
        while (await enumerator.MoveNextAsync())
        {
            T current = enumerator.Current;
            yield return (prev, current);
            prev = current;
        }
        yield return (prev, default);
    }
}