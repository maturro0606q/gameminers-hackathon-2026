using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld
    {
        private static readonly Color Vanilla = new Color32(255, 226, 163, 255);
        private static readonly Color Frosting = new Color32(255, 245, 221, 255);
        private static readonly Color Blush = new Color32(247, 180, 179, 255);
        private static readonly Color Jam = new Color32(199, 68, 98, 255);
        private static readonly Color Cocoa = new Color32(87, 55, 59, 255);
        private static readonly Color Pistachio = new Color32(163, 221, 180, 255);
        private static readonly Color Lagoon = new Color32(72, 174, 168, 255);
        private static readonly Color Blackberry = new Color32(48, 37, 65, 255);
        private static readonly Color Lavender = new Color32(144, 112, 171, 255);

        private readonly Transform[] flavorTrays = new Transform[3];

        public Transform FlavorTray(int index) => flavorTrays[index];

        public void MenuBackdrop(Camera camera)
        {
            DressArena(false);
            camera.transform.position = new Vector3(12.8f, 10.8f, -13.3f);
            camera.transform.LookAt(new Vector3(0, 0.7f, 0));
        }

        private void DressShop()
        {
            var shop = Group("Decoración de heladería", root, Vector3.zero);
            Box("Cielo interior", shop, 0, 3.34f, 0, 12f, 0.14f, 10f, Frosting);
            for (int x = -5; x <= 5; x++)
            for (int z = -4; z <= 4; z++)
                Box("Baldosa", shop, x, -0.022f, z, 0.96f, 0.014f, 0.96f,
                    (x + z) % 2 == 0 ? Blush : Frosting);

            for (int i = 0; i < 8; i++)
            {
                float z = -4.4f + i * 1.22f;
                Box("Rayas de papel mural", shop, -5.83f, 1.97f, z, 0.025f, 2.58f, 0.5f, Frosting);
                Box("Rayas de papel mural", shop, 5.83f, 1.97f, z, 0.025f, 2.58f, 0.5f, Frosting);
            }
            Box("Zócalo de chocolate", shop, -5.82f, 0.19f, 0, 0.1f, 0.36f, 9.7f, Cocoa);
            Box("Zócalo de chocolate", shop, 5.82f, 0.19f, 0, 0.1f, 0.36f, 9.7f, Cocoa);

            Box("Colisión de mostrador", shop, 0, 0.52f, 0, 4.9f, 1.04f, 0.72f, Jam, true);
            Rounded("Frente de fresa", shop, new Vector3(0, 0.53f, -0.035f),
                new Vector3(5.03f, 1.05f, 0.79f), Jam);
            Rounded("Cubierta de crema", shop, new Vector3(0, 1.08f, 0),
                new Vector3(5.18f, 0.18f, 0.99f), Frosting);
            for (int i = 0; i < 9; i++)
                Rounded("Moldura del mostrador", shop, new Vector3(-2.18f + i * 0.545f, 0.55f, -0.436f),
                    new Vector3(0.38f, 0.58f, 0.10f), Blush);
            for (int i = 0; i < 3; i++)
            {
                float x = -1.21f + i * 1.21f;
                Rounded("Cubeta de helado", shop, new Vector3(x, 1.18f, -0.06f),
                    new Vector3(0.42f, 0.13f, 0.39f), Cocoa);
                flavorTrays[i] = Ellipsoid("Sabor de vitrina", shop, new Vector3(x, 1.25f, -0.06f),
                    new Vector3(0.37f, 0.13f, 0.33f), i == 0 ? Jam : i == 1 ? Vanilla : Pistachio).transform;
                string flavor = i == 0 ? "FRESA" : i == 1 ? "VAINILLA" : "MENTA";
                Sign(shop, flavor, new Vector3(x, 1.02f, -0.505f), new Vector2(0.82f, 0.20f), Cocoa, Frosting, 0.025f);
            }

            for (int i = 0; i < 3; i++)
            {
                float x = -3.85f + i * 2.9f;
                Box("Cable de lámpara", shop, x, 3.04f, -1.6f, 0.045f, 0.52f, 0.045f, Cocoa);
                Ellipsoid("Pantalla de lámpara", shop, new Vector3(x, 2.76f, -1.6f),
                    new Vector3(0.87f, 0.35f, 0.87f), i == 1 ? Jam : Lagoon);
                Ellipsoid("Difusor de lámpara", shop, new Vector3(x, 2.62f, -1.6f),
                    new Vector3(0.56f, 0.14f, 0.56f), Frosting);
            }

            Sign(shop, "HELADERÍA\nLa Cereza", new Vector3(-0.75f, 2.58f, 4.70f),
                new Vector2(3.75f, 0.88f), Jam, Frosting, 0.074f);
            Sign(shop, "VAINILLA   FRESA   MENTA", new Vector3(-2.21f, 1.99f, 4.69f),
                new Vector2(3.65f, 0.31f), Cocoa, Frosting, 0.041f);

            shopRoute = Group("Ruta a bodega", shop, Vector3.zero);
            for (int i = 0; i < 5; i++)
                Chevron(shopRoute, new Vector3(3f, 0.008f, 0.7f + i * 0.86f), 0.69f, Lagoon);
            shopRoute.gameObject.SetActive(false);
        }

        private Transform StorageDoor()
        {
            var doorway = Group("Marco de bodega", root, new Vector3(3, 0, 5));
            Rounded("Marco izquierdo", doorway, new Vector3(-0.98f, 1.18f, -0.05f),
                new Vector3(0.19f, 2.38f, 0.34f), Cocoa);
            Rounded("Marco derecho", doorway, new Vector3(0.98f, 1.18f, -0.05f),
                new Vector3(0.19f, 2.38f, 0.34f), Cocoa);
            Sign(doorway, "BODEGA", new Vector3(0, 2.49f, -0.16f),
                new Vector2(1.97f, 0.43f), Cocoa, Frosting, 0.073f);

            var door = Group("Puerta de bodega", doorway, Vector3.up * 1.1f);
            var body = Rounded("Puerta esmaltada", door, Vector3.zero,
                new Vector3(1.87f, 2.2f, 0.23f), Lagoon);
            var collider = body.gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one;
            Rounded("Panel de bodega", door, new Vector3(0, 0.29f, -0.126f),
                new Vector3(1.32f, 0.64f, 0.031f), Pistachio);
            Rounded("Manilla de bodega", door, new Vector3(0.59f, -0.02f, -0.16f),
                new Vector3(0.19f, 0.10f, 0.12f), Vanilla);
            return door;
        }

        private Transform DressStorage()
        {
            var storage = Group("Estanterías y cámara fría", root, Vector3.zero);
            Box("Techo de bodega", storage, 3.5f, 3.19f, 7.66f, 5f, 0.15f, 5.4f, Cocoa);
            for (int row = 0; row < 3; row++)
            {
                float y = 0.47f + 0.65f * row;
                Box("Bandeja de almacenaje", storage, 5.36f, y, 8.47f, 0.95f, 0.13f, 2.58f, Vanilla, true);
                for (int i = 0; i < 3; i++)
                {
                    float z = 7.64f + i * 0.80f;
                    Rounded("Caja de suministros", storage, new Vector3(5.43f, y + 0.25f, z),
                        new Vector3(0.66f, 0.39f, 0.57f), (row + i) % 2 == 0 ? Blush : Vanilla);
                    Rounded("Etiqueta de suministros", storage, new Vector3(5.095f, y + 0.25f, z),
                        new Vector3(0.015f, 0.16f, 0.29f), Frosting);
                }
            }
            for (int i = 0; i < 2; i++)
                Box("Pie de estantería", storage, 4.92f, 1.02f, 7.24f + i * 2.45f, 0.12f, 2.05f, 0.12f, Cocoa, true);

            var fridge = BuildFridge(new Vector3(3f, 0, 9.15f));
            storageRoute = Group("Ruta al refrigerador", storage, Vector3.zero);
            for (int i = 0; i < 4; i++)
                Chevron(storageRoute, new Vector3(3f, 0.015f, 5.7f + i * 0.56f), 0.61f, Vanilla);
            storageRoute.gameObject.SetActive(false);
            return fridge;
        }

        private Transform BuildFridge(Vector3 position)
        {
            var fridge = Group("Refrigerador", root, position);
            Box("Fondo de la cámara fría", fridge, 0, 1.16f, 0.45f, 1.54f, 2.32f, 0.13f, Dark, true);
            Box("Lateral izquierdo", fridge, -0.73f, 1.16f, 0, 0.15f, 2.32f, 1.02f, Frosting, true);
            Box("Lateral derecho", fridge, 0.73f, 1.16f, 0, 0.15f, 2.32f, 1.02f, Frosting, true);
            Box("Parte superior", fridge, 0, 2.28f, 0, 1.54f, 0.14f, 1.02f, Frosting);
            Box("Umbral", fridge, 0, 0.06f, 0, 1.54f, 0.17f, 1.02f, Frosting);
            Ellipsoid("Umbral al mundo helado", fridge, new Vector3(0, 1.11f, 0.34f),
                new Vector3(1.28f, 2.13f, 0.13f), new Color32(107, 209, 252, 255));
            Box("Punto de interacción", fridge, 0, 1.18f, -0.44f, 1.42f, 2.26f, 0.23f, Ice, true).enabled = false;
            fridgeDoor = Group("Bisagra del refrigerador", fridge, new Vector3(-0.75f, 0, -0.53f));
            Rounded("Puerta esmaltada", fridgeDoor, new Vector3(0.75f, 1.16f, 0),
                new Vector3(1.48f, 2.20f, 0.15f), Ice);
            Rounded("Panel superior", fridgeDoor, new Vector3(0.75f, 1.71f, -0.09f),
                new Vector3(1.25f, 0.81f, 0.085f), Frosting);
            Rounded("Manilla del refrigerador", fridgeDoor, new Vector3(1.23f, 1.46f, -0.13f),
                new Vector3(0.13f, 0.34f, 0.15f), Lagoon);
            Sign(fridge, "RESERVA", new Vector3(0, 2.54f, -0.22f),
                new Vector2(1.54f, 0.34f), Blackberry, Frosting, 0.056f);

            return fridge;
        }

        public void ShowShopRoute(bool toDoor, bool toFridge)
        {
            shopRoute.gameObject.SetActive(toDoor);
            storageRoute.gameObject.SetActive(toFridge);
        }

        private void DressArena(bool finalRoom)
        {
            var scene = Group(finalRoom ? "Palacio helado" : "Valle de confites", root, Vector3.zero);
            var baseColor = finalRoom ? Lavender : Vanilla;
            Box("Superficie navegable", scene, 0, -0.18f, 0, 19.4f, 0.3f, 15.4f, baseColor, true);
            Rounded("Estrato de chocolate", scene, new Vector3(0, -0.64f, 0),
                new Vector3(20.1f, 1.03f, 15.95f), finalRoom ? Blackberry : Cocoa);
            Box("Paisaje inferior", scene, 0, -1.44f, 0, 72f, 0.24f, 62f, finalRoom ? Blackberry : Blush);
            var line = finalRoom ? Cocoa : Jam;
            Box("Límite frontal", scene, 0, 0.015f, -7.22f, 18.82f, 0.055f, 0.12f, line);
            Box("Límite trasero", scene, 0, 0.015f, 7.22f, 18.82f, 0.055f, 0.12f, line);
            Box("Límite izquierdo", scene, -9.32f, 0.015f, 0, 0.12f, 0.055f, 14.44f, line);
            Box("Límite derecho", scene, 9.32f, 0.015f, 0, 0.12f, 0.055f, 14.44f, line);

            if (!finalRoom) PastelValley(scene);
            for (int i = 0; i < 9; i++)
            {
                float x = -8.75f + i * 2.18f;
                if (finalRoom)
                {
                    Box("Loseta de palacio", scene, x, -0.017f, 0, 2.03f, 0.022f, 13.88f,
                        i % 2 == 0 ? Lavender : Plum);
                    Crystal(scene, new Vector3(x, -0.12f, 8.9f), 1.7f + 0.38f * (i % 3), i % 2 == 0 ? Ice : Lavender);
                }
                else
                {
                    var color = i % 3 == 0 ? Jam : i % 3 == 1 ? Pistachio : Blush;
                    Ellipsoid("Chispa de borde", scene, new Vector3(x, 0.09f, -7.47f),
                        new Vector3(0.56f, 0.20f, 0.23f), color);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                float z = -6.2f + 2.48f * i;
                Ellipsoid("Referencia de profundidad", scene, new Vector3(-9.55f, 0.17f, z),
                    new Vector3(0.36f, 0.34f, 0.93f), finalRoom ? Ice : Jam);
                Ellipsoid("Referencia de profundidad", scene, new Vector3(9.55f, 0.17f, z),
                    new Vector3(0.36f, 0.34f, 0.93f), finalRoom ? Ice : Lagoon);
            }

            if (!finalRoom) return;
            for (int i = 0; i < 7; i++)
                Box("Alfombra del trono", scene, 0, -0.001f, -4.65f + i * 1.39f, 2.30f, 0.027f, 1.37f, i % 2 == 0 ? Jam : Rose);
            for (int side = -1; side <= 1; side += 2)
            {
                Box("Borde de la alfombra", scene, side * 1.22f, 0.016f, -0.48f, 0.10f, 0.03f, 9.7f, Vanilla);
                Crystal(scene, new Vector3(side * 4.9f, -0.08f, 8.0f), 3.35f, Ice);
                Crystal(scene, new Vector3(side * 7.8f, -0.12f, 7.9f), 2.8f, Lavender);
            }
        }

        private Transform CandyPortal()
        {
            var gate = Group("Portal a la sala del Yeti", root, new Vector3(7f, 0, 4.5f));
            CandyPillar(gate, new Vector3(-1.16f, 0, 0), 2.45f, Jam);
            CandyPillar(gate, new Vector3(1.16f, 0, 0), 2.45f, Jam);
            Sign(gate, "SALA DEL YETI", new Vector3(0, 2.81f, 0), new Vector2(2.74f, 0.52f), Cocoa, Frosting, 0.065f);
            Ellipsoid("Velo de caramelo", gate, new Vector3(0, 1.17f, 0),
                new Vector3(1.77f, 2.23f, 0.17f), Lagoon);
            Ellipsoid("Centro de portal", gate, new Vector3(0, 1.17f, -0.104f),
                new Vector3(1.35f, 1.77f, 0.07f), Pistachio);
            for (int i = 0; i < 4; i++)
                Chevron(gate, new Vector3(0, 0.017f, -2.3f + i * 0.55f), 0.81f, Lagoon);
            gate.gameObject.SetActive(false);
            return gate;
        }

        private void DressThrone()
        {
            var throne = Group("Trono de helado", root, new Vector3(0, 0, 5.98f));
            Rounded("Pedestal de chocolate", throne, new Vector3(0, 0.17f, 0),
                new Vector3(3.58f, 0.37f, 2.08f), Cocoa);
            Rounded("Moldura de vainilla", throne, new Vector3(0, 0.36f, 0),
                new Vector3(2.28f, 0.20f, 1.58f), Vanilla);
            Rounded("Asiento de fresa", throne, new Vector3(0, 0.49f, 0.02f),
                new Vector3(1.74f, 0.19f, 1.27f), Jam);
            Rounded("Respaldo de galleta", throne, new Vector3(0, 1.52f, 0.64f),
                new Vector3(2.14f, 2.55f, 0.47f), Cocoa);
            Rounded("Tapiz de fresa", throne, new Vector3(0, 1.51f, 0.43f),
                new Vector3(1.71f, 2.03f, 0.18f), Jam);
            Ellipsoid("Glaseado de la corona", throne, new Vector3(0, 2.77f, 0.57f),
                new Vector3(1.23f, 0.49f, 0.59f), Frosting);
            Ellipsoid("Cereza de la corona", throne, new Vector3(0, 3.02f, 0.55f), Vector3.one * 0.37f, Jam);
            for (int side = -1; side <= 1; side += 2)
            {
                Rounded("Apoyabrazos acaramelado", throne, new Vector3(side * 0.94f, 0.91f, 0.05f),
                    new Vector3(0.36f, 0.36f, 1.35f), Vanilla);
                CandyPillar(throne, new Vector3(side * 1.71f, 0.08f, 0.66f), 2.92f, Jam);
            }
        }

        private void Sign(Transform parent, string words, Vector3 position, Vector2 size, Color plate, Color ink, float characterSize)
        {
            var sign = Group(words.Replace('\n', ' '), parent, position);
            Rounded("Placa", sign, Vector3.zero, new Vector3(size.x, size.y, 0.15f), plate);
            var label = Group("Rótulo", sign, new Vector3(0, 0, -0.091f)).gameObject.AddComponent<TextMesh>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.GetComponent<MeshRenderer>().sharedMaterial = label.font.material;
            label.text = words;
            label.fontSize = 34;
            label.characterSize = characterSize;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontStyle = FontStyle.Bold;
            label.color = ink;
        }

        private void Chevron(Transform parent, Vector3 position, float width, Color color)
        {
            var arrow = Group("Flecha de recorrido", parent, position);
            for (int side = -1; side <= 1; side += 2)
            {
                var stem = Rounded("Trazo", arrow, new Vector3(side * width * 0.22f, 0, 0),
                    new Vector3(0.13f, 0.029f, width * 0.72f), color);
                stem.transform.localRotation = Quaternion.Euler(0, -side * 48f, 0);
            }
        }

        private void CandyPillar(Transform parent, Vector3 position, float height, Color color)
        {
            var pillar = Group("Pilar de caramelo", parent, position);
            const int bands = 7;
            for (int i = 0; i < bands; i++)
                Rounded("Anillo de caramelo", pillar, new Vector3(0, (i + 0.5f) * height / bands, 0),
                    new Vector3(0.42f, height / bands + 0.04f, 0.42f), i % 2 == 0 ? color : Frosting);
            Ellipsoid("Confite superior", pillar, new Vector3(0, height, 0), Vector3.one * 0.66f, color);
        }

        private void Crystal(Transform parent, Vector3 position, float height, Color color)
        {
            for (int i = -1; i <= 1; i++)
            {
                float length = i == 0 ? height : height * 0.64f;
                var crystal = Rounded("Cristal de azúcar", parent,
                    position + new Vector3(i * 0.43f, length * 0.39f, i * 0.10f),
                    new Vector3(0.49f, length, 0.49f), color);
                crystal.transform.localRotation = Quaternion.Euler(i * 8f, 33f, i * -24f);
            }
        }

        private Renderer Ellipsoid(string name, Transform parent, Vector3 position, Vector3 scale, Color color) =>
            Shape(PrimitiveType.Sphere, name, parent, position, scale, color, false);

        private Renderer Rounded(string name, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            if (roundedMesh == null) roundedMesh = CreateRoundedCube();
            var shape = Group(name, parent, position).gameObject;
            shape.transform.localScale = scale;
            shape.AddComponent<MeshFilter>().sharedMesh = roundedMesh;
            var renderer = shape.AddComponent<MeshRenderer>();
            Paint(renderer, color);
            return renderer;
        }

        private static Mesh CreateRoundedCube()
        {
            const int side = 8;
            const float edge = 0.14f;
            float[] coordinates = { -0.5f, -0.47f, -0.42f, -0.36f, 0.36f, 0.42f, 0.47f, 0.5f };
            var vertices = new Vector3[6 * side * side];
            var normals = new Vector3[vertices.Length];
            var triangles = new int[6 * (side - 1) * (side - 1) * 6];
            var directions = new[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };
            int triangle = 0;

            for (int face = 0; face < directions.Length; face++)
            {
                var n = directions[face];
                var u = Mathf.Abs(n.y) > 0.5f ? Vector3.right : Vector3.Cross(Vector3.up, n);
                var v = Vector3.Cross(n, u);
                int offset = face * side * side;
                for (int y = 0; y < side; y++)
                for (int x = 0; x < side; x++)
                {
                    int index = offset + y * side + x;
                    var point = n * 0.5f + u * coordinates[x] + v * coordinates[y];
                    var inside = new Vector3(Mathf.Clamp(point.x, -0.36f, 0.36f),
                        Mathf.Clamp(point.y, -0.36f, 0.36f), Mathf.Clamp(point.z, -0.36f, 0.36f));
                    normals[index] = (point - inside).normalized;
                    vertices[index] = inside + normals[index] * edge;
                    if (x == side - 1 || y == side - 1) continue;
                    triangles[triangle++] = index;
                    triangles[triangle++] = index + 1;
                    triangles[triangle++] = index + side;
                    triangles[triangle++] = index + 1;
                    triangles[triangle++] = index + side + 1;
                    triangles[triangle++] = index + side;
                }
            }

            var mesh = new Mesh { name = "Cubo biselado", vertices = vertices, normals = normals, triangles = triangles };
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
