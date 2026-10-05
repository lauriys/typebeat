// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace typebeat.Game.Graphics
{
    /// <summary>
    /// type!beat: the one corner radius for UI chrome: buttons, fields, dropdowns, form cards, panels,
    /// popovers, tooltips, dialogs and notifications. osu! rounds these anywhere from 5 to 20 px, set
    /// per component; type!beat squares them off entirely, like a terminal rather than osu!'s soft
    /// cards. Pills (slider tracks, toggles, nubs), circles and the gameplay HUD keep their own shapes.
    /// </summary>
    /// <remarks>
    /// Layouts inherited from osu! overlap neighbouring pieces by their corner radius to hide the seam
    /// under a rounded corner; at zero those pieces simply meet edge to edge. Anything that derives an
    /// inner radius from this one must clamp it at zero.
    /// </remarks>
    public static class UICorners
    {
        public const float RADIUS = 0;
    }
}
