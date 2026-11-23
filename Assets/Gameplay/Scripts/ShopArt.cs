using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld
    {
        private Mesh coneMesh;
        private Texture2D portraitTexture;
        private Transform preparedCone;
        public Transform CodexFrame { get; private set; }

        private void OutfitTruck(Transform counter)
        {
            var teal = new Color32(60, 126, 122, 255);
            var brass = new Color32(212, 159, 94, 255);
            var warm = new Color32(255, 221, 170, 255);
            Rounded("Borde tallado del mesón", counter, new Vector3(-0.49f, 1.075f, 0),
                new Vector3(0.06f, 0.14f, 3.2f), Cocoa);
            for (int i = 0; i < 3; i++)
            {
                float z = (i - 1) * 0.96f;
                Rounded("Panel esmaltado", counter, new Vector3(-0.48f, 0.54f, z),
                    new Vector3(0.06f, 0.72f, 0.86f), teal);
                Rounded("Tirador", counter, new Vector3(-0.52f, 0.69f, z),
                    new Vector3(0.075f, 0.069f, 0.25f), brass);
                var label = Group("Nombre del sabor", counter, new Vector3(-0.524f, 1.09f, (i - 1) * 0.61f));
                label.rotation = Quaternion.Euler(0, 90, 0);
                Sign(label, i == 0 ? "FRESA" : i == 1 ? "VAINILLA" : "MENTA", Vector3.zero,
                    new Vector2(0.52f, 0.16f), teal, Frosting, 0.021f);
            }
            for (int i = 0; i < 3; i++)
            {
                float z = i * 1.63f - 2.42f;
                var lamp = Group("Luces de servicio", root, new Vector3(0.0f, 2.65f, z));
                Ellipsoid("Pantalla de cobre", lamp, Vector3.zero, new Vector3(0.60f, 0.24f, 0.60f), brass);
                Ellipsoid("Bombilla cálida", lamp, Vector3.down * 0.104f, new Vector3(0.23f, 0.10f, 0.23f), warm);
                var light = lamp.gameObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 4.5f;
                light.intensity = 0.86f;
                light.color = new Color32(255, 213, 169, 255);
            }

            Rounded("Repisa de los frascos", root, new Vector3(-2.07f, 1.49f, 0.68f),
                new Vector3(0.53f, 0.11f, 3.1f), Cocoa);
            for (int i = 0; i < 5; i++)
            {
                float z = -0.54f + i * 0.54f;
                var tin = Group("Frasco de toppings", root, new Vector3(-2.10f, 1.55f, z));
                Rounded("Frasco", tin, Vector3.up * 0.18f, new Vector3(0.29f, 0.35f, 0.29f),
                    i % 3 == 0 ? Jam : i % 3 == 1 ? Vanilla : Pistachio);
                Rounded("Tapa", tin, Vector3.up * 0.37f, new Vector3(0.32f, 0.07f, 0.32f), teal);
                Box("Etiqueta", tin, 0.15f, 0.18f, 0, 0.012f, 0.11f, 0.17f, Frosting);
            }
            var menu = Group("Carta de sabores", root, new Vector3(-2.29f, 2.19f, 0.54f));
            menu.rotation = Quaternion.Euler(0, -90f, 0);
            Sign(menu, "LA CEREZA\nFRESA  •  VAINILLA  •  MENTA", Vector3.zero,
                new Vector2(2.63f, 0.70f), Blackberry, Frosting, 0.04f);
            Rounded("Caja registradora", counter, new Vector3(0, 1.33f, -1.34f),
                new Vector3(0.51f, 0.30f, 0.47f), teal);
            Box("Pantalla de caja", counter, -0.261f, 1.39f, -1.34f, 0.025f, 0.055f, 0.23f, Pistachio);
            for (int i = 0; i < 4; i++)
                Rounded("Taza de servicio", counter, new Vector3(0.20f, 1.25f + i * 0.025f, 1.33f),
                    new Vector3(0.23f, 0.17f, 0.23f), i % 2 == 0 ? Frosting : Vanilla);
            var utensil = Group("Cuchara de heladería", counter, new Vector3(-0.33f, 1.232f, 0.82f));
            utensil.localRotation = Quaternion.Euler(-7f, -27f, 0f);
            utensil.localScale = Vector3.one * 0.20f;
            PortableModel.Instantiate("Ladle", utensil, surface);
            BuildPortrait();
        }

        private void BuildPortrait()
        {
            var frame = Group("Codex · Empleado del mes", root, new Vector3(-2.28f, 1.90f, -1.54f));
            frame.rotation = Quaternion.Euler(0, -90f, 0);
            CodexFrame = frame;
            Rounded("Marco de madera", frame, Vector3.zero, new Vector3(1.28f, 0.93f, 0.13f), Cocoa);
            Rounded("Paspartú", frame, new Vector3(0, 0.047f, -0.069f),
                new Vector3(1.15f, 0.73f, 0.028f), Frosting);
            Sign(frame, "EMPLEADO DEL MES", new Vector3(0, -0.337f, -0.091f),
                new Vector2(1.02f, 0.15f), Cocoa, Frosting, 0.020f);

            string path = PortableModel.PathFor("CodexPortrait.png");
            if (!File.Exists(path)) return;
            portraitTexture = new Texture2D(2, 2);
            if (!portraitTexture.LoadImage(File.ReadAllBytes(path), true)) return;
            var art = new GameObject("Retrato oficial de Codex", typeof(RectTransform), typeof(Canvas));
            var rect = art.GetComponent<RectTransform>();
            rect.SetParent(frame, false);
            rect.localPosition = new Vector3(0, 0.070f, -0.11f);
            rect.localScale = Vector3.one * 0.002f;
            float ratio = portraitTexture.width / (float)portraitTexture.height;
            float width = Mathf.Min(496f, 270f * ratio);
            rect.sizeDelta = new Vector2(width, width / ratio);
            art.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            var image = new GameObject("Imagen impresa", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            image.rectTransform.SetParent(rect, false);
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = image.rectTransform.offsetMax = Vector2.zero;
            image.texture = portraitTexture;
            image.raycastTarget = false;
        }

        private void DressPark()
        {
            Color lawn = new Color32(129, 183, 124, 255);
            Color green = new Color32(89, 154, 116, 255);
            Color lightGreen = new Color32(161, 202, 110, 255);
            var park = Group("Parque La Cereza", root, Vector3.zero);
            Box("Pradera", park, 13f, -0.22f, 1f, 25f, 0.20f, 37f, lawn);
            Box("Paseo de piedra", park, 4.0f, -0.099f, 0, 3.45f, 0.14f, 25f, Cream);
            Rounded("Sendero al césped", park, new Vector3(8.6f, -0.041f, 0.13f), new Vector3(9.7f, 0.06f, 2.1f), Cream);
            for (int i = 0; i < 9; i++)
            {
                float x = 7.5f + i % 3 * 4.1f;
                float z = -8.4f + i * 2.1f;
                var tree = Group("Árbol del parque", park, new Vector3(x, -0.08f, z));
                var trunk = Shape(PrimitiveType.Cylinder, "Tronco", tree, new Vector3(0, 1.30f, 0),
                    new Vector3(0.42f, 1.45f, 0.42f), Cocoa, false);
                trunk.transform.localRotation = Quaternion.Euler(0, 0, i % 2 == 0 ? -5 : 5);
                for (int j = 0; j < 4; j++)
                {
                    float angle = j * 1.8f;
                    Ellipsoid("Follaje", tree, new Vector3(Mathf.Sin(angle) * 0.88f, 3.12f + j % 2 * 0.51f, Mathf.Cos(angle) * 0.86f),
                        new Vector3(2.40f, 2.20f + j % 2 * 0.32f, 2.58f), j % 2 == 0 ? green : lightGreen);
                }
            }
            PicnicBench(park, new Vector3(7.29f, 0, -2.26f));
            PicnicBench(park, new Vector3(7.87f, 0, 2.16f));
            for (int i = 0; i < 12; i++)
            {
                float z = -6 + i * 1.02f;
                Ellipsoid("Arbusto", park, new Vector3(10.6f, 0.33f, z), new Vector3(1.14f, 0.80f, 1.28f), green);
                for (int j = 0; j < 4; j++)
                    Ellipsoid("Flor", park, new Vector3(9.96f - 0.19f * j, 0.12f + (j % 3) * 0.08f, z + 0.13f * j),
                        Vector3.one * 0.20f, (i + j) % 2 == 0 ? Rose : Vanilla);
            }
            var notice = Group("Entrada del parque", park, new Vector3(6.85f, 1.4f, 3.45f));
            notice.rotation = Quaternion.Euler(0, 107f, 0);
            Sign(notice, "PARQUE LA CEREZA", Vector3.zero, new Vector2(1.95f, 0.48f), Cocoa, Frosting, 0.039f);
            Box("Poste", park, 6.85f, 0.66f, 3.45f, 0.15f, 1.43f, 0.15f, Cocoa);
        }

        private void PicnicBench(Transform parent, Vector3 at)
        {
            var bench = Group("Banca de parque", parent, at);
            bench.rotation = Quaternion.Euler(0, -90f, 0);
            for (int i = 0; i < 3; i++)
            {
                Rounded("Listón de asiento", bench, new Vector3(0, 0.45f, (i - 1) * 0.18f),
                    new Vector3(2.05f, 0.085f, 0.155f), Cocoa);
                Rounded("Listón de respaldo", bench, new Vector3(0, 0.67f + i * 0.12f, -0.24f),
                    new Vector3(2.05f, 0.095f, 0.075f), Cocoa);
            }
            foreach (float side in new[] {-1f, 1f})
                Box("Soporte hierro", bench, side * 0.79f, 0.21f, 0, 0.10f, 0.43f, 0.39f, Blackberry);
        }

        public void ServeCone(int flavor)
        {
            preparedCone = IceCreamCone(root, new Vector3(1.71f, 1.23f, 0.92f), flavor, 1f);
        }

        public void HandOffCone(Transform customer, int flavor, ShopGuest guest)
        {
            if (preparedCone != null) UnityEngine.Object.Destroy(preparedCone.gameObject);
            var cone = IceCreamCone(customer, new Vector3(0.30f, 1.13f, 0.37f), flavor, 0.85f);
            guest.Receive(cone);
        }

        private Transform IceCreamCone(Transform parent, Vector3 position, int flavor, float scale)
        {
            var cone = Group("Cono entregado", parent, position);
            cone.localScale = Vector3.one * scale;
            if (coneMesh == null)
            {
                const int sides = 24;
                var vertices = new Vector3[sides + 2];
                var triangles = new int[sides * 6];
                vertices[sides] = new Vector3(0, 0.055f, 0);
                for (int i = 0; i < sides; i++)
                {
                    float angle = i * 2 * Mathf.PI / sides;
                    vertices[i] = new Vector3(0.12f * Mathf.Cos(angle), 0.055f, 0.12f * Mathf.Sin(angle));
                    int next = (i + 1) % sides;
                    triangles[i*6] = i; triangles[i*6+1] = next; triangles[i*6+2] = sides+1;
                    triangles[i*6+3] = next; triangles[i*6+4] = i; triangles[i*6+5] = sides;
                }
                vertices[sides+1] = new Vector3(0, -0.22f, 0);
                coneMesh = new Mesh { name = "Barquillo", vertices = vertices, triangles = triangles };
                coneMesh.RecalculateNormals();
            }
            cone.gameObject.AddComponent<MeshFilter>().sharedMesh = coneMesh;
            Paint(cone.gameObject.AddComponent<MeshRenderer>(), new Color32(204, 133, 76, 255));
            Ellipsoid("Bola de helado", cone, Vector3.up * 0.092f, Vector3.one * 0.24f,
                flavor == 0 ? Rose : flavor == 1 ? Vanilla : Mint);
            return cone;
        }
    }
}