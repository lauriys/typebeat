// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
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
        /// The weights OsuFont asks for. Each registers as a separate face named "{family}-{weight}".
        /// </summary>
        public static readonly string[] WEIGHTS = { "Light", "Regular", "Medium", "SemiBold", "Bold" };

        /// <summary>
        /// Which of JetBrains Mono's files draws each weight OsuFont asks for: each one step heavier than
        /// its name, so the headings carry Torus-Alternate's weight beside Torus.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> JETBRAINS_MONO_FACES = new Dictionary<string, string>
        {
            ["Light"] = "Regular",
            ["Regular"] = "Medium",
            ["Medium"] = "SemiBold",
            ["SemiBold"] = "Bold",
            ["Bold"] = "ExtraBold",
        };

        /// <summary>
        /// Registers every bundled weight of every bundled family with <paramref name="fonts"/>.
        /// </summary>
        public static void AddAll(FontStore fonts)
        {
            add(fonts, JETBRAINS_MONO, JETBRAINS_MONO_FACES, JETBRAINS_MONO_SCALE, JETBRAINS_MONO_TRACKING);
        }

        /// <summary>
        /// JetBrains Mono runs larger than Torus-Alternate at the same font size. Drawn at this scale it
        /// reads at Torus-Alternate's size beside the Torus body text; being monospace, a line still runs
        /// a little wider.
        /// </summary>
        public const float JETBRAINS_MONO_SCALE = 0.72f;

        /// <summary>
        /// JetBrains Mono's letter spacing, in ems. A coding face spaces its letters wide for legibility
        /// in code; set as headings beside Torus that read a touch loose, so its letters draw a little closer.
        /// </summary>
        public const float JETBRAINS_MONO_TRACKING = -0.025f;

        /// <summary>
        /// The embedded resource holding <paramref name="family"/>'s file for <paramref name="fileWeight"/>, if bundled.
        /// </summary>
        public static string? FindResource(string family, string fileWeight)
        {
            string file = $".{family}-{fileWeight}.ttf";
            return typeof(BundledFonts).Assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(file, StringComparison.OrdinalIgnoreCase));
        }

        // A missing or unreadable file is logged and skipped: text in that weight falls back to the next face.
        private static void add(FontStore fonts, string family, IReadOnlyDictionary<string, string> faces, float scale = 1, float tracking = 0)
        {
            var assembly = typeof(BundledFonts).Assembly;

            foreach (string weight in WEIGHTS)
            {
                string fontName = $"{family}-{weight}";

                try
                {
                    string? resource = FindResource(family, faces[weight]);

                    if (resource == null)
                    {
                        Logger.Log($"The bundled font file {family}-{faces[weight]} for {fontName} is missing.", level: LogLevel.Error);
                        continue;
                    }

                    // One collection per weight: the static weights share a family name, and in one
                    // collection all but one would be hidden behind the same family and style.
                    var collection = new FontCollection();

                    using (var stream = assembly.GetManifestResourceStream(resource)!)
                    {
                        FontFamily fontFamily = collection.Add(stream, CultureInfo.InvariantCulture, out FontDescription description);
                        fonts.AddTextureSource(new RuntimeFontGlyphStore(fontFamily, fontName, description.Style, scale, tracking));
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
