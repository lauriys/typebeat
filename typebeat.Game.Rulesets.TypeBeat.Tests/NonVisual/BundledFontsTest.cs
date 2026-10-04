// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics.Sprites;
using SixLabors.Fonts;
using typebeat.Game.Graphics;
using typebeat.Game.Graphics.Fonts;

namespace typebeat.Game.Rulesets.TypeBeat.Tests.NonVisual
{
    /// <summary>
    /// The bundled TrueType faces: the heading face (<see cref="Typeface.TorusAlternate"/>) draws in
    /// JetBrains Mono while body text stays in Torus, and every weight OsuFont asks for ships as a
    /// resource and loads as its own face and style.
    /// </summary>
    [TestFixture]
    public class BundledFontsTest
    {
        [Test]
        public void TheHeadingFaceIsJetBrainsMono()
        {
            Assert.That(OsuFont.GetFamilyString(Typeface.TorusAlternate), Is.EqualTo(BundledFonts.JETBRAINS_MONO));
            Assert.That(OsuFont.TorusAlternate.FontName, Is.EqualTo("JetBrainsMono-Regular"));
            Assert.That(OsuFont.GetFont(Typeface.TorusAlternate, weight: FontWeight.Bold).FontName, Is.EqualTo("JetBrainsMono-Bold"));
        }

        [Test]
        public void TheBodyFaceStaysTorus()
        {
            Assert.That(OsuFont.GetFamilyString(Typeface.Torus), Is.EqualTo("Torus"));
            // Torus has no medium, so the default weight resolves to regular.
            Assert.That(OsuFont.Default.FontName, Is.EqualTo("Torus-Regular"));
        }

        [Test]
        public void TheBundledFacesKeepTheirMediumWeight()
        {
            // Torus has no medium and falls back to regular; the bundled faces ship one.
            Assert.That(OsuFont.GetWeightString(BundledFonts.JETBRAINS_MONO, FontWeight.Medium), Is.EqualTo("Medium"));
            Assert.That(OsuFont.GetWeightString("Torus", FontWeight.Medium), Is.EqualTo("Regular"));
        }

        [Test]
        public void EveryWeightShipsAndLoads()
        {
            foreach (string weight in BundledFonts.WEIGHTS)
            {
                string file = BundledFonts.JETBRAINS_MONO_FACES[weight];
                string? resource = BundledFonts.FindResource(BundledFonts.JETBRAINS_MONO, file);

                Assert.That(resource, Is.Not.Null, $"JetBrainsMono-{file}.ttf is not embedded");

                using var stream = typeof(BundledFonts).Assembly.GetManifestResourceStream(resource!)!;
                new FontCollection().Add(stream, CultureInfo.InvariantCulture, out FontDescription description);

                Assert.That(description.FontFamilyInvariantCulture, Does.StartWith("JetBrains Mono"), file);
            }
        }

        [Test]
        public void EachWeightDrawsOneStepHeavier()
        {
            // Regular draws Medium and so on up: a step heavier than the name, to carry
            // Torus-Alternate's weight.
            Assert.That(BundledFonts.JETBRAINS_MONO_FACES["Regular"], Is.EqualTo("Medium"));
            Assert.That(BundledFonts.JETBRAINS_MONO_FACES["Bold"], Is.EqualTo("ExtraBold"));

            // And every weight must draw its own face: a heavier weight inks a wider stem. The advance
            // of a monospace face never changes, so this measures the ink of 'l' instead.
            float previous = 0;

            foreach (string weight in BundledFonts.WEIGHTS)
            {
                string resource = BundledFonts.FindResource(BundledFonts.JETBRAINS_MONO, BundledFonts.JETBRAINS_MONO_FACES[weight])!;

                using var stream = typeof(BundledFonts).Assembly.GetManifestResourceStream(resource)!;
                var fontFamily = new FontCollection().Add(stream, CultureInfo.InvariantCulture, out FontDescription description);
                float stem = TextMeasurer.MeasureBounds("l", new TextOptions(fontFamily.CreateFont(100, description.Style))).Width;

                Assert.That(stem, Is.GreaterThan(previous), weight);
                previous = stem;
            }
        }

        [Test]
        public void TrackingTightensTheAdvance()
        {
            var assembly = typeof(BundledFonts).Assembly;
            string resource = assembly.GetManifestResourceNames().First(n => n.EndsWith($".{BundledFonts.JETBRAINS_MONO}-Regular.ttf", StringComparison.OrdinalIgnoreCase));

            float advance(float tracking)
            {
                using var stream = assembly.GetManifestResourceStream(resource)!;
                var fontFamily = new FontCollection().Add(stream, CultureInfo.InvariantCulture);
                return new RuntimeFontGlyphStore(fontFamily, "JetBrainsMono-Regular", scale: 0.5f, tracking: tracking).Get('a').XAdvance;
            }

            // Render-em is 100 px, so at half scale the face's em is 50 px and -0.1 em takes 5 px off.
            Assert.That(advance(-0.1f), Is.EqualTo(advance(0) - 5).Within(0.001f));
        }
    }
}
