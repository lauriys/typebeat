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
    /// The heading face (<see cref="Typeface.TorusAlternate"/>) draws in the bundled Inconsolata:
    /// every weight OsuFont asks for ships as a resource and loads as its own face and style.
    /// </summary>
    [TestFixture]
    public class BundledFontsTest
    {
        [Test]
        public void TheHeadingFaceIsInconsolata()
        {
            Assert.That(OsuFont.GetFamilyString(Typeface.TorusAlternate), Is.EqualTo(BundledFonts.INCONSOLATA));
            Assert.That(OsuFont.TorusAlternate.FontName, Is.EqualTo("Inconsolata-Regular"));
            Assert.That(OsuFont.GetFont(Typeface.TorusAlternate, weight: FontWeight.Bold).FontName, Is.EqualTo("Inconsolata-Bold"));
        }

        [Test]
        public void InconsolataKeepsItsMediumWeight()
        {
            // Torus has no medium and falls back to regular; Inconsolata ships one.
            Assert.That(OsuFont.GetWeightString(BundledFonts.INCONSOLATA, FontWeight.Medium), Is.EqualTo("Medium"));
            Assert.That(OsuFont.GetWeightString("Torus", FontWeight.Medium), Is.EqualTo("Regular"));
        }

        [TestCase("Light", FontStyle.Regular)]
        [TestCase("Regular", FontStyle.Regular)]
        [TestCase("Medium", FontStyle.Regular)]
        [TestCase("SemiBold", FontStyle.Regular)]
        [TestCase("Bold", FontStyle.Bold)]
        public void EveryWeightShipsAndLoads(string weight, FontStyle expectedStyle)
        {
            var assembly = typeof(BundledFonts).Assembly;
            string? resource = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith($"Inconsolata-{weight}.ttf", StringComparison.OrdinalIgnoreCase));

            Assert.That(resource, Is.Not.Null, $"Inconsolata-{weight}.ttf is not embedded");

            using var stream = assembly.GetManifestResourceStream(resource!)!;
            new FontCollection().Add(stream, CultureInfo.InvariantCulture, out FontDescription description);

            Assert.That(description.FontFamilyInvariantCulture, Does.StartWith("Inconsolata"));
            Assert.That(description.Style, Is.EqualTo(expectedStyle));
        }
    }
}
