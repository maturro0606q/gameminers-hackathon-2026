using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class UiPortalView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cover;
        [SerializeField] private Text messageLabel;

        public void Show(string message, float opacity)
        {
            cover.alpha = Mathf.Clamp01(opacity);
            messageLabel.text = string.IsNullOrWhiteSpace(message) ? "Entrando al mundo del Jety…" : message;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
