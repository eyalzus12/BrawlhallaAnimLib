namespace BrawlhallaAnimLib.Gfx;

public enum ColorSchemeSwapEnum
{
    HairLt,
    Hair,
    HairDk,
    Body1VL,
    Body1Lt,
    Body1,
    Body1Dk,
    Body1VD,
    Body1Acc,
    Body2VL,
    Body2Lt,
    Body2,
    Body2Dk,
    Body2VD,
    Body2Acc,
    SpecialVL,
    SpecialLt,
    Special,
    SpecialDk,
    SpecialVD,
    SpecialAcc,
    HandsLt,
    HandsDk,
    HandsSkinLt,
    HandsSkinDk,
    ClothVL,
    ClothLt,
    Cloth,
    ClothDk,
    WeaponVL,
    WeaponLt,
    Weapon,
    WeaponDk,
    WeaponAcc,
}

public interface IColorSchemeType
{
    uint GetSwap(ColorSchemeSwapEnum swapType);
}