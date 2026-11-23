using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private Button creditsButton;

        public Button PlayButton => playButton;
        public Button CreditsButton => creditsButton;

        private void Awake()
        {
            controlsButton.onClick.AddListener(ShowControls);
            backButton.onClick.AddListener(ShowMain);
            quitButton.onClick.AddListener(Application.Quit);
            quitButton.gameObject.SetActive(Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor);
        }

        private void OnEnable()
        {
            ShowMain();
        }

        public void ApplyArtwork(UiArtwork artwork)
        {
            UiArtwork.SetBackground(mainPanel.GetComponent<Image>(), artwork.MenuBackground);
            UiArtwork.SetBackground(controlsPanel.GetComponent<Image>(), artwork.ControlsBackground);
            artwork.PrimaryButton.Apply(playButton);
            artwork.SecondaryButton.Apply(controlsButton);
            artwork.SecondaryButton.Apply(creditsButton);
            artwork.SecondaryButton.Apply(quitButton);
            artwork.PrimaryButton.Apply(backButton);
        }

        public void ReturnFromControls()
        {
            if (controlsPanel.activeSelf)
                ShowMain();
        }

        private void ShowMain()
        {
            controlsPanel.SetActive(false);
            mainPanel.SetActive(true);
            playButton.Select();
        }

        private void ShowControls()
        {
            mainPanel.SetActive(false);
            controlsPanel.SetActive(true);
            backButton.Select();
        }
    }
}
