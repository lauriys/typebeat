// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace typebeat.Game.Overlays
{
    public enum OverlayColourScheme
    {
        Red,
        Orange,
        Lime,
        Green,
        Aquamarine,
        Blue,
        Purple,
        Plum,
        Pink,
    }

    public static class OverlayColourSchemeExtensions
    {
        public static int GetHue(this OverlayColourScheme colourScheme)
        {
            // See https://github.com/ppy/osu-web/blob/5a536d217a21582aad999db50a981003d3ad5659/app/helpers.php#L1620-L1628
            switch (colourScheme)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(colourScheme));

                case OverlayColourScheme.Red:
                    return 0;

                case OverlayColourScheme.Orange:
                    return 45;

                case OverlayColourScheme.Lime:
                    return 90;

                case OverlayColourScheme.Green:
                    return 125;

                case OverlayColourScheme.Aquamarine:
                    return 160;

                case OverlayColourScheme.Blue:
                    return 200;

                case OverlayColourScheme.Purple:
                    // type!beat: "Purple" is osu!'s default overlay scheme (settings, notifications,
                    // login, first-run setup, player loader, ...). osu! tinted every grey in it violet;
                    // here its accents take the Caret lime hue and its greys go neutral (see
                    // HasNeutralGreys), so the brand colour reads as an accent on charcoal, like the
                    // serika-dark gameplay palette, rather than as a lime-washed panel.
                    return 75;

                case OverlayColourScheme.Plum:
                    return 320;

                case OverlayColourScheme.Pink:
                    // type!beat: "Pink" is the default/brand scheme (profile overlay, chat, ...) —
                    // retinted to the Caret lime hue (#c9f24d ≈ 75°) to match the website.
                    return 75;
            }
        }

        /// <summary>
        /// Whether the greys of this scheme (backgrounds, foregrounds, content text) carry almost
        /// none of its hue, leaving the hue to the accents (<see cref="OverlayColourProvider.Colour0"/>
        /// to <see cref="OverlayColourProvider.Colour4"/> and <see cref="OverlayColourProvider.Highlight1"/>).
        /// </summary>
        public static bool HasNeutralGreys(this OverlayColourScheme colourScheme) => colourScheme == OverlayColourScheme.Purple;
    }
}
