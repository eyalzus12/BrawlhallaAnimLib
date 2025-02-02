using System;
using System.Collections.Generic;
using SwfLib.Tags;
using SwfLib.Tags.DisplayListTags;

namespace BrawlhallaAnimLib.Swf;

public class SwfSprite
{
    public SwfSpriteFrame[] Frames { get; set; } = [];

    public static SwfSprite CompileFrom(DefineSpriteTag spriteTag)
    {
        List<SwfSpriteFrame> frames = [new()];
        foreach (SwfTagBase tag in spriteTag.Tags)
        {
            if (tag is PlaceObjectBaseTag placeObject)
            {
                if (tag is PlaceObjectTag)
                {
                    throw new ArgumentException("Place object tag 1 is not supported. Only 2 and 3.");
                }

                if (frames[^1].Layers.TryGetValue(placeObject.Depth, out SwfSpriteFrameLayer? layer))
                {
                    layer.ModifyBy(placeObject);
                }
                else
                {
                    frames[^1].Layers[placeObject.Depth] = new() { FrameOffset = 0, Matrix = placeObject.Matrix, CharacterId = placeObject.CharacterID };
                }
            }
            else if (tag is RemoveObjectTag)
            {
                throw new ArgumentException("Remove object tag 1 is not supported. Only 2.");
            }
            else if (tag is RemoveObject2Tag removeObject)
            {
                frames[^1].Layers.Remove(removeObject.Depth);
            }
            else if (tag is ShowFrameTag)
            {
                frames.Add(frames[^1].Clone());
            }
        }
        // we're adding a redundant frame at the end
        frames.RemoveAt(frames.Count - 1);

        return new() { Frames = [.. frames] };
    }
}