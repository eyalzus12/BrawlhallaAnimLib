using System;
using System.Collections.Generic;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Math;
using SwfLib.Tags;
using SwfLib.Tags.ShapeTags;

namespace BrawlhallaAnimLib.Swf;

public sealed class SpriteToShapeConverter(ILoader loader)
{
    public BoneShape[]? ConvertToShapes(BoneSprite boneSprite)
    {
        string swfPath = boneSprite.SwfFilePath;

        ushort spriteId;
        string spriteName;
        if (boneSprite is BoneSpriteWithName boneSpriteWithName)
        {
            spriteName = boneSpriteWithName.SpriteName;
            if (!loader.LoadSwf(swfPath))
                return null;

            if (!loader.TryGetSymbolId(swfPath, spriteName, out spriteId))
            {
                return [];
                //throw new ArgumentException($"Sprite {spriteName} not found in {swfPath}");
            }
        }
        else if (boneSprite is BoneSpriteWithId boneSpriteWithId)
        {
            spriteId = boneSpriteWithId.SpriteId;
            spriteName = boneSpriteWithId.SpriteId.ToString();
        }
        else
        {
            throw new ArgumentException($"Unknown bone sprite type {boneSprite.GetType()}");
        }

        if (!loader.TryGetTag(swfPath, spriteId, out SwfTagBase? tag))
        {
            return [];
            //throw new ArgumentException($"Tag id {spriteId} for sprite {spriteName} not found in {swfPath}");
        }

        if (tag is not DefineSpriteTag spriteTag)
            throw new ArgumentException($"Tag id {spriteId} for sprite {spriteName} leads to a non-sprite tag in {swfPath}");

        // TODO: cache
        SwfSprite sprite = SwfSprite.CompileFrom(spriteTag);
        if (sprite.Frames.Length == 0)
            throw new ArgumentException($"Sprite {spriteName} has no frames");

        List<BoneShape> result = [];

        int frame = (int)MathUtils.SafeMod(boneSprite.Frame, sprite.Frames.Length);
        SwfSpriteFrame spriteFrame = sprite.Frames[frame];
        foreach ((ushort depth, SwfSpriteFrameLayer layer) in spriteFrame.Layers)
        {
            if (!loader.TryGetTag(swfPath, layer.CharacterId, out SwfTagBase? layerTag))
                throw new ArgumentException($"Sprite {spriteName} has invalid character id {layer.CharacterId} at depth {depth}");

            Transform2D layerTransform = MathUtils.SwfMatrixToTransform(layer.Matrix);
            Transform2D childTransform = boneSprite.Transform * layerTransform;

            // is a shape
            if (layerTag is ShapeBaseTag shapeTag)
            {
                result.Add(new()
                {
                    SwfFilePath = swfPath,
                    BoneName = spriteName,
                    ShapeId = shapeTag.ShapeID,
                    AnimScale = boneSprite.AnimScale,
                    Transform = childTransform,
                    Tint = boneSprite.Tint,
                    ColorSwapDict = boneSprite.ColorSwapDict,
                    Opacity = boneSprite.Opacity,
                });
            }
            else if (layerTag is DefineSpriteTag childSpriteTag)
            {
                ushort childSpriteId = childSpriteTag.SpriteID;
                BoneSpriteWithId childSprite = new()
                {
                    SwfFilePath = swfPath,
                    SpriteId = childSpriteId,
                    Frame = boneSprite.Frame + layer.FrameOffset,
                    AnimScale = boneSprite.AnimScale,
                    Transform = childTransform,
                    Tint = boneSprite.Tint,
                    ColorSwapDict = boneSprite.ColorSwapDict,
                    Opacity = boneSprite.Opacity,
                };
                BoneShape[]? shapes = ConvertToShapes(childSprite);
                if (shapes is null) return null;
                result.AddRange(shapes);
            }
            else
            {
                throw new ArgumentException($"Sprite {spriteName} has unknown tag type {layerTag.TagType} at depth {depth}");
            }
        }

        return [.. result];
    }
}