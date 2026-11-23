using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class PlayerHudView : MonoBehaviour
    {
        [SerializeField] private GameObject reticle;
        [SerializeField] private GameObject objectivePanel;
        [SerializeField] private Text objectiveLabel;
        [SerializeField] private Text progressLabel;
        [SerializeField] private GameObject healthPanel;
        [SerializeField] private Text healthLabel;
        [SerializeField] private RectTransform healthFill;

        public void ShowShop(string objective)
        {
            SetObjective(objective, 0, 0);
            healthPanel.SetActive(false);
            reticle.SetActive(true);
            gameObject.SetActive(true);
        }

        public void ShowCombat(string objective, int health, int maxHealth, int current, int required)
        {
            SetObjective(objective, current, required);
            reticle.SetActive(false);
            int displayedHealth = Mathf.Clamp(health, 0, Mathf.Max(0, maxHealth));
            healthPanel.SetActive(maxHealth > 0);
            healthFill.anchorMax = new Vector2(maxHealth > 0 ? (float)displayedHealth / maxHealth : 0f, 1f);
            healthLabel.text = $"Vida  {displayedHealth} / {maxHealth}";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetObjective(string objective, int current, int required)
        {
            bool hasObjective = !string.IsNullOrWhiteSpace(objective);
            bool hasProgress = required > 0;
            objectiveLabel.text = objective ?? string.Empty;
            objectiveLabel.gameObject.SetActive(hasObjective);
            progressLabel.text = hasProgress ? $"{Mathf.Clamp(current, 0, required)} / {required}" : string.Empty;
            progressLabel.gameObject.SetActive(hasProgress);
            objectivePanel.SetActive(hasObjective || hasProgress);
        }
    }
}
