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
        [SerializeField] private Text equipmentLabel;

        public void ShowShop(string objective)
        {
            ShowShop(objective, null);
        }

        public void ShowShop(string objective, int? stockRemaining)
        {
            SetObjective(objective, stockRemaining.HasValue ? $"Stock  {Mathf.Max(0, stockRemaining.Value)}" : null);
            healthPanel.SetActive(false);
            SetEquipment(null, null);
            reticle.SetActive(true);
            gameObject.SetActive(true);
        }

        public void ShowCombat(string objective, int health, int maxHealth, int current, int required)
        {
            ShowCombat(objective, health, maxHealth, current, required, null, null);
        }

        public void ShowCombat(string objective, int health, int maxHealth, int current, int required,
            string weapon, int? bombs)
        {
            SetObjective(objective, required > 0 ? $"{Mathf.Clamp(current, 0, required)} / {required}" : null);
            reticle.SetActive(false);
            int displayedHealth = Mathf.Clamp(health, 0, Mathf.Max(0, maxHealth));
            healthPanel.SetActive(maxHealth > 0);
            healthFill.anchorMax = new Vector2(maxHealth > 0 ? (float)displayedHealth / maxHealth : 0f, 1f);
            healthLabel.text = $"Vida  {displayedHealth} / {maxHealth}";
            SetEquipment(weapon, bombs);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetObjective(string objective, string progress)
        {
            bool hasObjective = !string.IsNullOrWhiteSpace(objective);
            bool hasProgress = !string.IsNullOrWhiteSpace(progress);
            objectiveLabel.text = objective ?? string.Empty;
            objectiveLabel.gameObject.SetActive(hasObjective);
            progressLabel.text = progress ?? string.Empty;
            progressLabel.gameObject.SetActive(hasProgress);
            objectivePanel.SetActive(hasObjective || hasProgress);
        }

        private void SetEquipment(string weapon, int? bombs)
        {
            if (equipmentLabel == null)
                return;

            string label = string.IsNullOrWhiteSpace(weapon) ? string.Empty : $"Arma  {weapon}";
            if (bombs.HasValue)
            {
                string bombsText = $"Bombas  {Mathf.Max(0, bombs.Value)}";
                label = label.Length == 0 ? bombsText : $"{label}  ·  {bombsText}";
            }

            equipmentLabel.text = label;
            equipmentLabel.gameObject.SetActive(label.Length != 0);
        }
    }
}
