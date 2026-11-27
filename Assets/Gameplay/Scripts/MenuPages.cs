using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMiners.Gameplay
{
    internal sealed class MenuPages
    {
        private static readonly Color Ink = new Color32(40, 25, 55, 255);
        private static readonly Color Paper = new Color32(255, 236, 222, 255);
        private static readonly Color Pink = new Color32(247, 142, 177, 255);
        private static readonly Color Mint = new Color32(160, 230, 201, 255);

        private readonly Transform root;
        private readonly Font font = CookieUi.Hand;
        private readonly GameObject settings, credits, creditNotes, pause;
        private GameObject active;
        private Action back;
        private readonly Text fullscreenLabel, creditsBackLabel;
        private bool fullscreenRequested;
        private static Vector2Int windowSize = new Vector2Int(1280, 720);

        public MenuPages(Transform root, Func<float> volume, Action<float> setVolume, Action resume, Action mainMenu)
        {
            this.root = root;
            settings = Panel("Configuración", 610, 460, out var settingsCard);
            Label(settingsCard, "Volumen de la música", 29, 0, 57, 450, 42, Paper);
            var percent = Label(settingsCard, "", 23, 0, -53, 444, 32, Mint);
            var slider = VolumeSlider(settingsCard);
            slider.SetValueWithoutNotify(volume());
            percent.text = $"{Mathf.RoundToInt(slider.value * 100)} %";
            slider.onValueChanged.AddListener(value =>
            {
                percent.text = $"{Mathf.RoundToInt(value * 100)} %";
                setVolume(value);
            });
            fullscreenRequested = Screen.fullScreenMode != FullScreenMode.Windowed;
            fullscreenLabel = Button(settingsCard, "Pantalla completa", 0, -111, 431, 52, ToggleFullscreen);
            UpdateFullscreenLabel();
            Button(settingsCard, "Volver", 0, -177, 242, 52, Back);

            credits = Panel("Créditos", 790, 704, out var creditsCard);
            Label(creditsCard, "La heladería maldita", 27, 0, 223, 660, 39, Mint, FontStyle.Bold);
            Label(creditsCard, "Un juego de GameMiners · Hackatón 2026", 20, 0, 188, 660, 30, Paper);
            CreditRow(creditsCard, "Matías", "Dirección e integración / UX / Interfaces", 140, true);
            CreditRow(creditsCard, "Nacho", "Programación y diseño de gameplay / escenarios", 91, true);
            CreditRow(creditsCard, "Cami", "Dirección artística, personajes, modelos 3D y animaciones", 42, true);

            Label(creditsCard, "Música · OpenGameArt · CC0", 25, 0, -13, 660, 42, Mint, FontStyle.Bold);
            CreditRow(creditsCard, "Ice Cream Truck Theme", "Cleyton Kauffman", -60);
            CreditRow(creditsCard, "Magic Puzzle In-Game 1", "MintoDog", -101);
            CreditRow(creditsCard, "Boss Battle Music", "Juhani Junkala / SubspaceAudio", -142);
            CreditRow(creditsCard, "Spooky Waltz", "Aureolus_Omicron", -183);
            Button(creditsCard, "Herramientas y licencias", -155, -264, 350, 57, ShowCreditNotes);
            creditsBackLabel = Button(creditsCard, "Volver", 175, -264, 245, 57, Back);

            creditNotes = Panel("Herramientas y licencias", 790, 656, out var notes);
            Label(notes, "Unity, Blender y OpenAI Codex", 25, 0, 165, 644, 42, Paper, FontStyle.Bold);
            Label(notes, "Herramientas de desarrollo · Sin patrocinio ni afiliación oficial declarada.", 22, 0, 108, 644, 65, Paper);
            Label(notes, "Efectos de sonido sintetizados para el juego.", 22, 0, 46, 644, 40, Paper);
            Label(notes, "© 2026 GameMiners contributors · Código original bajo MIT.", 22, 0, -13, 644, 53, Paper);
            Label(notes, "© 2026 Cami · Arte original, todos los derechos reservados. Uso autorizado únicamente para este videojuego GameMiners.", 22, 0, -87, 644, 84, Paper);
            Button(notes, "Créditos en GitHub", -151, -222, 283, 57, () => Application.OpenURL(
                "https://github.com/maturro0606q/gameminers-hackathon-2026/blob/docs%2Flicencias-y-musica-v01/docs%2FCREDITS_IN_GAME.md"));
            Button(notes, "Volver a créditos", 152, -222, 262, 57, Back);

            pause = Panel("Pausa", 560, 456, out var pauseCard);
            Button(pauseCard, "Continuar", 0, 53, 350, 56, resume);
            Button(pauseCard, "Configuración", 0, -20, 350, 56, () => Open(settings, ShowPause));
            Button(pauseCard, "Menú principal", 0, -93, 350, 56, mainMenu);
            CookieUi.Apply(settings.transform);
            CookieUi.Apply(credits.transform);
            CookieUi.Apply(creditNotes.transform);
            CookieUi.Apply(pause.transform);
        }

        public bool IsOpen => active != null;
        public bool IsPause => active == pause;
        public void ShowPause() => Open(pause, null);
        public void ShowSettings(Action returnTo)
        {
            UpdateFullscreenLabel();
            Open(settings, returnTo);
        }

        public void ToggleFullscreen()
        {
            if (!fullscreenRequested && Screen.width > 0 && Screen.height > 0)
                windowSize = new Vector2Int(Screen.width, Screen.height);
            fullscreenRequested = !fullscreenRequested;
            if (fullscreenRequested)
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            else
                Screen.SetResolution(windowSize.x, windowSize.y, FullScreenMode.Windowed);
            UpdateFullscreenLabel();
        }

        private void UpdateFullscreenLabel() =>
            fullscreenLabel.text = (fullscreenRequested ? "Modo ventana" : "Pantalla completa") + "   ·   F11";
        public void ShowCredits(Action returnTo, string backText = "Volver")
        {
            creditsBackLabel.text = backText;
            Open(credits, returnTo);
        }

        private void ShowCreditNotes()
        {
            var returnTo = back;
            string label = creditsBackLabel.text;
            Open(creditNotes, () => ShowCredits(returnTo, label));
        }

        public void Close()
        {
            if (active != null) active.SetActive(false);
            active = null;
            back = null;
        }

        public void Back()
        {
            var returnTo = back;
            Close();
            returnTo?.Invoke();
        }

        private void Open(GameObject page, Action returnTo)
        {
            Close();
            active = page;
            back = returnTo;
            page.transform.SetAsLastSibling();
            page.SetActive(true);
            page.GetComponentInChildren<Selectable>().Select();
        }

        private GameObject Panel(string title, float width, float height, out RectTransform card)
        {
            var panel = new GameObject(title, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root, false);
            var rect = (RectTransform)panel.transform;
            Stretch(rect);
            panel.GetComponent<Image>().color = new Color(0.07f, 0.04f, 0.12f, 0.84f);

            card = Center(rect, "Tarjeta", 0, 0, width, height);
            Image(card, Ink);
            Image(Center(card, "Borde superior", 0, height * 0.5f - 3, width, 6), Pink);
            Label(card, title, 49, 0, height * 0.5f - 68, width - 50, 65, Paper, FontStyle.Bold);
            panel.SetActive(false);
            return panel;
        }

        private Slider VolumeSlider(Transform card)
        {
            var rect = Center(card, "Volumen", 0, 2, 410, 42);
            var slider = rect.gameObject.AddComponent<Slider>();
            Image(rect, new Color32(79, 56, 98, 255));
            var fillArea = Center(rect, "Rango de volumen", 0, 0, 386, 12);
            var fill = Center(fillArea, "Indicador", 0, 0, 0, 0);
            Stretch(fill);
            Image(fill, Mint);
            var handleArea = Center(rect, "Rango del control", 0, 0, 386, 28);
            var handle = Center(handleArea, "Control", 0, 0, 24, 28);
            var thumb = Image(handle, Paper);
            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = thumb;
            return slider;
        }

        private void CreditRow(Transform card, string name, string detail, float y, bool team = false)
        {
            var left = Label(card, name, 24, team ? -233 : -165, y, team ? 170 : 306, 39, Paper, FontStyle.Bold);
            var right = Label(card, detail, 22, team ? 76 : 162, y, team ? 448 : 333, team ? 45 : 39, Paper);
            left.alignment = TextAnchor.MiddleLeft;
            right.alignment = TextAnchor.MiddleLeft;
            left.horizontalOverflow = right.horizontalOverflow = HorizontalWrapMode.Wrap;
        }

        private Text Button(Transform parent, string title, float x, float y, float width, float height, Action click)
        {
            var rect = Center(parent, title, x, y, width, height);
            var image = Image(rect, new Color32(108, 63, 111, 255));
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.highlightedColor = new Color32(255, 190, 213, 255);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            button.onClick.AddListener(() => click());
            return Label(rect, title, 25, 0, 0, width - 34, height - 12, Paper, FontStyle.Bold);
        }

        private Text Label(Transform parent, string value, int size, float x, float y, float width, float height,
            Color color, FontStyle style = FontStyle.Normal)
        {
            var label = Center(parent, value, x, y, width, height).gameObject.AddComponent<Text>();
            label.font = font;
            label.fontSize = size;
            label.fontStyle = style;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = color;
            label.raycastTarget = false;
            label.text = value;
            return label;
        }

        private static RectTransform Center(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static Image Image(RectTransform rect, Color color)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
