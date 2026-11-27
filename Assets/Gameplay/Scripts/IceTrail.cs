using UnityEngine;
using UnityEngine.Rendering;

namespace GameMiners.Gameplay
{
    internal static class IceTrail
    {
        internal readonly struct Area
        {
            public readonly string Name;
            public readonly float Entrance, Gate, FirstEnemy;
            public readonly int Regular, Guards;

            public Area(string name, float entrance, float gate, float firstEnemy, int regular, int guards)
            {
                Name = name;
                Entrance = entrance;
                Gate = gate;
                FirstEnemy = firstEnemy;
                Regular = regular;
                Guards = guards;
            }
        }

        internal static readonly Area[] Areas =
        {
            new("Huerto de cerezas", -3.8f, 23f, 3.8f, 5, 0),
            new("Molino de barquillo", 24.5f, 50f, 29f, 4, 2),
            new("Puente de menta", 51.5f, 77f, 55.7f, 4, 3),
            new("Palacio de helado", 78.5f, 110f, 93f, 0, 0)
        };

        internal const float Min = -11f, Max = 110f, PortalX = 105.5f;
        private static readonly float[] Islands = { 35.1f, 62.4f };

        internal static float Center(float x) => 2.25f * Mathf.Sin((x - 2f) * 0.128f)
            + 1.05f * Mathf.Sin((x - 2f) * 0.226f);
        internal static float HalfWidth(float x) => 4.55f + 0.93f * Mathf.Cos(x * 0.14f - 0.4f);
        internal static Vector3 At(float x, float lane = 0, float y = 0) => new(x, y, Center(x) + lane);

        internal static Vector3 Clamp(Vector3 point, float right = Max, float margin = 0.37f, bool avoidIslands = true)
        {
            float x = Mathf.Clamp(point.x, Min + margin, right - margin);
            float width = HalfWidth(x) - margin;
            float lane = Mathf.Clamp(point.z - Center(x), -width, width);
            if (avoidIslands)
            {
                foreach (float island in Islands)
                {
                    float along = (x - island) / (2.25f + margin);
                    if (Mathf.Abs(along) >= 1f) continue;
                    float side = (1.33f + margin) * Mathf.Sqrt(1 - along * along);
                    if (Mathf.Abs(lane) < side) lane = lane < 0f ? -side : side;
                }
            }
            return At(x, lane, point.y);
        }

        private static readonly Vector2[] Waypoints =
        {
            new(-3.42f, -2.48f), new(3.42f, -2.48f),
            new(3.42f, 2.48f), new(-3.42f, 2.48f)
        };

        internal static Vector3 Steer(Vector3 from, Vector3 target, Vector3 direction)
        {
            foreach (float island in Islands)
            {
                if (Mathf.Min(from.x, target.x) > island + 3.6f || Mathf.Max(from.x, target.x) < island - 3.6f)
                    continue;

                var origin = new Vector2(from.x - island, from.z - Center(from.x));
                var goal = new Vector2(target.x - island, target.z - Center(target.x));
                if (ClearOfIsland(origin, goal)) continue;

                int choice = -1;
                float best = float.PositiveInfinity;
                for (int i = 0; i < Waypoints.Length; i++)
                {
                    float first = Vector2.Distance(origin, Waypoints[i]);
                    if (first < 0.06f || !ClearOfIsland(origin, Waypoints[i])) continue;
                    for (int j = 0; j < Waypoints.Length; j++)
                    {
                        if (!ClearOfIsland(Waypoints[j], goal)) continue;
                        float byEdge = Vector2.Distance(Waypoints[i], Waypoints[j]);
                        if ((i + 2) % 4 == j) byEdge = 11.8f;
                        float cost = first + byEdge + Vector2.Distance(Waypoints[j], goal);
                        if (cost >= best) continue;
                        best = cost;
                        choice = i;
                    }
                }
                if (choice >= 0)
                {
                    var point = Waypoints[choice];
                    return (At(island + point.x, point.y) - from).normalized;
                }
            }
            return direction;
        }

        private static bool ClearOfIsland(Vector2 from, Vector2 to)
        {
            var origin = new Vector2(from.x / 2.72f, from.y / 1.81f);
            var end = new Vector2(to.x / 2.72f, to.y / 1.81f);
            var delta = end - origin;
            if (delta.sqrMagnitude < 0.001f) return true;
            float projection = -Vector2.Dot(origin, delta);
            float nearest = Mathf.Clamp01(projection / delta.sqrMagnitude);
            if (nearest <= 0.001f || nearest >= 0.999f) return true;
            return (origin + delta * nearest).sqrMagnitude >= 1f;
        }
    }

    internal sealed partial class BlockoutWorld
    {
        private Mesh trailMesh;
        public Transform[] DressTrail(out Transform portal)
        {
            var scene = Group("Ruta del Valle de Azúcar", root, Vector3.zero);
            var pastry = new Color32(237, 183, 119, 255);
            var pink = new Color32(236, 152, 177, 255);
            var mint = new Color32(159, 204, 183, 255);
            var berry = new Color32(212, 161, 220, 255);
            Color[] zones = { pink, pastry, mint, berry };
            Color[] scoops = { new Color32(255, 218, 135, 255), new Color32(149, 219, 181, 255),
                new Color32(232, 160, 187, 255), new Color32(192, 165, 223, 255) };

            Box("Paisaje inferior", scene, 48, -1.49f, 4, 180, 0.36f, 88, Blush);
            BuildBiscuitPath(scene);

            for (int i = 0; i < 25; i++)
            {
                float x = -15 + i * 5.6f;
                int zone = Mathf.Clamp((int)((x + 3f) / 27f), 0, 3);
                float lane = IceTrail.HalfWidth(x);
                Ellipsoid("Glaseado de la ribera", scene, IceTrail.At(x, lane + 3.7f, -0.32f),
                    new Vector3(8.7f, 0.72f, 6.85f), zones[zone]);
                Ellipsoid("Glaseado junto al camino", scene, IceTrail.At(x - 1f, -lane - 2.2f, -0.45f),
                    new Vector3(7.2f, 0.65f, 4.04f), Color.Lerp(zones[zone], Blush, 0.34f));

                if (i % 3 != 1)
                    WaferTree(scene, IceTrail.At(x + 1.6f, lane + 4.1f, -0.22f), 0.66f + (i % 3) * 0.16f,
                        scoops[(i + zone) % scoops.Length], pastry, new Color32(206, 143, 99, 255));
                if (i % 3 != 0)
                    WaferTree(scene, IceTrail.At(x - 2f, lane + 11.2f, -0.92f), 0.81f + (i % 3) * 0.20f,
                        Color.Lerp(scoops[(i + zone + 1) % scoops.Length], Frosting, 0.21f), pastry,
                        new Color32(220, 165, 121, 255));
                if (i % 2 == 0) CandyCane(scene, IceTrail.At(x + 0.35f, lane + 0.61f, -0.04f), 1.37f + i % 3 * 0.29f, i % 4 == 0);
                for (int j = 0; j < 4; j++)
                {
                    Ellipsoid("Pincelada de bizcocho", scene, IceTrail.At(x + j * 0.71f,
                        (j % 2 == 0 ? 1f : -1f) * (lane - 0.55f - j * 0.25f), -0.025f),
                        new Vector3(0.74f + j * 0.15f, 0.011f, 0.14f), Color.Lerp(Vanilla, pastry, 0.21f));
                }
            }
            for (int c = 0; c < 12; c++)
            for (int j = 0; j < 3; j++)
                Ellipsoid("Nube de merengue", scene, new Vector3(-21 + c * 14.8f + j * 2.24f,
                    7.2f + 0.87f * Mathf.Sin(c * 1.14f) + Mathf.Sin(j) * 0.6f, 29f + c % 2 * 4.1f),
                    new Vector3(5.12f, 1.76f, 3.1f), Frosting);

            float[] forks = { 35.1f, 62.4f };
            for (int i = 0; i < forks.Length; i++)
            {
                var fork = Group("Isla de galleta", scene, IceTrail.At(forks[i]));
                Ellipsoid("Cerro de bizcocho", fork, Vector3.up * 0.11f, new Vector3(4.24f, 0.54f, 2.44f), pastry);
                Ellipsoid("Cubierta de crema", fork, Vector3.up * 0.37f, new Vector3(3.76f, 0.34f, 1.97f), zones[i + 1]);
                CandyCane(fork, new Vector3(-0.85f, 0.53f, 0.28f), 1.18f, true);
                CandyCane(fork, new Vector3(0.48f, 0.52f, -0.05f), 1.45f, false);
            }

            var barriers = new Transform[IceTrail.Areas.Length - 1];
            for (int i = 0; i < IceTrail.Areas.Length; i++)
            {
                var stage = IceTrail.Areas[i];
                float signX = stage.Entrance + 1.9f;
                Sign(scene, stage.Name.ToUpperInvariant(), IceTrail.At(signX, IceTrail.HalfWidth(signX) - 0.39f, 1.55f),
                    new Vector2(2.92f, 0.46f), Cocoa, Frosting, 0.040f);
                for (int j = 0; j < 3; j++)
                {
                    var arrow = Group("Flecha del sendero", scene, IceTrail.At(stage.Entrance + 2.0f + j * 0.79f, -1.0f, 0.004f));
                    arrow.localRotation = Quaternion.Euler(0, 90, 0);
                    Chevron(arrow, Vector3.zero, 0.42f, Lagoon);
                }

                if (i >= barriers.Length) continue;
                var gate = Group("Paso de caramelo", scene, IceTrail.At(stage.Gate));
                var width = IceTrail.HalfWidth(stage.Gate);
                CandyPillar(gate, Vector3.forward * width, 2.83f, zones[i]);
                CandyPillar(gate, Vector3.back * width, 2.83f, zones[i]);
                var shutter = Group("Reja de galleta", gate, Vector3.zero);
                barriers[i] = shutter;
                for (int j = 0; j < 14; j++)
                {
                    float lane = Mathf.Lerp(-width + 0.24f, width - 0.24f, j / 13f);
                    Rounded("Barrote de caramelo", shutter, new Vector3(0, 1.03f, lane),
                        new Vector3(0.23f, 2.09f, 0.19f), j % 2 == 0 ? Jam : Rose);
                }
                for (int j = 0; j < 2; j++)
                    Rounded("Travesaño", shutter, Vector3.up * (0.55f + j * 1.05f),
                        new Vector3(0.24f, 0.12f, width * 2 - 0.15f), Frosting);
            }

            portal = CandyPortal();
            portal.position = IceTrail.At(IceTrail.PortalX);
            portal.rotation = Quaternion.Euler(0, -31f, 0);
            return barriers;
        }

        private void BuildBiscuitPath(Transform scene)
        {
            const int segments = 122;
            var vertices = new Vector3[(segments + 1) * 2];
            var indices = new int[segments * 6];
            for (int i = 0; i <= segments; i++)
            {
                float x = Mathf.Lerp(IceTrail.Min - 0.5f, IceTrail.Max + 0.5f, i / (float)segments);
                float width = IceTrail.HalfWidth(x) + 0.36f;
                vertices[i * 2] = IceTrail.At(x, -width, -0.033f);
                vertices[i * 2 + 1] = IceTrail.At(x, width, -0.033f);
                if (i == segments) break;
                int v = i * 2;
                int t = i * 6;
                indices[t] = v; indices[t + 1] = v + 1; indices[t + 2] = v + 2;
                indices[t + 3] = v + 1; indices[t + 4] = v + 3; indices[t + 5] = v + 2;
            }
            trailMesh = new Mesh { name = "Sendero de masa tostada", vertices = vertices, triangles = indices };
            trailMesh.RecalculateNormals();
            trailMesh.RecalculateBounds();
            var path = Group("Sendero jugable", scene, Vector3.zero).gameObject;
            path.AddComponent<MeshFilter>().sharedMesh = trailMesh;
            path.AddComponent<MeshCollider>().sharedMesh = trailMesh;
            Paint(path.AddComponent<MeshRenderer>(), Vanilla);
        }
    }
}