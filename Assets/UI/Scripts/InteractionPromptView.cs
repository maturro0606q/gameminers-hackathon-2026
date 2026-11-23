using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private GameObject keyBadge;
        [SerializeField] private Text keyLabel;
        [SerializeField] private Text actionLabel;

        public void Show(string binding, string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                Hide();
                return;
            }

            keyLabel.text = binding ?? string.Empty;
            keyBadge.SetActive(!string.IsNullOrWhiteSpace(binding));
            actionLabel.text = action;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
