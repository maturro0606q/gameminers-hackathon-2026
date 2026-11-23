using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.Gameplay
{
    internal static class CookieUi
    {
        internal static readonly Color Berry = new Color32(147, 36, 76, 255);
        internal static readonly Color Ink = new Color32(72, 42, 48, 255);
        private static readonly Color Dim = new Color32(46, 28, 41, 121);
        private static Font font;
        private static Sprite frame, badge;

        internal static Font Hand
        {
            get
            {
                if (font == null)
                {
                    var paper = Font.CreateDynamicFontFromOSFont(new[] { "Segoe Print", "Comic Sans MS", "Arial" }, 24);
                    font = paper != null && paper.material != null ? paper : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                return font;
            }
        }

        internal static void Frame(Image image, bool button = false)
        {
            if (image == null) return;
            if (frame == null) frame = Load("CookiePanel.png", 45);
            if (button && badge == null) badge = Load("CookieButton.png", 25);
            image.sprite = button ? badge : frame;
            image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            image.pixelsPerUnitMultiplier = button ? 1.64f : 1.34f;
            image.color = image.sprite != null ? Color.white : new Color32(243, 211, 159, 255);
        }

        internal static void Apply(Transform root)
        {
            foreach (var image in root.GetComponentsInChildren<Image>(true))
            {
                string n = image.name;
                if (n == "Accent" || n == "Borde superior") image.enabled = false;
                else if (image.GetComponent<Button>() != null || n == "KeyBadge")
                {
                    Frame(image, true);
                    if (image.TryGetComponent<Button>(out var button))
                    {
                        var colors = button.colors;
                        colors.normalColor = Color.white;
                        colors.highlightedColor = new Color32(255, 232, 205, 255);
                        colors.selectedColor = colors.highlightedColor;
                        colors.pressedColor = new Color32(232, 195, 170, 255);
                        button.colors = colors;
                    }
                }
                else if (n == "MainPanel" || n == "ControlsPanel" || n == "DefeatPanel(Clone)" || n == "EndingPanel(Clone)")
                    image.color = Dim;
                else if (n == "Tarjeta" || n == "Card" || n == "MenuCard" || n == "ControlsCard" ||
                    n == "Objective" || n == "Prompt" || n == "BossPanel" || n == "Health")
                {
                    Frame(image, n == "Objective" || n == "Prompt" || n == "BossPanel" || n == "Health");
                }
                else if (n == "ShopCard" || n == "CombatCard")
                    image.color = new Color32(154, 75, 73, 31);
            }

            foreach (var text in root.GetComponentsInChildren<Text>(true))
            {
                text.font = Hand;
                text.color = text.name == "Title" || text.name == "Heading" ? Berry : Ink;
                if (text.name == "Title")
                {
                    text.fontStyle = FontStyle.Bold;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 24;
                    text.resizeTextMaxSize = Mathf.Max(26, Mathf.Min(62, text.fontSize));
                }
                else if (text.name != "Bindings")
                {
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = Mathf.Min(13, text.fontSize);
                    text.resizeTextMaxSize = text.fontSize;
                }
            }
        }

        private static Sprite Load(string file, float edge)
        {
            var path = PortableModel.PathFor(file);
            if (!File.Exists(path)) return null;
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(File.ReadAllBytes(path), true)) return null;
            texture.wrapMode = TextureWrapMode.Clamp;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(edge, edge, edge, edge));
        }
    }
}