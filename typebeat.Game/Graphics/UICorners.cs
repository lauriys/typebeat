// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace typebeat.Game.Graphics
{
    /// <summary>
    /// type!beat: the one corner radius for UI chrome: buttons, fields, dropdowns, form cards, panels,
    /// popovers, tooltips, dialogs and notifications. osu! rounds these anywhere from 5 to 20 px, set
    /// per component; type!beat squares them off to a single tighter radius, closer to a terminal than
    /// to osu!'s soft cards. Pills (slider tracks, toggles, nubs), circles and the gameplay HUD keep
    /// their own shapes.
    /// </summary>
    public static class UICorners
    {
        public const float RADIUS = 4;
    }
}
