// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Globalization;
using System.Linq;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using SixLabors.Fonts;

namespace typebeat.Game.Graphics.Fonts
{
    /// <summary>
    /// type!beat's UI faces shipped as TrueType files in <c>Resources/Fonts</c> rather than in the
    /// packaged BMFont resources, rasterised at runtime through <see cref="RuntimeFontGlyphStore"/>.
    /// Each family is licensed under the SIL OFL 1.1; its licence sits beside its files.
    /// </summary>
    public static class BundledFonts
    {
        /// <summary>
        /// JetBrains Mono: the monospace heading face, in place of osu!'s stylised Torus-Alternate.
        /// A typing game's titles in a terminal's type.
        /// </summary>
        public const string JETBRAINS_MONO = "JetBrainsMono";

        /// <summary>
        /// Nunito: the body face, in place of osu!'s Torus; rounded like it, but softer. Its static
        /// weights are instances cut from the family's variable font, which the runtime renderer
        /// cannot vary: Light 300, Regular 400, Medium 500, and SemiBold and Bold at Nunito's own 700
        /// and 800, one step heavier than their names, since Torus's bold runs much heavier than
        /// Nunito's and button labels lost their weight.
        /// </summary>
        public const string NUNITO = "Nunito";

        /// <summary>
        /// The weights OsuFont asks for. Each is a separate static file and face named "{family}-{weight}".
        /// </summary>
        public static readonly string[] WEIGHTS = { "Light", "Regular", "Medium", "SemiBold", "Bold" };

        /// <summary>
        /// Registers every bundled weight of every bundled family with <paramref name="fonts"/>.
        /// </summary>
        public static void AddAll(FontStore fonts)
        {
            add(fonts, NUNITO, NUNITO_SCALE);
            add(fonts, JETBRAINS_MONO, JETBRAINS_MONO_SCALE);
        }

        /// <summary>
        /// Nunito runs about 15% larger than Torus at the same font size (wider letters, a taller x-height),
        /// which overflowed layouts built around Torus. Drawn at this scale, a line of it takes Torus's width.
        /// </summary>
        public const float NUNITO_SCALE = 0.87f;

        /// <summary>
        /// JetBrains Mono runs about 25% taller than Torus-Alternate at the same font size. Drawn at this
        /// scale its letters stand as tall as Torus-Alternate's did; being monospace, a line still runs a
        /// little wider.
        /// </summary>
        public const float JETBRAINS_MONO_SCALE = 0.8f;

        // A missing or unreadable file is logged and skipped: text in that weight falls back to the next face.
        private static void add(FontStore fonts, string family, float scale = 1)
        {
            var assembly = typeof(BundledFonts).Assembly;
            string[] resources = assembly.GetManifestResourceNames();

            foreach (string weight in WEIGHTS)
            {
                string fontName = $"{family}-{weight}";

                try
                {
                    string? resource = resources.FirstOrDefault(n => n.EndsWith($".{fontName}.ttf", StringComparison.OrdinalIgnoreCase));

                    if (resource == null)
                    {
                        Logger.Log($"The bundled font {fontName} is missing.", level: LogLevel.Error);
                        continue;
                    }

                    // One collection per weight: the static weights share a family name, and in one
                    // collection all but one would be hidden behind the same family and style.
                    var collection = new FontCollection();

                    using (var stream = assembly.GetManifestResourceStream(resource)!)
                    {
                        FontFamily fontFamily = collection.Add(stream, CultureInfo.InvariantCulture, out FontDescription description);
                        fonts.AddTextureSource(new RuntimeFontGlyphStore(fontFamily, fontName, description.Style, scale));
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Could not load the bundled font {fontName}.");
                }
            }
        }
    }
}
