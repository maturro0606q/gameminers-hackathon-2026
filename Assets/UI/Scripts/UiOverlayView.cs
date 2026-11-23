using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class UiOverlayView : MonoBehaviour
    {
        [SerializeField] private Button primaryButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        public Button PrimaryButton => primaryButton;
        public Button RestartButton => restartButton;
        public Button MenuButton => menuButton;

        public void ApplyArtwork(UiArtwork artwork, Sprite background)
        {
            UiArtwork.SetBackground(GetComponent<Image>(), background);
            artwork.PrimaryButton.Apply(primaryButton);
            artwork.SecondaryButton.Apply(restartButton);
            artwork.SecondaryButton.Apply(menuButton);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            primaryButton.Select();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
