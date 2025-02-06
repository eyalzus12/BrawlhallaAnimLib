using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public static class WeaponSkinTypesReader
{
    public static WeaponSkinTypesGfxInfo GetGfxTypeInfo(ICsvRow row, ICsvReader costumeTypesReader, IColorSchemeType? colorScheme)
    {
        WeaponSkinTypesGfxInfo gfx = new();

        string? baseWeapon = null;

        bool hasPickupCustomArt = false;
        List<InternalCustomArtImpl>? customArts = null;

        Dictionary<ColorSchemeSwapEnum, uint>? swapDefines = null;

        ColorSchemeSwapEnum? AttackFxLt_Enum = null;
        uint AttackFxLt_Color = 0;
        ColorSchemeSwapEnum? AttackFxDk_Enum = null;
        uint AttackFxDk_Color = 0;

        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "BaseWeapon")
            {
                baseWeapon = value;
            }
            else if (value == "UseRightGauntlet")
            {
                gfx.UseRightGauntlet = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (value == "UseRightKatar")
            {
                gfx.UseRightKatar = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (value == "HideRightPistol2D")
            {
                gfx.HideRightPistol2D = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select(static (flag) =>
                {
                    if (Enum.TryParse(flag, out BoneTypeEnum result))
                        return 1u << (int)result;
                    return 0u;
                }).Aggregate((a, v) => a | v);

                gfx.BoneTypeFlags = asf;
            }
            else if (key.StartsWith("CustomArt"))
            {
                ArtTypeEnum defaultType = ArtTypeEnum.Weapon;
                if (key.Contains("Pickup"))
                {
                    defaultType = ArtTypeEnum.Pickup;
                    hasPickupCustomArt = true;
                }
                // the game also checks for Costume, but sets to ArtTypeEnum.Weapon instead of ArtTypeEnum.Costume
                // is that a bug?

                customArts ??= [];
                customArts.Add(FromCustomArtCell(value, true, defaultType));
            }
            else if (key.EndsWith("_Define"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");
                swapDefines ??= [];
                swapDefines[swapType] = uint.Parse(value, CultureInfo.InvariantCulture);
            }
            else if (key == "AttackFxLt_Swap")
            {
                if (value.StartsWith("0x"))
                {
                    uint direct = Convert.ToUInt32(value, 16);
                    AttackFxLt_Color = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    AttackFxLt_Enum = target;
                }
            }
            else if (key == "AttackFxDk_Swap")
            {
                if (value.StartsWith("0x"))
                {
                    uint direct = Convert.ToUInt32(value, 16);
                    AttackFxDk_Color = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    AttackFxDk_Enum = target;
                }
            }
            // stupid bullshit
            else if (key == "InheritCostumeDefines")
            {
                if (!costumeTypesReader.TryGetCol(value, out ICsvRow? costumeType))
                    throw new ArgumentException($"{value} from InheritCostumeDefines not found");
                foreach ((string key2, string value2) in costumeType.ColEntries)
                {
                    if (key2.EndsWith("_Define"))
                    {
                        string swap = key[..^"_Define".Length];
                        if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                            throw new ArgumentException($"Invalid swap {swap}");
                        swapDefines ??= [];
                        swapDefines.TryAdd(swapType, uint.Parse(value2, CultureInfo.InvariantCulture));
                    }
                }
            }
            /*
            There are also these properties:
            CostumeOverrides - used by dhalsim
            OverrideCustomArt - used by dhalsim
            AttackGfxOverrideSource - used by lightsabers
            AttackGfxOverride.* - used by lightsabers
            */
        }

        ColorSchemeSwapEnum[] swapTypesList = Enum.GetValues<ColorSchemeSwapEnum>();
        if (colorScheme is not null)
        {
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                // TODO: color exception

                uint sourceColor = swapDefines?.GetValueOrDefault(swapType, 0u) ?? 0;
                if (sourceColor == 0) continue;
                uint targetColor = colorScheme.GetSwap(swapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfx.ColorSwapsInternal.Add(colorSwap);
            }

        }

        InternalColorSwapImpl? getColorSwap(ColorSchemeSwapEnum? @enum, uint color, uint sourceColor)
        {
            if (@enum is not null)
            {
                ColorSchemeSwapEnum swapType = @enum.Value;
                uint targetColor = colorScheme?.GetSwap(swapType) ?? 0;
                if (targetColor == 0)
                {
                    uint definedColor = swapDefines?.GetValueOrDefault(swapType, 0u) ?? 0;
                    targetColor = definedColor;
                }
                return new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
            }
            else if (color != 0)
            {
                return new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = color,
                };
            }
            return null;
        }

        InternalColorSwapImpl? AttackFxLt = getColorSwap(AttackFxLt_Enum, AttackFxLt_Color, 0x14D3FF);
        if (AttackFxLt is not null) gfx.ColorSwapsInternal.Add(AttackFxLt);
        InternalColorSwapImpl? AttackFxDk = getColorSwap(AttackFxDk_Enum, AttackFxDk_Color, 0x004DCC);
        if (AttackFxDk is not null) gfx.ColorSwapsInternal.Add(AttackFxDk);

        if (hasPickupCustomArt)
        {
            foreach ((ColorSchemeSwapEnum a, ColorSchemeSwapEnum b) in PickupColorSwapTypes)
            {
                uint source = swapDefines?.GetValueOrDefault(a, 0u) ?? 0;
                uint target = colorScheme?.GetSwap(b) ?? 0;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Pickup,
                    OldColor = source,
                    NewColor = target,
                };
                gfx.ColorSwapsInternal.Add(colorSwap);
            }
        }

        // there's some extra bs here regarding lightsabers
        // it depends on the costume type used with the weapon skin
        // hell
        /*if(baseWeapon == "Katar" && swapDefines is not null)
        {
            // ...
        }*/

        return gfx;
    }

    private static InternalCustomArtImpl FromCustomArtCell(string value, bool grabType, ArtTypeEnum defaultType)
    {
        bool right = grabType && value.StartsWith("RIGHT:");

        ArtTypeEnum type = defaultType;
        if (value.StartsWith("C:")) type = ArtTypeEnum.Costume;
        else if (value.StartsWith("W:")) type = ArtTypeEnum.Weapon;

        string rest = grabType ? value[(value.IndexOf(':') + 1)..] : value;
        string[] parts = rest.Split('/');
        string fileName = parts[0];
        string name = parts[1];

        return new()
        {
            Right = right,
            Type = type,
            FileName = fileName,
            Name = name,
        };
    }

    private static readonly (ColorSchemeSwapEnum, ColorSchemeSwapEnum)[] PickupColorSwapTypes = [
        (ColorSchemeSwapEnum.SpecialAcc, ColorSchemeSwapEnum.SpecialAcc),
        (ColorSchemeSwapEnum.SpecialDk, ColorSchemeSwapEnum.SpecialDk),
        (ColorSchemeSwapEnum.SpecialLt, ColorSchemeSwapEnum.SpecialLt),
        (ColorSchemeSwapEnum.Special, ColorSchemeSwapEnum.SpecialDk),
        (ColorSchemeSwapEnum.SpecialVL, ColorSchemeSwapEnum.SpecialLt),
    ];
}