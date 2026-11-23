using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameMiners.Gameplay
{
    public sealed partial class GameFlow : MonoBehaviour
    {
        private enum Chapter { Menu, Shop, IceWorld, JetyRoom }
        private const int MaxHealth = 100;
        private static readonly string[] Scenes = { "00_Menu", "01_Shop", "02_IceWorld", "03_JetyRoom" };
        private static readonly Color Mint = new Color32(100, 220, 171, 255);
        private static readonly Color Rose = new Color32(241, 100, 147, 255);
        private static readonly Color Grape = new Color32(167, 115, 245, 255);
        private static readonly Color Ice = new Color32(158, 222, 255, 255);
        private static readonly string[] Flavors = { "fresa", "vainilla", "menta" };
        private static readonly int[] Orders = { 1, 0, 2 };

        [SerializeField] private Chapter chapter;
        [SerializeField] private InputActionAsset input;
        [SerializeField] private Material blockoutSurface;
        [SerializeField] private GameObject[] uiPrefabs;
        [SerializeField] private AudioClip sceneMusic;
        [Header("Modelos de Cami")]
        [SerializeField] private GameObject protagonistModel;
        [SerializeField] private GameObject seatedProtagonistModel;
        [SerializeField] private GameObject smallIceCreamModel;
        [SerializeField] private GameObject largeIceCreamModel;
        [SerializeField] private GameObject iceCreamBossModel;
        [SerializeField] private GameObject jetyModel;
        [SerializeField] private GameObject scoopModel;

#if !GAMEMINERS_PORTABLE
        [SerializeField]
#endif
        private GameObject truckModel, rabbitModel;
#if !GAMEMINERS_PORTABLE
        [SerializeField]
#endif
        private RuntimeAnimatorController heroMotion, iceMotion;

        private CharacterMotion hero;
        private Transform guide;
        private static float runSeconds;
        private static bool arrivingFromShop;
        private static int carriedHealth = MaxHealth, carriedBombs;
        private static Weapon carriedWeapon = Weapon.Scoop;

        private BlockoutWorld world;
        private GamePresentation ui;
        private GameAudio audio;
        private InputActionMap playerInput;
        private InputAction move, look, interact, attack;
        private CharacterController player;
        private Camera view;
        private ShopFirstPersonController shopController;
        private WorldThirdPersonController combatController;
        private Transform[] customers;
        private Transform fridge, apparition, portal, scoop, spear, playerVisual;
        private int served, health = MaxHealth;
        private bool inputLocked, loading, paused, finished;

        private void Start()
        {
            RenderSettings.fog = false;
            world = new BlockoutWorld(transform, blockoutSurface);
            audio = new GameAudio(this, (int)chapter, sceneMusic);
            ui = new GamePresentation(transform, uiPrefabs, BeginRun, Retry, () => Load(Chapter.Menu),
                Resume, () => audio.Volume, audio.SetVolume);
            SetCursor(chapter != Chapter.Menu);
            if (chapter == Chapter.Menu)
            {
                view = world.CameraAt(new Color32(58, 40, 89, 255));
                world.TruckBackdrop(view, truckModel);
                ui.ShowMenu();
                return;
            }

            playerInput = input.FindActionMap("Player", true);
            move = playerInput.FindAction("Move", true);
            look = playerInput.FindAction("Look", true);
            interact = playerInput.FindAction("Interact", true);
            attack = playerInput.FindAction("Attack", true);
            playerInput.Enable();

            if (chapter == Chapter.Shop)
            {
                world.Truck(jetyModel, out customers, out fridge, out apparition);
                view = world.CameraAt(new Color32(177, 216, 232, 255));
                player = world.Player(new Vector3(0.43f, 0, -0.69f));
                player.transform.rotation = Quaternion.Euler(0, 90, 0);
                shopController = new ShopFirstPersonController(player, view);
                BeginService();
                return;
            }

            bool finalRoom = chapter == Chapter.JetyRoom;
            if (finalRoom) world.Arena(true, out portal);
            else trailGates = world.DressTrail(out portal);
            view = world.CameraAt(finalRoom ? new Color32(39, 23, 60, 255) : new Color32(188, 231, 241, 255));
            player = world.Player(finalRoom ? new Vector3(-3.8f, 0, -2.6f) : IceTrail.At(IceTrail.Areas[trailCheckpoint].Entrance));
            player.transform.rotation = Quaternion.Euler(0, 67f, 0);
            playerVisual = protagonistModel != null
                ? BlockoutWorld.MountModel(protagonistModel, player.transform)
                : PortableModel.Instantiate("Hero", player.transform, blockoutSurface);
            if (playerVisual != null)
            {
                playerVisual.localRotation = Quaternion.Euler(0, 180, 0);
                BlockoutWorld.NormalizeModel(playerVisual, 1.76f);
                hero = new CharacterMotion(playerVisual, heroMotion);
            }
            else playerVisual = world.Person(player.transform, Rose);
            Transform grip = CharacterMotion.Find(playerVisual, "Grip.R");
            scoop = world.Scoop(grip != null ? grip : player.transform, scoopModel);
            spear = world.Spear(grip != null ? grip : player.transform, grip != null ? Vector3.zero : new Vector3(0.47f, 1f, 0.12f));
            if (grip != null)
            {
                scoop.localPosition = Vector3.zero;
                Transform tip = CharacterMotion.Find(playerVisual, "GripTip.R");
                if (tip != null && (tip.position - grip.position).sqrMagnitude > 0.001f)
                {
                    Vector3 forward = grip.InverseTransformDirection(tip.position - grip.position);
                    scoop.localRotation = spear.localRotation = Quaternion.LookRotation(forward, Vector3.forward);
                }
            }
            combatController = new WorldThirdPersonController(player, view, new Vector2(8.9f, 6.8f), finalRoom ? null : TrailMovement);
            if (!finalRoom)
            {
                guide = world.RabbitGuide(rabbitModel, IceTrail.At(-1.45f, 0.88f));
                world.RabbitAudience();
            }
            if (!finalRoom)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogColor = new Color32(199, 225, 237, 255);
                RenderSettings.fogStartDistance = 18f;
                RenderSettings.fogEndDistance = 78f;
            }
            StartCombat(finalRoom);
            if (!finalRoom && arrivingFromShop)
            {
                arrivingFromShop = false;
                StartCoroutine(Arrival());
            }
            else if (!finalRoom && trailCheckpoint == 0) StartCoroutine(GuideArrival());
        }

        private void BeginRun()
        {
            runSeconds = 0f;
            trailCheckpoint = bankedSugar = 0;
            arrivingFromShop = false;
            carriedHealth = MaxHealth;
            carriedBombs = 0;
            carriedWeapon = Weapon.Scoop;
            Load(Chapter.Shop);
        }

        private void Retry()
        {
            if (chapter == Chapter.Shop) BeginRun();
            else Load(chapter);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard?.f11Key.wasPressedThisFrame == true) ui.ToggleFullscreen();
            bool escape = keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
            if (chapter == Chapter.Menu)
            {
                if (escape) ui.MenuEscape();
                return;
            }
            if (loading) return;
            if (finished)
            {
                if (escape) ui.EndingEscape();
                return;
            }
            if (!paused) runSeconds += Time.deltaTime;
            if (inputLocked) return;

            if (escape)
            {
                if (paused) ui.PauseEscape(Resume);
                else Pause();
                return;
            }
            if (paused) return;

            var direction = move.ReadValue<Vector2>();
            if (chapter == Chapter.Shop)
            {
                shopController.Tick(direction, look.ReadValue<Vector2>(), Time.deltaTime);
                TickShop();
            }
            else
            {
                bool dash = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
                if (Time.time >= impactPauseUntil)
                {
                    combatController.Tick(direction, dash, Time.deltaTime, RunningRate);
                    if (combatController.JustDashed) BeginDash();
                }
                TickCombat(keyboard != null && keyboard.qKey.wasPressedThisFrame);
            }
        }

        private void LateUpdate()
        {
            if (!inputLocked && !loading && !paused && combatController != null)
            {
                combatController.Follow(CameraOpponent(), Time.deltaTime);
                UpdateAimMark();
            }
        }

        private void Pause()
        {
            paused = true;
            SetCursor(false);
            ui.HideGameplay();
            ui.ShowPause();
        }

        private void Resume()
        {
            paused = false;
            SetCursor(true);
            ui.ClosePage();
            if (chapter == Chapter.Shop) ResumeShop();
            else RefreshCombatHud();
        }

        private bool FacingShopTarget(Vector3 point, Transform target, float range, float minDot = 0.82f)
        {
            var offset = point - view.transform.position;
            float distance = offset.magnitude;
            if (distance > range || distance < 0.01f || Vector3.Dot(view.transform.forward, offset / distance) < minDot)
                return false;

            return !Physics.Raycast(view.transform.position, offset / distance, out var hit, distance, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore) || hit.transform.IsChildOf(target);
        }

        private void Load(Chapter next)
        {
            if (loading) return;
            loading = true;
            SceneManager.LoadScene(Scenes[(int)next]);
        }

        private void SetCursor(bool playing)
        {
            bool looking = playing && chapter == Chapter.Shop;
            Cursor.lockState = looking ? CursorLockMode.Locked : playing ? CursorLockMode.Confined : CursorLockMode.None;
            Cursor.visible = !looking;
        }

        private void OnDestroy()
        {
            playerInput?.Disable();
            audio?.Dispose();
            world?.Dispose();
        }
    }
}
