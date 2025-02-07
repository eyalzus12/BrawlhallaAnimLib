using System;
using System.Globalization;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public static class WeaponSkinTypesReader
{
    public static WeaponSkinTypesGfxInfo GetGfxTypeInfo(ICsvRow row, ICsvReader costumeTypesReader)
    {
        WeaponSkinTypesGfxInfo info = new();

        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "BaseWeapon")
            {
                info.BaseWeapon = value;
            }
            else if (value == "UseRightGauntlet")
            {
                info.UseRightGauntlet = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (value == "UseRightKatar")
            {
                info.UseRightKatar = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (value == "HideRightPistol2D")
            {
                info.HideRightPistol2D = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select(static (flag) =>
                {
                    if (Enum.TryParse(flag, out BoneTypeEnum result))
                        return 1u << (int)result;
                    return 0u;
                }).Aggregate((a, v) => a | v);

                info.AsymmetrySwapFlags = asf;
            }
            else if (key.StartsWith("CustomArt"))
            {
                ArtTypeEnum defaultType = ArtTypeEnum.Weapon;
                if (key.Contains("Pickup"))
                {
                    defaultType = ArtTypeEnum.Pickup;
                    info.HasPickupCustomArt = true;
                }
                // the game also checks for Costume, but sets to ArtTypeEnum.Weapon instead of ArtTypeEnum.Costume
                // is that a bug?

                info.CustomArtsInternal.Add(FromCustomArtCell(value, true, defaultType));
            }
            else if (key.EndsWith("_Define"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");
                info.SwapDefines[swapType] = uint.Parse(value, CultureInfo.InvariantCulture);
            }
            else if (key == "AttackFxLt_Swap")
            {
                if (value.StartsWith("0x"))
                {
                    uint direct = Convert.ToUInt32(value, 16);
                    info.AttackFxLt_Color = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    info.AttackFxLt_Enum = target;
                }
            }
            else if (key == "AttackFxDk_Swap")
            {
                if (value.StartsWith("0x"))
                {
                    uint direct = Convert.ToUInt32(value, 16);
                    info.AttackFxDk_Color = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    info.AttackFxDk_Enum = target;
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
                        info.SwapDefines.TryAdd(swapType, uint.Parse(value2, CultureInfo.InvariantCulture));
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

        return info;
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
}