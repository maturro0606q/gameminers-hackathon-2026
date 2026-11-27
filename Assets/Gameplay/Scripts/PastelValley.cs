using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld
    {
        private void PastelValley(Transform scene)
        {
            var pink = new Color32(245, 148, 170, 255);
            var pastry = new Color32(244, 186, 126, 255);
            var tan = new Color32(206, 143, 99, 255);
            var cloud = new Color32(255, 248, 247, 255);
            Color[] scoops = { new Color32(255, 213, 129, 255), new Color32(146, 217, 176, 255),
                new Color32(219, 176, 226, 255), new Color32(181, 222, 136, 255), new Color32(239, 162, 182, 255) };
            for (int band = 0; band < 3; band++)
            {
                for (int i = 0; i < 9; i++)
                {
                    float x = -34 + i * 8.55f + band * 1.7f;
                    float z = 14.9f + band * 8.6f + Mathf.Sin(i * 4.2f) * 2.6f;
                    float h = 0.74f + Mathf.Abs(Mathf.Sin(i * 1.78f - band)) * 0.47f;
                    var cake = Group("Colina de bizcocho y glaseado", scene, new Vector3(x, -1.08f, z));
                    Ellipsoid("Bizcocho", cake, Vector3.zero, new Vector3(11f, 1.55f, 8f), pastry);
                    Ellipsoid("Glaseado de fresa", cake, Vector3.up * 0.52f, new Vector3(10.4f, 1.21f, 7.65f), pink);
                    if (band == 0 || i % 3 != 1)
                        WaferTree(cake, new Vector3(0.8f * Mathf.Cos(i * 2f), 0.62f, 0.1f), h,
                            Color.Lerp(scoops[(i + band * 2) % scoops.Length], cloud, band * 0.15f), pastry, tan);
                    if (band == 0)
                    {
                        CandyCane(cake, new Vector3(-2.75f, 0.43f, -1.6f), 0.9f + h * 0.71f, i % 2 == 0);
                        CandyCane(cake, new Vector3(-1.97f, 0.58f, -1.75f), 0.64f + h * 0.52f, false);
                    }
                }
            }
            for (int i = 0; i < 7; i++)
            {
                var p = new Vector3(-43 + i * 14.2f, 8.55f + 1.51f * Mathf.Sin(i * 2.6f), 37 + i % 3 * 4);
                for (int j = 0; j < 4; j++)
                    Ellipsoid("Nube de merengue", scene, p + new Vector3(j * 2.3f, Mathf.Sin(j * 1.05f) * 0.8f, 0),
                        new Vector3(5.5f, 1.55f + j % 3 * 0.42f, 2.8f), cloud);
            }
            for (int i = 0; i < 29; i++)
            {
                float x = -9.20f + i % 10 * 1.96f + Mathf.Sin(i * 8.63f) * 0.24f;
                float z = i % 3 == 1 ? -6.72f + 0.24f * Mathf.Cos(i) : 4.5f + 1.61f * Mathf.Sin(i * 2.1f);
                var baked = Color.Lerp(Vanilla, pastry, 0.30f + (i % 3) * 0.12f);
                Ellipsoid("Miga en el camino", scene, new Vector3(x, -0.024f, z),
                    new Vector3(0.25f + (i % 4) * 0.16f, 0.013f, 0.08f + i % 3 * 0.042f), baked);
            }
            for (int i = 0; i < 16; i++)
            {
                float x = -9 + i * 1.2f;
                var p = new Vector3(x, 0, 7.48f + 0.23f * Mathf.Sin(i));
                for (int j = 0; j < 3; j++)
                {
                    var tuft = Ellipsoid("Pétalo de azúcar", scene, p + new Vector3((j - 1) * 0.11f, 0.13f, 0),
                        new Vector3(0.082f, 0.32f, 0.12f), i % 2 == 0 ? pink : pastry);
                    tuft.transform.localRotation = Quaternion.Euler(0, 0, (1 - j) * 35);
                }
            }
        }

        private void WaferTree(Transform parent, Vector3 at, float scale, Color scoop, Color pastry, Color toasted)
        {
            var tree = Group("Árbol de helado", parent, at);
            tree.localScale = Vector3.one * scale;
            Shape(PrimitiveType.Cylinder, "Tronco de barquillo", tree, new Vector3(0, 1.66f, 0),
                new Vector3(1.40f, 1.72f, 1.4f), pastry, false);
            for (int side = -1; side <= 1; side += 2)
            for (int row = 0; row < 5; row++)
            {
                var stripe = Box("Trama tostada del barquillo", tree, 0, 0.36f + row * 0.60f, -0.66f,
                    1.27f, 0.084f, 0.055f, toasted);
                stripe.transform.localRotation = Quaternion.Euler(0, 0, side * 33);
            }

            Ellipsoid("Bola de helado", tree, new Vector3(0, 3.78f, 0), new Vector3(3.6f, 3.22f, 3.42f), scoop);
            for (int i = 0; i < 9; i++)
            {
                float a = i * 2 * Mathf.PI / 9f;
                Ellipsoid("Borde de helado", tree, new Vector3(Mathf.Sin(a) * 1.34f, 2.91f + 0.11f * Mathf.Sin(a * 3), Mathf.Cos(a) * 1.42f),
                    new Vector3(1.21f, 0.74f, 1.24f), scoop);
            }
            for (int i = 0; i < 3; i++)
            {
                float x = (i - 1f) * 0.80f;
                float y = 3.5f + 0.53f * (i % 2);
                float z = -1.5f + Mathf.Abs(x) * 0.21f;
                Ellipsoid("Cereza", tree, new Vector3(x, y, z), Vector3.one * (0.29f + i * 0.022f), Jam);
                var stem = Box("Tallo de cereza", tree, x + 0.07f, y + 0.21f, z, 0.046f, 0.29f, 0.046f, Cocoa);
                stem.transform.localRotation = Quaternion.Euler(0, 0, -23 + i * 16);
            }
        }

        private void CandyCane(Transform parent, Vector3 position, float height, bool left)
        {
            var cane = Group("Bastón confitado", parent, position);
            for (int i = 0; i < 23; i++)
            {
                float t = i / 22f;
                Vector3 point;
                if (t < 0.64f) point = new Vector3(0, t * height / 0.8f, 0);
                else
                {
                    float a = (t - 0.64f) / 0.36f * Mathf.PI * 1.1f;
                    point = new Vector3((1 - Mathf.Cos(a)) * height * (left ? -0.16f : 0.16f),
                        height * (0.8f + 0.16f * Mathf.Sin(a)), 0);
                }
                Ellipsoid("Franja", cane, point, Vector3.one * (height * 0.10f), i / 3 % 2 == 0 ? Frosting : Jam);
            }
        }
    }
}