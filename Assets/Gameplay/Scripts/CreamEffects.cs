using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameMiners.Gameplay
{
    internal sealed partial class BlockoutWorld
    {
        private readonly List<CreamDrop> creamDrops = new();
        private readonly Queue<Transform> puddles = new();
        private readonly Stack<Transform> idleDrops = new();
        private Mesh stainMesh, dropletMesh;

        public int CreamMarkCount => puddles.Count;

        private sealed class CreamDrop
        {
            public Transform Body;
            public Vector3 Speed;
            public Color Cream;
            public float Size, Age;
            public bool Marks;
        }

        public void CreamSpray(Vector3 at, Vector3 axis, Color cream, int count, float force, bool leavesMarks)
        {
            if (dropletMesh == null)
            {
                var source = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                dropletMesh = source.GetComponent<MeshFilter>().sharedMesh;
                UnityEngine.Object.Destroy(source);
            }

            for (int i = 0; i < count && creamDrops.Count < 185; i++)
            {
                float size = Random.Range(0.061f, leavesMarks ? 0.156f : 0.125f);
                var body = idleDrops.Count > 0 ? idleDrops.Pop() : SpriteObject("Gota de crema", dropletMesh);
                body.gameObject.SetActive(true);
                body.position = at + Random.insideUnitSphere * 0.11f;
                body.localScale = new Vector3(size, size * 0.84f, size * 1.48f);
                Color hue = CreamTone(cream, i % 5);
                Paint(body.GetComponent<Renderer>(), hue);
                var drift = new Vector3(Random.Range(-0.95f, 0.95f), Random.Range(0.28f, 0.96f),
                    Random.Range(-0.95f, 0.95f));
                creamDrops.Add(new CreamDrop { Body = body, Size = size, Cream = cream, Marks = leavesMarks,
                    Speed = (axis * 0.56f + drift) * force });
            }
        }

        public void AdvanceCream(float dt)
        {
            for (int i = creamDrops.Count - 1; i >= 0; i--)
            {
                var piece = creamDrops[i];
                piece.Speed += Vector3.down * (16f * dt);
                piece.Body.position += piece.Speed * dt;
                piece.Age += dt;
                if (piece.Body.position.y > 0.009f && piece.Age < 1.55f)
                {
                    if (piece.Speed.sqrMagnitude > 0.05f) piece.Body.rotation = Quaternion.LookRotation(piece.Speed);
                    continue;
                }
                if (piece.Marks && (i % 3 == 0 || piece.Size > 0.131f))
                    CreamStain(piece.Body.position, piece.Cream, piece.Size * Random.Range(1.5f, 2.75f));
                piece.Body.gameObject.SetActive(false);
                idleDrops.Push(piece.Body);
                creamDrops.RemoveAt(i);
            }
        }

        public void CreamStain(Vector3 at, Color cream, float radius)
        {
            if (trailMesh != null && (at.x < IceTrail.Min - 0.45f || at.x > IceTrail.Max + 0.45f ||
                Mathf.Abs(at.z - IceTrail.Center(at.x)) > IceTrail.HalfWidth(at.x) + 0.27f)) return;
            if (puddles.Count >= 105)
            {
                var old = puddles.Dequeue();
                if (old != null) UnityEngine.Object.Destroy(old.gameObject);
            }

            if (stainMesh == null) stainMesh = MakeStain();
            var mark = SpriteObject("Mancha de crema", stainMesh);
            mark.position = new Vector3(at.x, 0.007f + (puddles.Count % 8) * 0.0006f, at.z);
            mark.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            mark.localScale = new Vector3(radius, 1, radius * Random.Range(0.70f, 1.13f));
            Paint(mark.GetComponent<Renderer>(), CreamTone(cream, Random.Range(0, 4)));
            puddles.Enqueue(mark);
        }

        private Transform SpriteObject(string title, Mesh mesh)
        {
            var node = Group(title, root, Vector3.zero);
            node.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = node.gameObject.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            return node;
        }

        private static Color CreamTone(Color color, int shade) =>
            shade == 4 ? Color.Lerp(color, Color.white, 0.19f) :
                Color.Lerp(color, new Color32(85, 36, 61, 255), shade * 0.07f);

        private static Mesh MakeStain()
        {
            const int edges = 23;
            var vertices = new Vector3[edges + 1];
            var triangles = new int[edges * 3];
            for (int i = 0; i < edges; i++)
            {
                float angle = 2f * Mathf.PI * i / edges;
                float shape = 0.83f + Mathf.Sin(angle * 3f + 0.7f) * 0.12f + Mathf.Cos(angle * 7f) * 0.15f;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * shape, 0, Mathf.Sin(angle) * shape);
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = (i + 1) % edges + 1;
                triangles[i * 3 + 2] = i + 1;
            }
            var mesh = new Mesh { name = "Gota de helado extendida", vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals();
            return mesh;
        }

        public Transform SugarGem(Vector3 at, Color color)
        {
            var gem = Shape(PrimitiveType.Cube, "Cristal de azúcar", root, at, Vector3.one * 0.21f, color, false);
            gem.transform.rotation = Quaternion.Euler(35, 0, 45);
            return gem.transform;
        }

        public Transform CursorPip() => Ellipsoid("Marca de puntería", root, Vector3.zero,
            new Vector3(0.18f, 0.025f, 0.18f), new Color32(252, 247, 226, 255)).transform;
    }

    internal sealed class CreamSurface
    {
        private readonly List<(Renderer renderer, int slot)> channels = new();
        private readonly MaterialPropertyBlock paint = new();
        private Color last;

        public CreamSurface(Transform root)
        {
            foreach (var renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];
                    if (material == null) continue;
                    var tint = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") : material.color;
                    if (material.name.Contains("GM_Ice_Vanilla") ||
                        Mathf.Abs(tint.r - 0.9647f) + Mathf.Abs(tint.g - 0.8588f) + Mathf.Abs(tint.b - 0.62745f) < 0.035f)
                        channels.Add((renderer, i));
                }
            }
        }

        public void Set(Color tint)
        {
            if (tint == last) return;
            last = tint;
            foreach (var (renderer, slot) in channels)
            {
                renderer.GetPropertyBlock(paint, slot);
                paint.SetColor("_Color", tint);
                paint.SetColor("_BaseColor", tint);
                renderer.SetPropertyBlock(paint, slot);
            }
        }
    }
}