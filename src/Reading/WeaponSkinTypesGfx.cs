using System;
using System.Collections.Generic;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class WeaponSkinTypesGfx
{
    private static readonly (ColorSchemeSwapEnum, ColorSchemeSwapEnum)[] PickupColorSwapTypes = [
        (ColorSchemeSwapEnum.SpecialAcc, ColorSchemeSwapEnum.SpecialAcc),
        (ColorSchemeSwapEnum.SpecialDk, ColorSchemeSwapEnum.SpecialDk),
        (ColorSchemeSwapEnum.SpecialLt, ColorSchemeSwapEnum.SpecialLt),
        (ColorSchemeSwapEnum.Special, ColorSchemeSwapEnum.SpecialDk),
        (ColorSchemeSwapEnum.SpecialVL, ColorSchemeSwapEnum.SpecialLt),
    ];

    private static readonly (ColorSchemeSwapEnum swapType, uint source, uint fallbackTarget)[] KatarsLightsaberSwapTypes = [
        (ColorSchemeSwapEnum.HandsLt, 0x54ABEB, 0),
        (ColorSchemeSwapEnum.HandsSkinLt, 0x54ABEB, 0xFFCC99),
        (ColorSchemeSwapEnum.HandsDk, 0xBFFFFC, 0),
        (ColorSchemeSwapEnum.HandsSkinDk, 0xBFFFFC, 0xFF926C),
        (ColorSchemeSwapEnum.HandsSkinLt, 0xFFCC99, 0),
        (ColorSchemeSwapEnum.HandsSkinDk, 0xFFCC99, 0),
    ];

    internal string WeaponSkinName { get; }
    internal uint AsymmetrySwapFlags { get; } = 0;
    internal bool UseRightGauntlet { get; } = false;
    internal bool UseRightKatar { get; } = false;
    internal bool HideRightPistol2D { get; } = false;
    internal List<InternalCustomArtImpl> BaseCustomArts { get; } = [];

    internal bool HasPickupCustomArt { get; } = false;
    internal Dictionary<ColorSchemeSwapEnum, uint> SwapDefines { get; } = [];
    internal string? BaseWeapon { get; } = null;

    internal ColorSchemeSwapEnum? AttackFxLt_Enum { get; } = null;
    internal uint AttackFxLt_Color { get; } = 0;
    internal ColorSchemeSwapEnum? AttackFxDk_Enum { get; } = null;
    internal uint AttackFxDk_Color { get; } = 0;

    public WeaponSkinTypesGfx(ICsvRow row, ICsvReader costumeTypesReader)
    {
        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "WeaponSkinName")
            {
                WeaponSkinName = value;
            }
            else if (key == "BaseWeapon")
            {
                BaseWeapon = value;
            }
            else if (key == "UseRightGauntlet")
            {
                UseRightGauntlet = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightKatar")
            {
                UseRightKatar = ParserUtils.ParseBool(value);
            }
            else if (key == "HideRightPistol2D")
            {
                HideRightPistol2D = ParserUtils.ParseBool(value);
            }
            else if (key == "AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select(static (flag) =>
                {
                    if (Enum.TryParse(flag, out BoneTypeEnum result))
                        return 1u << (int)result;
                    return 0u;
                }).Aggregate((a, v) => a | v);

                AsymmetrySwapFlags = asf;
            }
            else if (key.StartsWith("CustomArt"))
            {
                ArtTypeEnum defaultType = ArtTypeEnum.Weapon;
                if (key.Contains("Pickup"))
                {
                    defaultType = ArtTypeEnum.Pickup;
                    HasPickupCustomArt = true;
                }
                // the game also checks for Costume, but sets to ArtTypeEnum.Weapon instead of ArtTypeEnum.Costume
                // is that a bug?

                BaseCustomArts.Add(ParserUtils.ParseCustomArt(value, true, defaultType));
            }
            else if (key.EndsWith("_Define"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");
                uint define = ParserUtils.ParseHexString(value);
                SwapDefines[swapType] = define;
            }
            else if (key == "AttackFxLt_Swap")
            {
                if (value.StartsWith("0x"))
                {
                    uint direct = ParserUtils.ParseHexString(value);
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
                    uint direct = ParserUtils.ParseHexString(value);
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
                    if (value2 == "") continue;

                    if (key2.EndsWith("_Define"))
                    {
                        string swap = key2[..^"_Define".Length];
                        if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                            throw new ArgumentException($"Invalid swap {swap}");
                        SwapDefines.TryAdd(swapType, ParserUtils.ParseHexString(value2));
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

        if (WeaponSkinName is null) throw new ArgumentException("Missing WeaponSkinName");
    }

    public IGfxType ToGfxType(IGfxType gfxType, IColorSchemeType? colorScheme = null, IColorExceptionTypes? colorExceptions = null, CostumeTypesGfx? costumeType = null)
    {
        // weapon skin types only modify the existing gfx
        InternalGfxImpl gfxResult = new(gfxType)
        {
            UseRightGauntlet = UseRightGauntlet,
            UseRightKatar = UseRightKatar,
            HideRightPistol2D = HideRightPistol2D,
        };
        gfxResult.AsymmetrySwapFlags |= AsymmetrySwapFlags;
        gfxResult.CustomArtsInternal.AddRange(BaseCustomArts);

        ColorSchemeSwapEnum[] swapTypesList = Enum.GetValues<ColorSchemeSwapEnum>();
        if (colorScheme is not null)
        {
            IColorExceptionType? colorException = null;
            colorExceptions?.TryGetColorException(
                WeaponSkinName, colorScheme.Name, ColorExceptionMode.Weapon,
                out colorException
            );

            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                // source is define
                uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                if (sourceColor == 0) continue;
                // color exception
                ColorSchemeSwapEnum targetSwapType = swapType;
                if (colorException?.TryGetSwapRedirect(swapType, out ColorSchemeSwapEnum newSwapType) ?? false)
                {
                    if (colorScheme.GetSwap(newSwapType) != 0)
                        targetSwapType = newSwapType;
                }
                // target from scheme
                uint targetColor = colorScheme.GetSwap(targetSwapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
        }

        InternalColorSwapImpl? getColorSwap(ColorSchemeSwapEnum? swap, uint color, uint sourceColor)
        {
            if (swap is not null)
            {
                ColorSchemeSwapEnum swapType = swap.Value;
                uint targetColor = colorScheme?.GetSwap(swapType) ?? 0;
                if (targetColor == 0)
                {
                    uint definedColor = SwapDefines.GetValueOrDefault(swapType, 0u);
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
        if (AttackFxLt is not null) gfxResult.ColorSwapsInternal.Add(AttackFxLt);
        InternalColorSwapImpl? AttackFxDk = getColorSwap(AttackFxDk_Enum, AttackFxDk_Color, 0x004DCC);
        if (AttackFxDk is not null) gfxResult.ColorSwapsInternal.Add(AttackFxDk);

        if (HasPickupCustomArt)
        {
            foreach ((ColorSchemeSwapEnum a, ColorSchemeSwapEnum b) in PickupColorSwapTypes)
            {
                uint source = SwapDefines.GetValueOrDefault(a, 0u);
                uint target = colorScheme?.GetSwap(b) ?? 0;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Pickup,
                    OldColor = source,
                    NewColor = target,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
        }

        if (BaseWeapon == "Katar" && costumeType is not null && costumeType.SwapDefines is not null)
        {
            uint getSwap(ColorSchemeSwapEnum swapType)
            {
                if (colorScheme is not null) return colorScheme.GetSwap(swapType);
                return costumeType.SwapDefines.GetValueOrDefault(swapType, 0u);
            }

            foreach ((ColorSchemeSwapEnum swapType, uint source, uint fallbackTarget) in KatarsLightsaberSwapTypes)
            {
                uint target = 0;
                // indirect
                if (costumeType.IndirectSwaps.TryGetValue(swapType, out ColorSchemeSwapEnum newSwap))
                {
                    uint newTarget = getSwap(newSwap);
                    if (newTarget != 0) target = newTarget;
                }
                // direct
                if (target == 0)
                {
                    if (costumeType.DirectSwaps.TryGetValue(swapType, out uint newTarget))
                    {
                        target = newTarget;
                    }
                }
                // fallback
                if (target == 0)
                {
                    target = fallbackTarget;
                }

                if (target != 0)
                {
                    InternalColorSwapImpl colorSwap = new()
                    {
                        ArtType = ArtTypeEnum.Weapon,
                        OldColor = source,
                        NewColor = target,
                    };
                    gfxResult.ColorSwapsInternal.Add(colorSwap);
                }
            }
        }

        return gfxResult;
    }
}