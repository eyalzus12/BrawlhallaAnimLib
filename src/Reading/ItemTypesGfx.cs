using System;
using System.Collections.Generic;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading.ItemTypes;

public sealed class ItemTypesGfx
{
    internal InternalCustomArtImpl? HeldCustomArt { get; }
    internal bool HasSeparateTeamAnims { get; }
    internal InternalGfxImpl? EquipGfxType { get; }
    internal InternalGfxImpl? WorldGfxType { get; }
    internal string? WorldGfxSingleOverride { get; }

    public bool HasHeldCustomArt => HeldCustomArt is not null;
    public bool HasEquipGfx => EquipGfxType is not null;
    public bool HasWorldGfx => WorldGfxType is not null;

    public ItemTypesGfx(ICsvRow row)
    {
        string? EquipGfxType_AnimClass = null;
        string? EquipGfxType_AnimFile = null;
        List<InternalCustomArtImpl> EquipGfxType_CustomArts = [];
        List<InternalColorSwapImpl> EquipGfxType_ColorSwaps = [];

        string? WorldGfxType_AnimClass = null;
        string? WorldGfxType_AnimFile = null;
        List<InternalCustomArtImpl> WorldGfxType_CustomArts = [];
        List<InternalColorSwapImpl> WorldGfxType_ColorSwaps = [];

        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "HeldCustomArt")
            {
                HeldCustomArt = ParserUtils.ParseCustomArt(value, false, ArtTypeEnum.None);
            }
            else if (key == "HasSeparateTeamAnims")
            {
                HasSeparateTeamAnims = ParserUtils.ParseBool(value);
            }
            else if (key == "EquipGfxType.AnimClass")
            {
                EquipGfxType_AnimClass = value;
            }
            else if (key == "EquipGfxType.AnimFile")
            {
                EquipGfxType_AnimFile = value;
            }
            else if (key.StartsWith("EquipGfxType.CustomArt"))
            {
                EquipGfxType_CustomArts.Add(ParserUtils.ParseCustomArt(value, true, ArtTypeEnum.Weapon));
            }
            else if (key.StartsWith("EquipGfxType.ColorSwap"))
            {
                EquipGfxType_ColorSwaps.Add(ParserUtils.ParseColorSwap(value, ArtTypeEnum.Weapon));
            }
            else if (key == "WorldGfxType.AnimClass")
            {
                WorldGfxType_AnimClass = value;
            }
            else if (key == "WorldGfxType.AnimFile")
            {
                WorldGfxType_AnimFile = value;
            }
            else if (key.StartsWith("WorldGfxType.CustomArt"))
            {
                WorldGfxType_CustomArts.Add(ParserUtils.ParseCustomArt(value, true, ArtTypeEnum.None));
            }
            else if (key.StartsWith("WorldGfxType.ColorSwap"))
            {
                WorldGfxType_ColorSwaps.Add(ParserUtils.ParseColorSwap(value, ArtTypeEnum.None));
            }
            else if (key == "WorldGfxSingleOverride")
            {
                WorldGfxSingleOverride = value;
            }
        }

        if (EquipGfxType_AnimClass is null && EquipGfxType_AnimFile is not null) throw new ArgumentException("Missing EquipGfxType.AnimClass");
        if (EquipGfxType_AnimFile is null && EquipGfxType_AnimClass is not null) throw new ArgumentException("Missing EquipGfxType.AnimFile");
        if (WorldGfxType_AnimClass is null && WorldGfxType_AnimFile is not null) throw new ArgumentException("Missing WorldGfxType.AnimClass");
        if (WorldGfxType_AnimFile is null && WorldGfxType_AnimClass is not null) throw new ArgumentException("Missing WorldGfxType.AnimFile");

        if (EquipGfxType_AnimFile is not null && EquipGfxType_AnimClass is not null)
            EquipGfxType = new InternalGfxImpl()
            {
                AnimFile = EquipGfxType_AnimFile,
                AnimClass = EquipGfxType_AnimClass,
                CustomArtsInternal = [.. EquipGfxType_CustomArts],
                ColorSwapsInternal = [.. EquipGfxType_ColorSwaps],
            };

        if (WorldGfxType_AnimFile is not null && WorldGfxType_AnimClass is not null)
            WorldGfxType = new InternalGfxImpl()
            {
                AnimFile = WorldGfxType_AnimFile,
                AnimClass = WorldGfxType_AnimClass,
                CustomArtsInternal = [.. WorldGfxType_CustomArts],
                ColorSwapsInternal = [.. WorldGfxType_ColorSwaps],
            };
    }

    public IGfxType ToHeldGfx(IGfxType gfxType, int team)
    {
        if (HeldCustomArt is null) return gfxType;

        InternalGfxImpl gfxResult = new(gfxType);
        if (HasSeparateTeamAnims && (team == 1 || team == 2))
        {
            string teamString = team == 1 ? "Red" : "Blue";
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl(HeldCustomArt)
            {
                Name = HeldCustomArt.Name + teamString,
            });
        }
        else
        {
            gfxResult.CustomArtsInternal.Add(HeldCustomArt);
        }
        return gfxResult;
    }

    public IGfxType? ToEquipGfx(IGfxType? gfxType = null)
    {
        if (gfxType is null) return EquipGfxType;
        if (EquipGfxType is null) return gfxType;

        InternalGfxImpl gfxResult = new(gfxType);
        gfxResult.CustomArtsInternal.AddRange(EquipGfxType.CustomArtsInternal);
        return gfxResult;
    }

    public IGfxType? ToWorldGfx(bool isSingle = false)
    {
        if (WorldGfxType is null)
            return null;
        if (!isSingle || WorldGfxSingleOverride is null)
            return WorldGfxType;
        return new InternalGfxImpl(WorldGfxType)
        {
            AnimClass = WorldGfxSingleOverride
        };
    }
}