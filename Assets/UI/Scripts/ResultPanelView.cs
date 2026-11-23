using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class ResultPanelView : MonoBehaviour
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button retryButton;

        public Button MenuButton => menuButton;
        public Button RetryButton => retryButton;

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
