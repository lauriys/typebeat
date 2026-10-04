// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osuTK;
using typebeat.Game.Tests.Visual;

namespace typebeat.Game.Rulesets.TypeBeat.Tests.Visual
{
    /// <summary>
    /// The ruleset icon is a texture shipped inside the ruleset assembly, so a renamed PNG or a
    /// broken resource path would not fail the build; the icon would just draw nothing. This loads
    /// two icons the way the toolbar does and checks they found the texture, share it, and keep the
    /// fixed 20px footprint the old solid circle had.
    /// </summary>
    [TestFixture]
    public partial class TestSceneTypeBeatRulesetIcon : OsuTestScene
    {
        private Drawable first = null!;
        private Drawable second = null!;

        [Test]
        public void TestIconLoadsSharedTexture()
        {
            AddStep("add icons", () => Child = new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Spacing = new Vector2(10),
                Children = new[]
                {
                    first = new TypeBeatRuleset().CreateIcon(),
                    second = new TypeBeatRuleset().CreateIcon(),
                }
            });

            AddUntilStep("both icons loaded", () => first.IsLoaded && second.IsLoaded);
            AddAssert("texture found", () => spriteOf(first).Texture, () => Is.Not.Null);
            AddAssert("icons share one texture", () => spriteOf(second).Texture, () => Is.SameAs(spriteOf(first).Texture));
            AddAssert("fixed 20px size", () => first.DrawSize, () => Is.EqualTo(new Vector2(20)));
        }

        private static Sprite spriteOf(Drawable icon) => icon.ChildrenOfType<Sprite>().Single();
    }
}
