using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class ResultPanelView : MonoBehaviour
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button retryButton;

        [SerializeField] private Button creditsButton;

        public Button MenuButton => menuButton;
        public Button CreditsButton => creditsButton;
        public Button RetryButton => retryButton;

        public void ApplyArtwork(UiArtwork artwork, Sprite background)
        {
            UiArtwork.SetBackground(GetComponent<Image>(), background);
            if (retryButton != null)
            {
                artwork.PrimaryButton.Apply(retryButton);
                artwork.SecondaryButton.Apply(menuButton);
            }
            else
            {
                artwork.PrimaryButton.Apply(menuButton);
            }

            artwork.SecondaryButton.Apply(creditsButton);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            (retryButton != null ? retryButton : menuButton).Select();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
