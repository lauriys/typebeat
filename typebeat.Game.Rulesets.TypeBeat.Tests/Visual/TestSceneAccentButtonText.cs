// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using typebeat.Game.Graphics;
using typebeat.Game.Graphics.UserInterface;
using typebeat.Game.Graphics.UserInterfaceV2;
using typebeat.Game.Overlays;
using typebeat.Game.Screens.Footer;
using typebeat.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;

namespace typebeat.Game.Rulesets.TypeBeat.Tests.Visual
{
    /// <summary>
    /// Text and icons on accent-filled buttons take the scheme's <see cref="OverlayColourProvider.ForegroundOnAccent"/>:
    /// a deep olive ink on the default scheme's lime, white on osu!'s schemes, and white again
    /// wherever a button brings its own background colour. The back buttons wear the violet of
    /// osu!'s pink slot with white text.
    /// </summary>
    [TestFixture]
    public partial class TestSceneAccentButtonText : OsuTestScene
    {
        private static readonly Color4 ink = new Color4(0x30, 0x39, 0x13, 255);

        private RoundedButton accentButton = null!;
        private RoundedButton localButton = null!;
        private FormButton formButton = null!;

        private void create(OverlayColourScheme scheme) => AddStep($"create buttons on {scheme}", () =>
        {
            Child = new DependencyProvidingContainer
            {
                RelativeSizeAxes = Axes.Both,
                CachedDependencies = new (System.Type, object)[] { (typeof(OverlayColourProvider), new OverlayColourProvider(scheme)) },
                Child = new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Width = 400,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        accentButton = new RoundedButton { RelativeSizeAxes = Axes.X, Text = "accent" },
                        localButton = new RoundedButton { RelativeSizeAxes = Axes.X, Text = "local", BackgroundColour = new OsuColour().YellowDark },
                        formButton = new FormButton { Caption = "form", Action = () => { } },
                    }
                }
            };
        });

        [Test]
        public void TestDefaultSchemeTakesTheInk()
        {
            create(OverlayColourScheme.Purple);
            AddAssert("accent button text is ink", () => text(accentButton) == ink);
            AddAssert("local background keeps white", () => text(localButton) == Color4.White);
            AddAssert("form button icon is ink", () => icon(formButton).Colour == ink);
            AddAssert("ink icon drops the shadow", () => !icon(formButton).Shadow);
        }

        [Test]
        public void TestOtherSchemesStayWhite()
        {
            create(OverlayColourScheme.Blue);
            AddAssert("accent button text is white", () => text(accentButton) == Color4.White);
            AddAssert("form button icon is white", () => icon(formButton).Colour == Color4.White);
            AddAssert("white icon keeps the shadow", () => icon(formButton).Shadow);
        }

        [Test]
        public void TestALaterLocalBackgroundRestoresWhite()
        {
            create(OverlayColourScheme.Purple);
            AddStep("give the accent button its own colour", () => accentButton.BackgroundColour = new OsuColour().Red2);
            AddAssert("text is white again", () => text(accentButton) == Color4.White);
        }

        [Test]
        public void TestBackButtonsAreViolet()
        {
            ScreenBackButton screenBack = null!;
            BackButton back = null!;

            AddStep("create back buttons", () => Child = new DependencyProvidingContainer
            {
                RelativeSizeAxes = Axes.Both,
                CachedDependencies = new (System.Type, object)[] { (typeof(OverlayColourProvider), new OverlayColourProvider(OverlayColourScheme.Purple)) },
                Children = new Drawable[]
                {
                    screenBack = new ScreenBackButton { Anchor = Anchor.TopLeft, Origin = Anchor.TopLeft, Action = () => { } },
                    back = new BackButton { Anchor = Anchor.BottomLeft, Origin = Anchor.BottomLeft, State = { Value = Visibility.Visible } },
                }
            });
            AddUntilStep("footer back button is violet", () => screenBack.ChildrenOfType<Box>().Any(b => b.Colour.Equals((ColourInfo)new OsuColour().Pink2)));
            AddAssert("legacy back button is violet", () => back.ChildrenOfType<TwoLayerButton>().Single().HoverColour == new OsuColour().PinkDark);
        }

        private static Color4 text(RoundedButton button) => button.ChildrenOfType<SpriteText>().Single().Colour;

        private static SpriteIcon icon(FormButton button) => button.ChildrenOfType<FormButton.Button>().Single().ChildrenOfType<SpriteIcon>().Single();
    }
}
