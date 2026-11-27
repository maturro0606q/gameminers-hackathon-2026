using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [CreateAssetMenu(menuName = "GameMiners/UI/Artwork", fileName = "UiArtwork")]
    public sealed class UiArtwork : ScriptableObject
    {
        public Sprite MenuBackground;
        public Sprite ControlsBackground;
        public Sprite PauseBackground;
        public Sprite DefeatBackground;
        public Sprite EndingBackground;
        public Sprite CreditsBackground;
        public ButtonArtwork PrimaryButton;
        public ButtonArtwork SecondaryButton;

        internal static void SetBackground(Image image, Sprite sprite)
        {
            if (image == null || sprite == null)
                return;

            image.sprite = sprite;
            image.color = Color.white;
            image.type = Image.Type.Simple;
        }
    }

    [Serializable]
    public struct ButtonArtwork
    {
        public Sprite Normal;
        public Sprite Highlighted;
        public Sprite Pressed;
        public Sprite Selected;
        public Sprite Disabled;

        internal void Apply(Button button)
        {
            if (button == null || Normal == null || button.targetGraphic is not Image image)
                return;

            if (image.sprite != null)
                return;

            image.sprite = Normal;
            image.color = Color.white;
            image.type = Image.Type.Simple;
            if (Highlighted == null && Pressed == null && Selected == null && Disabled == null)
                return;

            button.spriteState = new SpriteState
            {
                highlightedSprite = Highlighted != null ? Highlighted : Normal,
                pressedSprite = Pressed != null ? Pressed : Normal,
                selectedSprite = Selected != null ? Selected : Highlighted != null ? Highlighted : Normal,
                disabledSprite = Disabled != null ? Disabled : Normal
            };
            button.transition = Selectable.Transition.SpriteSwap;
        }
    }
}
