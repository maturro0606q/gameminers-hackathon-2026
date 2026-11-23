using System.Collections.Generic;
using UnityEngine;

namespace GameMiners.Gameplay
{
    internal sealed class YetiScars
    {
        private readonly Transform actor;
        private readonly Transform[] arms = new Transform[2];
        private readonly int[] harm = new int[2];
        private readonly HoleSurface torso, chest;
        private readonly BlockoutWorld world;
        private int torsoHits;

        public int ArmsRemaining { get; private set; } = 2;
        public int OpenHoles { get; private set; }

        public YetiScars(Transform actor, BlockoutWorld world)
        {
            this.actor = actor;
            this.world = world;
            int arm = 0;
            foreach (var mesh in actor.GetComponentsInChildren<MeshFilter>())
            {
                if (mesh.name == "Brazo" && arm < arms.Length) arms[arm++] = mesh.transform;
                else if (mesh.name == "Torso") torso = new HoleSurface(mesh);
                else if (mesh.name == "Pecho blanco") chest = new HoleSurface(mesh);
            }
            ArmsRemaining = arm;
        }

        public Vector3 Hit(Vector3 source, Ray? aim, int damage)
        {
            int arm = SelectArm(source, aim);
            if (arm >= 0)
            {
                var limb = arms[arm];
                var joint = limb.position;
                harm[arm] += damage;
                if (harm[arm] >= 6)
                {
                    Vector3 peel = (joint - actor.position).normalized;
                    world.DetachYetiArm(limb, joint, peel);
                    arms[arm] = null;
                    ArmsRemaining--;
                    if (torso?.Carve(peel, 4) == true) OpenHoles++;
                }
                return joint + (source - joint).normalized * 0.28f;
            }

            if (torso == null) return actor.position + Vector3.up * 1.4f;
            torsoHits++;
            var center = torso.Part.position;
            var outwards = source - center;
            outwards.y = 0.12f;
            if (aim.HasValue && SphereHit(torso.Part, aim.Value, out float distance))
                outwards = aim.Value.GetPoint(distance) - center;
            var normal = outwards.normalized;
            if (normal.sqrMagnitude < 0.5f) normal = actor.forward;
            if (damage >= 4 || torsoHits % 2 == 0)
            {
                Vector3 spot = Quaternion.AngleAxis(OpenHoles % 2 == 0 ? -24 : 24, Vector3.up) * normal;
                spot = (spot + Vector3.up * (OpenHoles % 3 * 0.16f - 0.13f)).normalized;
                if (OpenHoles < 7 && torso.Carve(spot, damage))
                {
                    chest?.Carve(spot, damage);
                    OpenHoles++;
                }
            }
            var local = torso.Part.InverseTransformVector(normal).normalized * 0.51f;
            return torso.Part.TransformPoint(local);
        }

        private int SelectArm(Vector3 source, Ray? aim)
        {
            if (aim.HasValue)
            {
                float nearest = float.PositiveInfinity;
                int selected = -1;
                if (torso != null && SphereHit(torso.Part, aim.Value, out float torsoRange)) nearest = torsoRange + 0.15f;
                for (int i = 0; i < arms.Length; i++)
                    if (arms[i] != null && SphereHit(arms[i], aim.Value, out float hit) && hit < nearest)
                        (selected, nearest) = (i, hit);
                if (selected >= 0) return selected;
                if (nearest < float.PositiveInfinity) return -1;
            }

            Vector3 offset = source - actor.position;
            offset.y = 0;
            if (offset.sqrMagnitude < 0.08f) return -1;
            float flank = Vector3.Dot(offset.normalized, actor.right);
            if (Mathf.Abs(flank) < 0.41f) return -1;
            for (int i = 0; i < arms.Length; i++)
                if (arms[i] != null && Vector3.Dot(arms[i].position - actor.position, actor.right) * flank > 0) return i;
            return -1;
        }

        private static bool SphereHit(Transform part, Ray ray, out float hit)
        {
            var start = part.InverseTransformPoint(ray.origin);
            var rayDir = part.InverseTransformVector(ray.direction);
            float a = rayDir.sqrMagnitude;
            float b = Vector3.Dot(start, rayDir);
            float d = b * b - a * (start.sqrMagnitude - 0.25f);
            hit = d >= 0f ? (-b - Mathf.Sqrt(d)) / a : -1f;
            return hit >= 0f;
        }

        private sealed class HoleSurface
        {
            private readonly MeshFilter filter;
            private readonly Mesh intact;
            private readonly List<(Vector3 direction, float threshold)> holes = new();
            private Mesh wounded;
            private Material interior;

            public Transform Part => filter.transform;

            public HoleSurface(MeshFilter filter)
            {
                this.filter = filter;
                intact = filter.sharedMesh;
            }

            public bool Carve(Vector3 worldDirection, int damage)
            {
                if (!intact.isReadable) return false;
                var face = Part.InverseTransformVector(worldDirection).normalized;
                float threshold = damage >= 4 ? 0.90f : 0.935f;
                if (holes.Exists(hole => Vector3.Dot(face, hole.direction) > 0.975f && hole.threshold <= threshold)) return false;
                holes.Add((face, threshold));

                if (wounded == null)
                {
                    wounded = Object.Instantiate(intact);
                    wounded.name = "Malla recortada por golpes";
                    filter.sharedMesh = wounded;
                    var renderer = filter.GetComponent<MeshRenderer>();
                    var outer = renderer.sharedMaterial;
                    interior = new Material(outer) { color = new Color32(56, 37, 88, 255) };
                    renderer.sharedMaterials = new[] { outer, interior };
                    var cleanup = filter.gameObject.AddComponent<YetiMeshLifetime>();
                    cleanup.Mesh = wounded;
                    cleanup.Interior = interior;
                }

                var intactVertices = intact.vertices;
                int count = intactVertices.Length;
                var vertices = new Vector3[count * 2];
                var uvs = new Vector2[count * 2];
                var intactUvs = intact.uv;
                for (int i = 0; i < count; i++)
                {
                    var vertex = intactVertices[i];
                    vertices[i] = vertex;
                    float deep = 0f;
                    foreach (var hole in holes)
                        deep = Mathf.Max(deep, Mathf.InverseLerp(hole.threshold - 0.029f, 1f,
                            Vector3.Dot(vertex.normalized, hole.direction)));
                    vertices[i + count] = vertex * (1f - deep * 0.36f);
                    if (i < intactUvs.Length) uvs[i] = uvs[i + count] = intactUvs[i];
                }
                var triangles = intact.triangles;
                var visible = new List<int>(triangles.Length);
                var cut = new List<int>();
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
                    Vector3 center = (vertices[a] + vertices[b] + vertices[c]).normalized;
                    bool carve = false;
                    foreach (var hole in holes)
                        if (Vector3.Dot(center, hole.direction) > hole.threshold) { carve = true; break; }
                    if (carve) { cut.Add(a + count); cut.Add(b + count); cut.Add(c + count); }
                    else { visible.Add(a); visible.Add(b); visible.Add(c); }
                }
                wounded.Clear();
                wounded.vertices = vertices;
                wounded.uv = uvs;
                wounded.subMeshCount = 2;
                wounded.SetTriangles(visible, 0, false);
                wounded.SetTriangles(cut, 1, false);
                wounded.RecalculateNormals();
                wounded.RecalculateBounds();
                return true;
            }
        }
    }

    internal sealed class YetiMeshLifetime : MonoBehaviour
    {
        public Mesh Mesh;
        public Material Interior;
        private void OnDestroy()
        {
            Destroy(Mesh);
            Destroy(Interior);
        }
    }

    internal sealed partial class BlockoutWorld
    {
        public void DetachYetiArm(Transform limb, Vector3 joint, Vector3 axis)
        {
            limb.SetParent(root, true);
            limb.name = "Brazo desprendido del Yeti";
            var body = limb.gameObject.AddComponent<Rigidbody>();
            body.mass = 0.7f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            var collider = limb.gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.32f;
            body.AddForce(axis * 4.2f + Vector3.up * 3.1f, ForceMode.VelocityChange);
            body.AddTorque(new Vector3(11f, 4f, 15f), ForceMode.VelocityChange);
            UnityEngine.Object.Destroy(limb.gameObject, 9f);
            CreamSpray(joint, axis, new Color32(144, 198, 249, 255), 32, 5.5f, true);
        }
    }
}