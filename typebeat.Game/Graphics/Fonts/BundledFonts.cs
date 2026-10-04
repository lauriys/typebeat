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
    /// </summary>
    public static class BundledFonts
    {
        /// <summary>
        /// Inconsolata (SIL OFL 1.1, see <c>Resources/Fonts/Inconsolata/Inconsolata-OFL.txt</c>): the
        /// monospace heading face, in place of osu!'s stylised Torus-Alternate. A typing game's titles
        /// in a terminal's type.
        /// </summary>
        public const string INCONSOLATA = "Inconsolata";

        // The weights OsuFont asks for; each file is a separate face named "{family}-{weight}".
        private static readonly string[] inconsolata_weights = { "Light", "Regular", "Medium", "SemiBold", "Bold" };

        /// <summary>
        /// Registers every bundled Inconsolata weight with <paramref name="fonts"/>. A missing or
        /// unreadable file is logged and skipped: text in that weight falls back to the default face.
        /// </summary>
        public static void AddInconsolata(FontStore fonts)
        {
            var assembly = typeof(BundledFonts).Assembly;
            string[] resources = assembly.GetManifestResourceNames();

            foreach (string weight in inconsolata_weights)
            {
                string fontName = $"{INCONSOLATA}-{weight}";

                try
                {
                    string? resource = resources.FirstOrDefault(n => n.EndsWith($"{fontName}.ttf", StringComparison.OrdinalIgnoreCase));

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
                        FontFamily family = collection.Add(stream, CultureInfo.InvariantCulture, out FontDescription description);
                        fonts.AddTextureSource(new RuntimeFontGlyphStore(family, fontName, description.Style));
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
