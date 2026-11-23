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

        public Button PlayButton => playButton;

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
