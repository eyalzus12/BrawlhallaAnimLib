using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwfLib.Tags;
using SwfLib.Tags.ShapeTags;
using SwfLib.Tags.TextTags;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Swf;

public static class SpriteToShapeConverter
{
    public static async Task<BoneShape[]> ConvertToShapes(ILoader loader, BoneSprite boneSprite)
    {
        string swfPath = boneSprite.SwfFilePath;

        ushort spriteId;
        string spriteName;
        if (boneSprite is BoneSpriteWithName boneSpriteWithName)
        {
            spriteName = boneSpriteWithName.SpriteName;

            ushort? temp = await loader.GetSymbolId(swfPath, spriteName);
            if (temp is null)
            {
                //throw new ArgumentException($"Sprite {spriteName} not found in {swfPath}");
                return [];
            }

            spriteId = temp.Value;
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

        SwfTagBase? tag = await loader.GetTag(swfPath, spriteId);
        if (tag is null)
        {
            //throw new ArgumentException($"Tag id {spriteId} for sprite {spriteName} not found in {swfPath}");
            return [];
        }

        if (tag is not DefineSpriteTag spriteTag)
            throw new ArgumentException($"Tag id {spriteId} for sprite {spriteName} leads to a non-sprite tag in {swfPath}");

        // TODO: cache
        SwfSprite sprite = SwfSprite.CompileFrom(spriteTag);
        if (sprite.Frames.Length == 0)
            throw new ArgumentException($"Sprite {spriteName} has no frames");

        List<BoneShape> result = [];

        long clampedFrame = MathUtils.SafeMod(boneSprite.Frame, sprite.Frames.Length);
        SwfSpriteFrame spriteFrame = sprite.Frames[clampedFrame];
        foreach ((ushort depth, SwfSpriteFrameLayer layer) in spriteFrame.Layers)
        {
            SwfTagBase? layerTag = await loader.GetTag(swfPath, layer.CharacterId) ?? throw new ArgumentException($"Sprite {spriteName} has invalid character id {layer.CharacterId} at depth {depth} in {swfPath}");

            Transform2D layerTransform = MathUtils.SwfMatrixToTransform(layer.Matrix);
            Transform2D childTransform = boneSprite.Transform * layerTransform;

            // is a shape
            if (layerTag is ShapeBaseTag shapeTag)
            {
                result.Add(new()
                {
                    ShapeId = shapeTag.ShapeID,
                    Transform = childTransform,
                    Tint = boneSprite.Tint,
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
                    Transform = childTransform,
                    Tint = boneSprite.Tint,

                    AnimScale = 0, // not used
                    ColorSwapDict = null!, // not used
                    Opacity = 0, // nnot used
                };
                BoneShape[] shapes = await ConvertToShapes(loader, childSprite);
                result.AddRange(shapes);
            }
            else if (layerTag is DefineTextBaseTag text)
            {
                // we don't handle text because all DefineText in the game are empty strings
                // they are used for master chief cannon sigs for some reason
            }
            else
            {
                throw new ArgumentException($"Sprite {spriteName} has unimplemented tag type {layerTag.TagType} at depth {depth} in {swfPath}");
            }
        }

        return [.. result];
    }
}