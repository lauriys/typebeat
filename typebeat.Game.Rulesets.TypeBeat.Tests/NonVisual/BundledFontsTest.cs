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
    /// The bundled TrueType faces: the body face (<see cref="Typeface.Torus"/>) draws in Nunito and
    /// the heading face (<see cref="Typeface.TorusAlternate"/>) in JetBrains Mono, and every weight
    /// OsuFont asks for ships as a resource and loads as its own face and style.
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
        public void TheBodyFaceIsNunito()
        {
            Assert.That(OsuFont.GetFamilyString(Typeface.Torus), Is.EqualTo(BundledFonts.NUNITO));
            Assert.That(OsuFont.Default.FontName, Is.EqualTo("Nunito-Medium"));
            Assert.That(OsuFont.GetFont(weight: FontWeight.Light).FontName, Is.EqualTo("Nunito-Light"));
        }

        [Test]
        public void TheBundledFacesKeepTheirMediumWeight()
        {
            // Torus has no medium and falls back to regular; the bundled faces ship one.
            Assert.That(OsuFont.GetWeightString(BundledFonts.JETBRAINS_MONO, FontWeight.Medium), Is.EqualTo("Medium"));
            Assert.That(OsuFont.GetWeightString(BundledFonts.NUNITO, FontWeight.Medium), Is.EqualTo("Medium"));
            Assert.That(OsuFont.GetWeightString("Torus", FontWeight.Medium), Is.EqualTo("Regular"));
        }

        [TestCase(BundledFonts.JETBRAINS_MONO, "JetBrains Mono")]
        [TestCase(BundledFonts.NUNITO, "Nunito")]
        public void EveryWeightShipsAndLoads(string family, string familyName)
        {
            var assembly = typeof(BundledFonts).Assembly;

            foreach (string weight in BundledFonts.WEIGHTS)
            {
                string fontName = $"{family}-{weight}";
                string? resource = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith($".{fontName}.ttf", StringComparison.OrdinalIgnoreCase));

                Assert.That(resource, Is.Not.Null, $"{fontName}.ttf is not embedded");

                using var stream = assembly.GetManifestResourceStream(resource!)!;
                new FontCollection().Add(stream, CultureInfo.InvariantCulture, out FontDescription description);

                Assert.That(description.FontFamilyInvariantCulture, Does.StartWith(familyName), fontName);
            }
        }

        [TestCase(BundledFonts.JETBRAINS_MONO)]
        [TestCase(BundledFonts.NUNITO)]
        public void EachWeightIsHeavierThanTheLast(string family)
        {
            // Every weight must draw its own face: a heavier weight inks a wider stem. The advance
            // of a monospace face never changes, so this measures the ink of 'l' instead.
            var assembly = typeof(BundledFonts).Assembly;
            float previous = 0;

            foreach (string weight in BundledFonts.WEIGHTS)
            {
                string resource = assembly.GetManifestResourceNames().First(n => n.EndsWith($".{family}-{weight}.ttf", StringComparison.OrdinalIgnoreCase));

                using var stream = assembly.GetManifestResourceStream(resource)!;
                var fontFamily = new FontCollection().Add(stream, CultureInfo.InvariantCulture, out FontDescription description);
                var font = fontFamily.CreateFont(100, description.Style);
                float stem = TextMeasurer.MeasureBounds("l", new TextOptions(font)).Width;

                Assert.That(stem, Is.GreaterThan(previous), $"{family}-{weight}");
                previous = stem;
            }
        }
    }
}
