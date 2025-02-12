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
    // those are hardcoded for costume types. color swaps can't change those.
    AttackFxLt,
    AttackFxDk,
    // those are hardcoded for weapon skin types. color swaps can't change those.
    RHandsLt,
    RHandsDk,
    RHandsSkinLt,
    RHandsSkinDk,
}

public interface IColorSchemeType
{
    string Name { get; }
    uint GetSwap(ColorSchemeSwapEnum swapType);
}