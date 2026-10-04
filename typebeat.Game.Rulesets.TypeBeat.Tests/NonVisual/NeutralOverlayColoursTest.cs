// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;
using osu.Framework.Graphics;
using typebeat.Game.Overlays;
using osuTK.Graphics;

namespace typebeat.Game.Rulesets.TypeBeat.Tests.NonVisual
{
    /// <summary>
    /// The default overlay scheme ("Purple": settings, notifications, login, first-run setup, ...):
    /// lime accents on neutral greys, with the website's ink for text on the accent. Every other
    /// scheme keeps osu!'s tinted greys and white text.
    /// </summary>
    [TestFixture]
    public class NeutralOverlayColoursTest
    {
        private static readonly Color4 ink = new Color4(20, 21, 25, 255); // #141519

        [Test]
        public void TheDefaultSchemeHasLimeAccents()
        {
            var provider = new OverlayColourProvider(OverlayColourScheme.Purple);

            Assert.That(provider.Hue, Is.EqualTo(75));
            Assert.That(provider.Highlight1, Is.EqualTo((Color4)Colour4.FromHSL(75 / 360f, 1, 0.7f)));
            Assert.That(provider.Colour3, Is.EqualTo((Color4)Colour4.FromHSL(75 / 360f, 0.6f, 0.5f)));
        }

        [Test]
        public void TheDefaultSchemeHasNeutralGreys()
        {
            var provider = new OverlayColourProvider(OverlayColourScheme.Purple);

            foreach (var grey in new[] { provider.Background6, provider.Background5, provider.Background1, provider.Foreground1, provider.Dark1, provider.Light1, provider.Content2 })
                Assert.That(spread(grey), Is.LessThan(0.03f), grey.ToString());
        }

        [TestCase(OverlayColourScheme.Blue)]
        [TestCase(OverlayColourScheme.Aquamarine)]
        [TestCase(OverlayColourScheme.Pink)]
        public void OtherSchemesKeepTheirTintedGreys(OverlayColourScheme scheme)
        {
            var provider = new OverlayColourProvider(scheme);

            Assert.That(provider.NeutralGreys, Is.False);
            Assert.That(provider.Background5, Is.EqualTo((Color4)Colour4.FromHSL(scheme.GetHue() / 360f, 0.1f, 0.15f)));
            Assert.That(provider.ForegroundOnAccent, Is.EqualTo(Color4.White));
        }

        [Test]
        public void TextOnTheLimeAccentIsTheWebsitesInk()
            => Assert.That(new OverlayColourProvider(OverlayColourScheme.Purple).ForegroundOnAccent, Is.EqualTo(ink));

        [Test]
        public void ChangingSchemeCarriesTheNeutralGreys()
        {
            // The footer takes on the colours of the overlay it serves (first-run setup is "Purple").
            var footer = new OverlayColourProvider(OverlayColourScheme.Aquamarine);

            footer.ChangeColourScheme(new OverlayColourProvider(OverlayColourScheme.Purple));
            Assert.That(footer.Hue, Is.EqualTo(75));
            Assert.That(footer.NeutralGreys, Is.True);

            footer.ChangeColourScheme(new OverlayColourProvider(OverlayColourScheme.Aquamarine));
            Assert.That(footer.NeutralGreys, Is.False);

            footer.ChangeColourScheme(OverlayColourScheme.Purple);
            Assert.That(footer.NeutralGreys, Is.True);

            // A bare hue (a user's profile colour) is never the neutral scheme.
            footer.ChangeColourScheme(75);
            Assert.That(footer.NeutralGreys, Is.False);
        }

        private static float spread(Color4 c) => Math.Max(c.R, Math.Max(c.G, c.B)) - Math.Min(c.R, Math.Min(c.G, c.B));
    }
}
