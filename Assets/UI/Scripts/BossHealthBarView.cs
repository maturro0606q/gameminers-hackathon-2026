using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    public sealed class BossHealthBarView : MonoBehaviour
    {
        [SerializeField] private Text bossLabel;
        [SerializeField] private Text healthLabel;
        [SerializeField] private RectTransform healthFill;

        public void Show(string bossName, int health, int maxHealth)
        {
            if (string.IsNullOrWhiteSpace(bossName) || maxHealth <= 0)
            {
                Hide();
                return;
            }

            int displayedHealth = Mathf.Clamp(health, 0, maxHealth);
            bossLabel.text = bossName;
            healthLabel.text = $"{displayedHealth} / {maxHealth}";
            healthFill.anchorMax = new Vector2((float)displayedHealth / maxHealth, 1f);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
