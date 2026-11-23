using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld : IDisposable
    {
        private static readonly Color Cream = new Color32(255, 219, 176, 255);
        private static readonly Color Rose = new Color32(232, 106, 139, 255);
        private static readonly Color Mint = new Color32(102, 209, 176, 255);
        private static readonly Color Plum = new Color32(92, 47, 103, 255);
        private static readonly Color Ice = new Color32(134, 205, 241, 255);
        private static readonly Color Dark = new Color32(36, 24, 55, 255);
        private readonly Dictionary<Color, Material> materials = new Dictionary<Color, Material>();
        private readonly Transform root;
        private readonly Material surface;
        private Mesh roundedMesh;
        public Material EffectMaterial => surface;
        private Transform shopRoute;
        private Transform storageRoute, fridgeDoor;

        public BlockoutWorld(Transform parent, Material surface)
        {
            this.surface = surface;
            root = Group("Escenario", parent, Vector3.zero);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.68f, 0.61f, 0.74f);
            var sun = Group("Luz principal", root, Vector3.zero).gameObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.25f;
            sun.color = new Color(1f, 0.89f, 0.85f);
            sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            sun.shadows = LightShadows.Soft;
        }

        public Camera CameraAt(Color background)
        {
            var obj = Group("Cámara principal", root, Vector3.zero).gameObject;
            obj.tag = "MainCamera";
            var camera = obj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = background;
            camera.fieldOfView = 54f;
            camera.farClipPlane = 180f;
            obj.AddComponent<AudioListener>();
            return camera;
        }

        public CharacterController Player(Vector3 spawn)
        {
            var player = Group("Protagonista", root, spawn).gameObject;
            var motor = player.AddComponent<CharacterController>();
            motor.center = Vector3.up * 0.9f;
            motor.height = 1.8f;
            motor.radius = 0.34f;
            motor.minMoveDistance = 0f;
            return motor;
        }

        public void Shop(GameObject jetyModel, out Transform[] customers, out Transform door, out Transform fridge, out Transform apparition)
        {
            Box("Piso heladería", root, 0, -0.18f, 0, 12, 0.3f, 10, Vanilla, true);
            Box("Piso bodega", root, 3.5f, -0.18f, 7.65f, 5, 0.3f, 5.4f, Cocoa, true);
            Box("Muro izquierdo", root, -6, 1.65f, 0, 0.3f, 3.3f, 10, Blush, true);
            Box("Muro derecho", root, 6, 1.65f, 2.6f, 0.3f, 3.3f, 15.4f, Blush, true);
            Box("Fachada", root, 0, 1.65f, -5, 12, 3.3f, 0.3f, Blush, true);
            Box("Tabique de bodega", root, -2, 1.65f, 5, 8, 3.3f, 0.3f, Blush, true);
            Box("Tabique de bodega", root, 5, 1.65f, 5, 2, 3.3f, 0.3f, Blush, true);
            Box("Muro de bodega", root, 1, 1.5f, 7.7f, 0.3f, 3, 5.4f, Plum, true);
            Box("Fondo de bodega", root, 3.5f, 1.5f, 10.3f, 5, 3, 0.3f, Plum, true);

            DressShop();
            door = StorageDoor();
            fridge = DressStorage();

            customers = new Transform[6];
            for (int i = 0; i < customers.Length; i++)
            {
                float x = (i % 3 - 1) * 1.65f;
                var customer = Group($"Cliente {i + 1}", root, new Vector3(x, 0, 0.92f));
                customer.rotation = Quaternion.Euler(0, 180, 0);
                Person(customer, i % 3 == 0 ? Plum : i % 3 == 1 ? Mint : Rose);
                customer.gameObject.SetActive(i < 3);
                customers[i] = customer;
            }

            apparition = Enemy("Yeti del refrigerador", new Vector3(3, 0, 9.05f), 1.15f, Ice, true, out _, jetyModel);
            apparition.rotation = Quaternion.Euler(0, 180, 0);
            apparition.gameObject.SetActive(false);
        }

        public void Arena(bool finalRoom, out Transform portal)
        {
            DressArena(finalRoom);
            portal = null;
            if (finalRoom)
            {
                Chair();
                return;
            }

            portal = CandyPortal();
        }

        public Transform Person(Transform parent, Color color, bool seated = false)
        {
            var visual = Group("Visual del personaje", parent, Vector3.zero);
            Box("Cuerpo", visual, 0, 1.08f, 0, 0.59f, 0.66f, 0.42f, color);
            Box("Delantal", visual, 0, 0.98f, 0.225f, 0.42f, 0.49f, 0.05f, Cream);
            Ball("Cabeza", visual, 0, 1.62f, 0, 0.49f, Cream);
            Box("Gorro", visual, 0, 1.85f, -0.015f, 0.46f, 0.15f, 0.45f, color);
            Ball("Ojo izquierdo", visual, -0.08f, 1.65f, 0.23f, 0.064f, Dark);
            Ball("Ojo derecho", visual, 0.08f, 1.65f, 0.23f, 0.064f, Dark);
            foreach (float x in new[] { -0.16f, 0.16f })
            {
                if (seated)
                {
                    Box("Muslo", visual, x, 0.37f, 0.22f, 0.21f, 0.23f, 0.56f, Plum);
                    Box("Bota", visual, x, 0.12f, 0.49f, 0.22f, 0.49f, 0.24f, Plum);
                }
                else Box("Pierna", visual, x, 0.44f, 0, 0.21f, 0.64f, 0.24f, Plum);
            }
            Box("Brazo", visual, -0.4f, 1.035f, 0, 0.19f, 0.57f, 0.22f, color);
            Box("Brazo", visual, 0.4f, 1.035f, 0, 0.19f, 0.57f, 0.22f, color);
            return visual;
        }

        public Transform Scoop(Transform parent, GameObject model)
        {
            var pivot = Group("Cucharón", parent, new Vector3(0.44f, 0.98f, 0.11f));
            if (PortableModel.Instantiate("Ladle", pivot, surface) != null) return pivot;
            if (model != null)
            {
                MountModel(model, pivot);
                return pivot;
            }

            Box("Mango", pivot, 0, 0, 0.35f, 0.08f, 0.08f, 0.72f, Dark);
            Ball("Cazo", pivot, 0, 0.02f, 0.75f, 0.34f, Ice);
            return pivot;
        }

        public Transform Enemy(string name, Vector3 position, float size, Color color, bool jety,
            out Renderer body, GameObject model)
        {
            var enemy = Group(name, root, position);
            var customYeti = jety ? PortableModel.Instantiate("Yeti", enemy, surface) : null;
            var visual = customYeti != null ? customYeti : model != null ? MountModel(model, enemy) : null;
            if (visual == null && !jety && size > 1.76f)
                visual = PortableModel.Instantiate("IceGiant", enemy, surface);
            if (visual == null && !jety && size > 1.15f)
                visual = PortableModel.Instantiate("IceMedium", enemy, surface);
            if (visual == null && !jety) visual = PortableModel.Instantiate("Ice", enemy, surface);
            if (visual != null)
            {
                if (!jety || customYeti != null) visual.localRotation = Quaternion.Euler(0, 180, 0);
                NormalizeModel(visual, jety && size > 1.5f ? 3.52f : size * 1.52f);
                if (jety && customYeti != null)
                    customYeti.GetComponent<PortableRig>()?.Play("Idle", 1);
                if (!jety && size > 1.76f && visual.name != "IceGiant")
                {
                    var crown = Group(size > 1.8f ? "Corona de galleta" : "Bordes de armadura de azúcar", enemy, Vector3.up * (size * 1.30f));
                    var hue = size > 1.8f ? Vanilla : Lavender;
                    for (int i = 0; i < 6; i++)
                    {
                        float a = i * Mathf.PI / 3f;
                        var spike = Ellipsoid("Galleta", crown, new Vector3(Mathf.Cos(a), 0.12f + (i % 2) * 0.13f, Mathf.Sin(a)) * size * 0.33f,
                            new Vector3(0.29f, 0.59f, 0.19f) * size * 0.53f, hue);
                        spike.transform.localRotation = Quaternion.Euler(0, -i * 60f, i % 2 == 0 ? -14 : 11);
                    }
                }
                body = Shape(PrimitiveType.Sphere, "Señal de ataque y recuperación", enemy,
                    new Vector3(0f, 0.019f, 0f), new Vector3(size * 0.82f, 0.056f, size * 0.82f), color, false);
                return enemy;
            }

            body = jety ? ShapeJety(enemy, size) : ShapeIceCream(enemy, size, color);
            return enemy;
        }

        public void Tint(Renderer body, Color color) => Paint(body, color);

        public void Finale(Transform player, GameObject seatedModel)
        {
            player.position = new Vector3(0, 0.36f, 5.95f);
            player.rotation = Quaternion.Euler(0, 180, 0);
            if (seatedModel != null) MountModel(seatedModel, player);
            else Person(player, Rose, true);

            ThroneEffigy();
        }

        public void ThroneEffigy()
        {
            var lance = Group("Estandarte del Yeti", root, new Vector3(-1.16f, 0, 5.90f));
            Box("Asta", lance, 0, 1.07f, 0, 0.11f, 2.13f, 0.11f, Cream);
            var effigy = Group("Cabeza de hielo del Yeti", lance, new Vector3(0, 2.03f, 0));
            if (BuildYetiTrophy(effigy)) return;
            Ellipsoid("Efigie helada", effigy, Vector3.zero, new Vector3(0.69f, 0.53f, 0.54f), Ice);
            Ellipsoid("Hocico", effigy, new Vector3(0, -0.10f, -0.24f), new Vector3(0.42f, 0.23f, 0.16f), Frosting);
            Ball("Ojo izquierdo", effigy, -0.16f, 0.045f, -0.233f, 0.09f, Dark);
            Ball("Ojo derecho", effigy, 0.16f, 0.045f, -0.233f, 0.09f, Dark);
        }
        private void Chair()
        {
            DressThrone();
        }

        private Renderer Box(string name, Transform parent, float x, float y, float z,
            float sx, float sy, float sz, Color color, bool solid = false) =>
            Shape(PrimitiveType.Cube, name, parent, new Vector3(x, y, z), new Vector3(sx, sy, sz), color, solid);

        private Renderer Ball(string name, Transform parent, float x, float y, float z, float size, Color color) =>
            Shape(PrimitiveType.Sphere, name, parent, new Vector3(x, y, z), Vector3.one * size, color, false);

        private Renderer Shape(PrimitiveType type, string name, Transform parent, Vector3 point, Vector3 scale, Color color, bool solid)
        {
            var body = GameObject.CreatePrimitive(type);
            body.name = name;
            body.transform.SetParent(parent, false);
            body.transform.localPosition = point;
            body.transform.localScale = scale;
            if (!solid && body.TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
                UnityEngine.Object.Destroy(collider);
            }

            var renderer = body.GetComponent<MeshRenderer>();
            Paint(renderer, color);
            return renderer;
        }

        private void Paint(Renderer renderer, Color color)
        {
            if (!materials.TryGetValue(color, out var material))
            {
                material = new Material(surface) { color = color };
                materials.Add(color, material);
            }

            renderer.sharedMaterial = material;
        }

        private static Transform Group(string name, Transform parent, Vector3 position)
        {
            var item = new GameObject(name).transform;
            item.SetParent(parent, false);
            item.localPosition = position;
            return item;
        }

        internal static Transform MountModel(GameObject prefab, Transform parent)
        {
            var model = UnityEngine.Object.Instantiate(prefab, parent, false);
            foreach (var collider in model.GetComponentsInChildren<Collider>(true))
                collider.enabled = false;
            return model.transform;
        }

        public void Dispose()
        {
            foreach (var material in materials.Values) UnityEngine.Object.Destroy(material);
            if (roundedMesh != null) UnityEngine.Object.Destroy(roundedMesh);
            if (trailMesh != null) UnityEngine.Object.Destroy(trailMesh);
            if (stainMesh != null) UnityEngine.Object.Destroy(stainMesh);
            if (coneMesh != null) UnityEngine.Object.Destroy(coneMesh);
            if (portraitTexture != null) UnityEngine.Object.Destroy(portraitTexture);
        }
    }
}
