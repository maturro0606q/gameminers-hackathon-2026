using System;
using UnityEngine;

namespace GameMiners.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class GameUiFlowView : MonoBehaviour
    {
        [SerializeField] private MainMenuView menuPrefab;
        [SerializeField] private PlayerHudView hudPrefab;
        [SerializeField] private InteractionPromptView interactionPrefab;
        [SerializeField] private BossHealthBarView bossPrefab;
        [SerializeField] private ResultPanelView defeatPrefab;
        [SerializeField] private ResultPanelView endingPrefab;
        [SerializeField] private UiOverlayView pausePrefab;
        [SerializeField] private UiOverlayView creditsPrefab;
        [SerializeField] private UiPortalView portalPrefab;
        [SerializeField] private UiArtwork artwork;

        private MainMenuView menu;
        private PlayerHudView hud;
        private InteractionPromptView interaction;
        private BossHealthBarView boss;
        private ResultPanelView defeat;
        private ResultPanelView ending;
        private UiOverlayView pause;
        private UiOverlayView credits;
        private UiPortalView portal;
        private GameUiScreen? screen;
        private bool creditsOpen;

        public event Action PlayRequested;
        public event Action PauseRequested;
        public event Action ResumeRequested;
        public event Action RestartLevelRequested;
        public event Action RetryRequested;
        public event Action MenuRequested;

        private void Awake()
        {
            InitializeViews();
        }

        private void Start()
        {
            if (!screen.HasValue)
                Render(new GameUiSnapshot());
        }

        public void Render(GameUiSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            InitializeViews();
            if (screen != snapshot.Screen)
            {
                HideViews();
                screen = snapshot.Screen;
                ShowScreen();
            }

            switch (snapshot.Screen)
            {
                case GameUiScreen.Shop:
                    hud.ShowShop(snapshot.Objective, snapshot.StockRemaining);
                    interaction.Show(snapshot.InteractionBinding, snapshot.InteractionAction);
                    break;
                case GameUiScreen.Combat:
                    hud.ShowCombat(snapshot.Objective, snapshot.Health, snapshot.MaxHealth,
                        snapshot.DefeatedCount, snapshot.DefeatedTarget, snapshot.EquippedWeapon, snapshot.BombCount);
                    interaction.Show(snapshot.InteractionBinding, snapshot.InteractionAction);
                    boss.Show(snapshot.BossName, snapshot.BossHealth, snapshot.BossMaxHealth);
                    break;
                case GameUiScreen.Portal:
                    portal.Show(snapshot.PortalMessage, snapshot.PortalOpacity);
                    break;
            }
        }

        public void RequestBack()
        {
            if (creditsOpen)
            {
                CloseCredits();
                return;
            }

            switch (screen)
            {
                case GameUiScreen.Menu:
                    menu.ReturnFromControls();
                    break;
                case GameUiScreen.Shop:
                case GameUiScreen.Combat:
                    PauseRequested?.Invoke();
                    break;
                case GameUiScreen.Pause:
                    ResumeRequested?.Invoke();
                    break;
            }
        }

        private void InitializeViews()
        {
            if (menu != null)
                return;

            menu = Instantiate(menuPrefab, transform);
            hud = Instantiate(hudPrefab, transform);
            interaction = Instantiate(interactionPrefab, transform);
            boss = Instantiate(bossPrefab, transform);
            defeat = Instantiate(defeatPrefab, transform);
            ending = Instantiate(endingPrefab, transform);
            pause = Instantiate(pausePrefab, transform);
            portal = Instantiate(portalPrefab, transform);
            credits = Instantiate(creditsPrefab, transform);

            HideViews();
            menu.PlayButton.onClick.AddListener(() => PlayRequested?.Invoke());
            menu.CreditsButton.onClick.AddListener(OpenCredits);
            ending.CreditsButton.onClick.AddListener(OpenCredits);
            credits.PrimaryButton.onClick.AddListener(CloseCredits);
            pause.PrimaryButton.onClick.AddListener(() => ResumeRequested?.Invoke());
            pause.RestartButton.onClick.AddListener(() => RestartLevelRequested?.Invoke());
            pause.MenuButton.onClick.AddListener(() => MenuRequested?.Invoke());
            defeat.RetryButton.onClick.AddListener(() => RetryRequested?.Invoke());
            defeat.MenuButton.onClick.AddListener(() => MenuRequested?.Invoke());
            ending.MenuButton.onClick.AddListener(() => MenuRequested?.Invoke());

            if (artwork != null)
            {
                menu.ApplyArtwork(artwork);
                defeat.ApplyArtwork(artwork, artwork.DefeatBackground);
                ending.ApplyArtwork(artwork, artwork.EndingBackground);
                pause.ApplyArtwork(artwork, artwork.PauseBackground);
                credits.ApplyArtwork(artwork, artwork.CreditsBackground);
            }
        }

        private void HideViews()
        {
            creditsOpen = false;
            menu.gameObject.SetActive(false);
            hud.Hide();
            interaction.Hide();
            boss.Hide();
            defeat.Hide();
            ending.Hide();
            pause.Hide();
            portal.Hide();
            credits.Hide();
        }

        private void ShowScreen()
        {
            switch (screen)
            {
                case GameUiScreen.Menu:
                    menu.gameObject.SetActive(true);
                    menu.PlayButton.Select();
                    break;
                case GameUiScreen.Pause:
                    pause.Show();
                    break;
                case GameUiScreen.Defeat:
                    defeat.Show();
                    break;
                case GameUiScreen.Ending:
                    ending.Show();
                    break;
            }
        }

        private void OpenCredits()
        {
            if (screen != GameUiScreen.Menu && screen != GameUiScreen.Ending)
                return;

            creditsOpen = true;
            menu.gameObject.SetActive(false);
            ending.Hide();
            credits.Show();
        }

        private void CloseCredits()
        {
            if (!creditsOpen)
                return;

            creditsOpen = false;
            credits.Hide();
            ShowScreen();
            if (screen == GameUiScreen.Menu)
                menu.CreditsButton.Select();
            else if (screen == GameUiScreen.Ending)
                ending.CreditsButton.Select();
        }
    }
}
