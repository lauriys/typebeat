// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK.Graphics;

namespace typebeat.Game.Overlays
{
    public class OverlayColourProvider
    {
        /// <summary>
        /// The hue degree associated with the colour shades provided by this <see cref="OverlayColourProvider"/>.
        /// </summary>
        public int Hue { get; private set; }

        /// <summary>
        /// Whether the greys provided carry almost none of <see cref="Hue"/> (see
        /// <see cref="OverlayColourSchemeExtensions.HasNeutralGreys"/>).
        /// </summary>
        public bool NeutralGreys { get; private set; }

        public OverlayColourProvider(OverlayColourScheme colourScheme)
            : this(colourScheme.GetHue())
        {
            NeutralGreys = colourScheme.HasNeutralGreys();
        }

        public OverlayColourProvider(int hue)
        {
            Hue = hue;
        }

        // Note that the following five colours are also defined in `OsuColour` as `{colourScheme}{0,1,2,3,4}`.
        // The difference as to which should be used where comes down to context.
        // If the colour in question is supposed to always match the view in which it is displayed theme-wise, use `OverlayColourProvider`.
        // If the colour usage is special and in general differs from the surrounding view in choice of hue, use the `OsuColour` constants.
        public Color4 Colour0 => getColour(1, 0.8f);
        public Color4 Colour1 => getColour(1, 0.7f);
        public Color4 Colour2 => getColour(0.8f, 0.6f);
        public Color4 Colour3 => getColour(0.6f, 0.5f);
        public Color4 Colour4 => getColour(0.4f, 0.3f);

        public Color4 Highlight1 => getColour(1, 0.7f);
        public Color4 Content1 => getGrey(0.4f, 1);
        public Color4 Content2 => getGrey(0.4f, 0.9f);
        public Color4 Light1 => getGrey(0.4f, 0.8f);
        public Color4 Light2 => getGrey(0.4f, 0.75f);
        public Color4 Light3 => getGrey(0.4f, 0.7f);
        public Color4 Light4 => getGrey(0.4f, 0.5f);
        public Color4 Dark1 => getGrey(0.2f, 0.35f);
        public Color4 Dark2 => getGrey(0.2f, 0.3f);
        public Color4 Dark3 => getGrey(0.2f, 0.25f);
        public Color4 Dark4 => getGrey(0.2f, 0.2f);
        public Color4 Dark5 => getGrey(0.2f, 0.15f);
        public Color4 Dark6 => getGrey(0.2f, 0.1f);
        public Color4 Foreground1 => getGrey(0.1f, 0.6f);
        public Color4 Background1 => getGrey(0.1f, 0.4f);
        public Color4 Background2 => getGrey(0.1f, 0.3f);
        public Color4 Background3 => getGrey(0.1f, 0.25f);
        public Color4 Background4 => getGrey(0.1f, 0.2f);
        public Color4 Background5 => getGrey(0.1f, 0.15f);
        public Color4 Background6 => getGrey(0.1f, 0.1f);

        /// <summary>
        /// The colour for text and icons drawn on an accent fill such as <see cref="Colour3"/>. White,
        /// except with <see cref="NeutralGreys"/>: that scheme's lime accent is too light for white
        /// text, so it takes the website's ink, #141519, the colour it sets on its own lime buttons.
        /// </summary>
        public Color4 ForegroundOnAccent => NeutralGreys ? accent_ink : Color4.White;

        private static readonly Color4 accent_ink = new Color4(20, 21, 25, 255);

        /// <summary>
        /// Changes the <see cref="Hue"/> to a different degree.
        /// Note that this does not trigger any kind of signal to any drawable that received colours from here, all drawables need to be updated manually.
        /// </summary>
        /// <param name="colourScheme">The proposed colour scheme.</param>
        public void ChangeColourScheme(OverlayColourScheme colourScheme)
        {
            Hue = colourScheme.GetHue();
            NeutralGreys = colourScheme.HasNeutralGreys();
        }

        /// <summary>
        /// Takes on the colour scheme of another <see cref="OverlayColourProvider"/>, its <see cref="NeutralGreys"/> included.
        /// Note that this does not trigger any kind of signal to any drawable that received colours from here, all drawables need to be updated manually.
        /// </summary>
        /// <param name="other">The provider whose scheme to take on.</param>
        public void ChangeColourScheme(OverlayColourProvider other)
        {
            Hue = other.Hue;
            NeutralGreys = other.NeutralGreys;
        }

        /// <summary>
        /// Changes the <see cref="Hue"/> to a different degree.
        /// Note that this does not trigger any kind of signal to any drawable that received colours from here, all drawables need to be updated manually.
        /// </summary>
        /// <param name="hue">The proposed hue degree.</param>
        public void ChangeColourScheme(int hue)
        {
            Hue = hue;
            NeutralGreys = false;
        }

        private Color4 getColour(float saturation, float lightness) => osu.Framework.Graphics.Colour4.FromHSL(Hue / 360f, saturation, lightness);

        // A tenth of the saturation keeps the faintest trace of the hue, so the greys still sit with the accent.
        private Color4 getGrey(float saturation, float lightness) => getColour(NeutralGreys ? saturation / 10 : saturation, lightness);
    }
}
