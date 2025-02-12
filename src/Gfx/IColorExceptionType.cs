using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Gfx;

public enum ColorExceptionMode
{
    Costume,
    Weapon
}

public interface IColorExceptionType
{
    string TargetName { get; }
    string ColorSchemeName { get; }
    ColorExceptionMode Mode { get; }

    bool TryGetSwapRedirect(ColorSchemeSwapEnum swap, out ColorSchemeSwapEnum newSwap);
}

public interface IColorExceptionTypes
{
    bool TryGetColorException(string targetName, string colorSchemeName, ColorExceptionMode mode, [MaybeNullWhen(false)] out IColorExceptionType exception);
}