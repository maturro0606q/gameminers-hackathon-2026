using System;
using GameMiners.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace GameMiners.Gameplay
{
    internal sealed class GamePresentation
    {
        private readonly MainMenuView menu;
        private readonly PlayerHudView hud;
        private readonly InteractionPromptView prompt;
        private readonly BossHealthBarView boss;
        private readonly ResultPanelView defeat;
        private readonly ResultPanelView ending;
        private readonly MenuPages pages;
        private readonly Image shade;
        private readonly RectTransform topBar, bottomBar;
        private readonly Text equipment, elapsed, dialogue, score, routeNotice, boost;
        private readonly Action showCredits;
        private float noticeUntil;
        private string hint;

        public GamePresentation(Transform parent, GameObject[] prefabs, Action play, Action retry, Action returnToMenu,
            Action resume, Func<float> volume, Action<float> setVolume)
        {
            var canvasObject = new GameObject("GameMiners Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.transform.SetParent(parent, false);

            menu = Spawn<MainMenuView>(prefabs[0], canvas.transform);
            hud = Spawn<PlayerHudView>(prefabs[1], canvas.transform);
            prompt = Spawn<InteractionPromptView>(prefabs[2], canvas.transform);
            boss = Spawn<BossHealthBarView>(prefabs[3], canvas.transform);
            defeat = Spawn<ResultPanelView>(prefabs[4], canvas.transform);
            ending = Spawn<ResultPanelView>(prefabs[5], canvas.transform);
            menu.PlayButton.onClick.AddListener(() => play());
            defeat.RetryButton.onClick.AddListener(() => retry());
            defeat.MenuButton.onClick.AddListener(() => returnToMenu());
            ending.MenuButton.onClick.AddListener(() => returnToMenu());

            pages = new MenuPages(canvas.transform, volume, setVolume, resume, returnToMenu);
            ExtendMenu();
            var endingCard = (RectTransform)ending.transform.Find("Card");
            endingCard.sizeDelta = new Vector2(660, 374);
            var endButton = (RectTransform)ending.MenuButton.transform;
            endButton.anchoredPosition = new Vector2(0, -114);
            showCredits = () =>
            {
                ending.Hide();
                pages.ShowCredits(ending.Show, "Continuar");
            };
            MenuLink(ending.MenuButton, "Créditos", -46, showCredits);
            var promptCard = (RectTransform)prompt.transform.Find("Prompt");
            promptCard.anchoredPosition = new Vector2(0, 56);

            topBar = Overlay(canvas.transform, "Barra de cine superior", 1f, 1f).rectTransform;
            bottomBar = Overlay(canvas.transform, "Barra de cine inferior", 0f, 0f).rectTransform;
            topBar.pivot = new Vector2(0.5f, 1f);
            bottomBar.pivot = new Vector2(0.5f, 0f);
            Letterbox(0f);

            equipment = SmallLabel(hud.transform, "Equipo", new Vector2(1, 0), new Vector2(-22, 23), TextAnchor.MiddleRight);
            equipment.transform.parent.gameObject.SetActive(false);
            score = SmallLabel(hud.transform, "Azúcar y racha", new Vector2(0, 0), new Vector2(22, 105), TextAnchor.MiddleLeft);
            ((RectTransform)score.transform.parent).sizeDelta = new Vector2(278, 44);
            score.transform.parent.gameObject.SetActive(false);
            routeNotice = SmallLabel(canvas.transform, "Paso abierto", new Vector2(0.5f, 0), new Vector2(0, 101), TextAnchor.MiddleCenter);
            ((RectTransform)routeNotice.transform.parent).sizeDelta = new Vector2(475, 48);
            routeNotice.transform.parent.gameObject.SetActive(false);
            elapsed = SmallLabel(ending.transform, "Duración", new Vector2(0.5f, 0), new Vector2(0, 56), TextAnchor.MiddleCenter);

            dialogue = SmallLabel(canvas.transform, "Diálogo", new Vector2(0.5f, 0), new Vector2(0, 101), TextAnchor.MiddleCenter);
            dialogue.rectTransform.offsetMin = new Vector2(54, 20);
            dialogue.rectTransform.offsetMax = new Vector2(-54, -20);
            dialogue.horizontalOverflow = HorizontalWrapMode.Wrap;
            boost = SmallLabel(hud.transform, "Completo italiano", new Vector2(0, 0), new Vector2(22, 157), TextAnchor.MiddleLeft);
            ((RectTransform)boost.transform.parent).sizeDelta = new Vector2(278, 53);
            Boost(0);
            foreach (var part in new[] { menu.transform, hud.transform, prompt.transform, boss.transform, defeat.transform, ending.transform })
                CookieUi.Apply(part);
            Dialogue(null);
            shade = Overlay(canvas.transform, "Fundido", 0f, 1f);
            Fade(0f);
        }

        public void Dialogue(string line, bool cinematic = false)
        {
            var card = (RectTransform)dialogue.transform.parent;
            bool visible = !string.IsNullOrEmpty(line);
            dialogue.text = line;
            card.gameObject.SetActive(visible);
            if (visible)
            {
                float width = cinematic ? 1100f : 960f;
                dialogue.fontSize = cinematic ? 36 : 29;
                var layout = dialogue.GetGenerationSettings(new Vector2(width - 108f, 0f));
                layout.verticalOverflow = VerticalWrapMode.Overflow;
                float textHeight = dialogue.cachedTextGeneratorForLayout.GetPreferredHeight(line, layout) / dialogue.pixelsPerUnit;
                card.anchoredPosition = new Vector2(0, cinematic ? 78f : 101f);
                card.sizeDelta = new Vector2(width, Mathf.Max(96f, Mathf.Ceil(textHeight) + 45f));
            }
            SyncNotice();
        }

        public void Boost(float seconds)
        {
            boost.transform.parent.gameObject.SetActive(seconds > 0f);
            if (seconds > 0f) boost.text = $"COMPLETO · {Mathf.CeilToInt(seconds)} s";
        }

        public void ShowMenu() => menu.gameObject.SetActive(true);
        public void ShowShop(string objective) => hud.ShowShop(objective);
        public void ShowCombat(string objective, int health, int maxHealth, int current, int required) =>
            hud.ShowCombat(objective, health, maxHealth, current, required);

        public void Score(int sugar, int chain)
        {
            score.transform.parent.gameObject.SetActive(true);
            score.text = chain >= 2 ? $"✦ {sugar}   ·   {chain} GOLPES" : $"✦ {sugar} AZÚCAR";
            SyncNotice();
        }

        public void TrailNotice(string label)
        {
            routeNotice.text = label;
            noticeUntil = Time.time + 2.4f;
            SyncNotice();
        }

        public void SetEquipment(string weapon, int bombs)
        {
            equipment.transform.parent.gameObject.SetActive(true);
            equipment.text = $"{weapon}   ·   Bombas (Q) {bombs}";
        }

        private void SyncNotice() => routeNotice.transform.parent.gameObject
            .SetActive(Time.time < noticeUntil && !dialogue.transform.parent.gameObject.activeSelf);

        public void ToggleFullscreen() => pages.ToggleFullscreen();

        public void ShowPause() => pages.ShowPause();
        public void ClosePage() => pages.Close();

        public void PauseEscape(Action resume)
        {
            if (pages.IsPause) resume();
            else pages.Back();
        }

        public void MenuEscape()
        {
            if (pages.IsOpen) pages.Back();
            else if (!menu.PlayButton.gameObject.activeInHierarchy)
                menu.transform.Find("ControlsPanel/ControlsCard/BackButton").GetComponent<Button>().onClick.Invoke();
        }

        public void Hint(string action)
        {
            if (hint == action) return;
            hint = action;
            if (string.IsNullOrEmpty(action)) prompt.Hide();
            else prompt.Show("E", action);
        }

        public void ShowBoss(string name, int life, int maxLife) => boss.Show(name, life, maxLife);
        public void HideBoss() => boss.Hide();

        public void HideGameplay()
        {
            hud.Hide();
            Hint(null);
            boss.Hide();
            noticeUntil = 0f;
            routeNotice.transform.parent.gameObject.SetActive(false);
        }

        public void ShowDefeat() => defeat.Show();

        public void ShowEnding(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            elapsed.text = $"Duración de la partida   {total / 60}:{total % 60:00}";
            showCredits();
        }

        public void EndingEscape()
        {
            if (pages.IsOpen) pages.Back();
        }

        public void Fade(float opacity, bool icy = false)
        {
            shade.color = icy
                ? new Color(0.77f, 0.92f, 1f, opacity)
                : new Color(0.07f, 0.035f, 0.11f, opacity);
            shade.raycastTarget = opacity > 0f;
            shade.enabled = opacity > 0f;
        }

        public void Letterbox(float weight)
        {
            topBar.sizeDelta = bottomBar.sizeDelta = new Vector2(0, 65f * weight);
            topBar.gameObject.SetActive(weight > 0f);
            bottomBar.gameObject.SetActive(weight > 0f);
        }

        private void ExtendMenu()
        {
            var card = menu.transform.Find("MainPanel/MenuCard");
            var cardRect = (RectTransform)card;
            cardRect.sizeDelta = new Vector2(610, 656);
            ((RectTransform)card.Find("Title")).anchoredPosition = new Vector2(0, 207);
            

            var play = (RectTransform)menu.PlayButton.transform;
            play.anchoredPosition = new Vector2(0, 79);
            var controls = card.Find("ControlsButton").GetComponent<Button>();
            ((RectTransform)controls.transform).anchoredPosition = new Vector2(0, -1);
            var quit = (RectTransform)card.Find("QuitButton");
            quit.anchoredPosition = new Vector2(0, -218);

            MenuLink(controls, "Configuración", -74, () =>
            {
                menu.gameObject.SetActive(false);
                pages.ShowSettings(ShowMenu);
            });
#if GAMEMINERS_PORTABLE
            Button credits = null;
#else
            var credits = menu.CreditsButton;
#endif
            Action openCredits = () =>
            {
                menu.gameObject.SetActive(false);
                pages.ShowCredits(ShowMenu);
            };
            if (credits != null)
            {
                ((RectTransform)credits.transform).anchoredPosition = new Vector2(0, -147);
                credits.onClick.AddListener(() => openCredits());
            }
            else MenuLink(controls, "Créditos", -147, openCredits);

            var controlsCard = menu.transform.Find("ControlsPanel/ControlsCard");
            var shopControls = controlsCard.Find("ShopCard");
            var combatControls = controlsCard.Find("CombatCard");
            combatControls.Find("Heading").GetComponent<Text>().text = "Mundo del Yeti";
            FitControls(shopControls, "WASD · Moverse\nRatón · Mirar\nE · Interactuar\nEsc · Pausa", 23);
            FitControls(combatControls, "WASD · Moverse\nClic izq. / der. · Atacar / fuerte\nEspacio · Esquivar\nE · Interactuar\nQ · Bomba  /  Esc · Pausa", 20);
            var title = card.Find("Title").GetComponent<Text>();
            title.text = "La Heladería Maldita";
            title.fontSize = 64;
        }

        private static void FitControls(Transform panel, string actions, int size)
        {
            ((RectTransform)panel).sizeDelta = new Vector2(330, 272);
            var bindings = panel.Find("Bindings").GetComponent<Text>();
            var box = bindings.rectTransform;
            box.pivot = new Vector2(0.5f, 1f);
            box.anchoredPosition = new Vector2(0f, -69f);
            box.sizeDelta = new Vector2(286f, 182f);
            bindings.text = actions;
            bindings.fontSize = size;
            bindings.resizeTextForBestFit = true;
            bindings.resizeTextMinSize = 16;
            bindings.resizeTextMaxSize = size;
            bindings.alignment = TextAnchor.UpperLeft;
        }

        private static void MenuLink(Button template, string label, float y, Action click)
        {
            var item = UnityEngine.Object.Instantiate(template, template.transform.parent, false);
            item.name = label;
            ((RectTransform)item.transform).anchoredPosition = new Vector2(0, y);
            item.GetComponentInChildren<Text>(true).text = label;
            item.onClick = new Button.ButtonClickedEvent();
            item.onClick.AddListener(() => click());
        }

        private static Image Overlay(Transform root, string title, float yMin, float yMax)
        {
            var layer = new GameObject(title, typeof(RectTransform), typeof(Image));
            layer.transform.SetParent(root, false);
            var rect = (RectTransform)layer.transform;
            rect.anchorMin = new Vector2(0f, yMin);
            rect.anchorMax = new Vector2(1f, yMax);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var image = layer.GetComponent<Image>();
            image.color = new Color32(13, 8, 24, 255);
            image.raycastTarget = false;
            return image;
        }

        private static Text SmallLabel(Transform parent, string name, Vector2 anchor, Vector2 position, TextAnchor alignment)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(Image));
            item.transform.SetParent(parent, false);
            var rect = (RectTransform)item.transform;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(396, 44);
            var backdrop = item.GetComponent<Image>();
            CookieUi.Frame(backdrop, name != "Diálogo");
            backdrop.raycastTarget = false;

            var label = new GameObject("Texto", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            label.transform.SetParent(item.transform, false);
            var textRect = label.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12, 0);
            textRect.offsetMax = new Vector2(-12, 0);
            label.font = CookieUi.Hand;
            label.color = CookieUi.Ink;
            label.fontSize = 22;
            label.alignment = alignment;
            label.raycastTarget = false;
            return label;
        }

        private static T Spawn<T>(GameObject prefab, Transform parent) where T : MonoBehaviour
        {
            var instance = UnityEngine.Object.Instantiate(prefab, parent, false);
            var fallbackFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            foreach (var label in instance.GetComponentsInChildren<Text>(true))
            {
                if (label.font == null || label.font.name == "Arial")
                    label.font = fallbackFont;
            }
            instance.SetActive(false);
            return instance.GetComponent<T>();
        }
    }
}
