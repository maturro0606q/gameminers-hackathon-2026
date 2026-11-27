using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameMiners.Gameplay
{
    internal sealed class ImportedYetiDamage
    {
        private readonly SkinnedMeshRenderer skin;
        private readonly Transform actor;
        private readonly BlockoutWorld world;
        private readonly Mesh original;
        private readonly Transform[] shoulders = new Transform[2], elbows = new Transform[2], hands = new Transform[2];
        private readonly int[] strain = new int[2];
        private readonly bool[] severed = new bool[2];
        private readonly Vector3[] vertices;
        private readonly BoneWeight[] weights;
        private readonly int[][] triangles;
        private readonly sbyte[][] arms;
        private readonly List<Patch> patches = new();
        private readonly Material[] materials;
        private Mesh wounded;
        private Material interior;
        private int torsoHits;

        private readonly struct Patch
        {
            public readonly Vector3 At, Inward;
            public readonly float Radius;
            public Patch(Vector3 at, Vector3 inward, float radius) => (At, Inward, Radius) = (at, inward, radius);
        }

        public int ArmsRemaining => 2 - (severed[0] ? 1 : 0) - (severed[1] ? 1 : 0);
        public int Holes => patches.Count;

        public ImportedYetiDamage(Transform actor, SkinnedMeshRenderer skin, BlockoutWorld world)
        {
            this.actor = actor;
            this.skin = skin;
            this.world = world;
            original = skin.sharedMesh;
            materials = skin.sharedMaterials;
            vertices = original.vertices;
            weights = original.boneWeights;
            triangles = new int[original.subMeshCount][];
            arms = new sbyte[triangles.Length][];
            var bones = skin.bones;
            var limits = new sbyte[bones.Length];
            for (int i = 0; i < bones.Length; i++) limits[i] = -1;
            for (int side = 0; side < 2; side++)
            {
                string suffix = side == 0 ? ".L" : ".R";
                shoulders[side] = CharacterMotion.Find(actor, "UpperArm" + suffix);
                elbows[side] = CharacterMotion.Find(actor, "Forearm" + suffix);
                hands[side] = CharacterMotion.Find(actor, "Hand" + suffix);
                if (shoulders[side] == null) continue;
                for (int i = 0; i < bones.Length; i++)
                    if (bones[i] == shoulders[side] || bones[i].IsChildOf(shoulders[side])) limits[i] = (sbyte)side;
            }

            var left = new float[vertices.Length];
            var right = new float[vertices.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                var w = weights[i];
                void Add(int b, float value)
                {
                    if (limits[b] == 0) left[i] += value;
                    else if (limits[b] == 1) right[i] += value;
                }
                Add(w.boneIndex0, w.weight0); Add(w.boneIndex1, w.weight1);
                Add(w.boneIndex2, w.weight2); Add(w.boneIndex3, w.weight3);
            }
            for (int sub = 0; sub < triangles.Length; sub++)
            {
                var indices = triangles[sub] = original.GetTriangles(sub);
                var flags = arms[sub] = new sbyte[indices.Length / 3];
                for (int i = 0; i < indices.Length; i += 3)
                {
                    int a = indices[i], b = indices[i + 1], c = indices[i + 2];
                    flags[i / 3] = left[a] + left[b] + left[c] >= 1.79f ? (sbyte)0 :
                        right[a] + right[b] + right[c] >= 1.79f ? (sbyte)1 : (sbyte)-1;
                }
            }
        }

        public Vector3 Hit(Vector3 source, Ray? aim, int damage)
        {
            if (shoulders[0] == null || shoulders[1] == null) return actor.position + Vector3.up * 1.5f;
            int side = FindArm(source, aim);
            if (side >= 0)
            {
                Vector3 joint = shoulders[side].position;
                strain[side] += damage;
                if (strain[side] >= 6)
                {
                    Sever(side, joint);
                    float x = side == 0 ? 0.57f : -0.57f;
                    AddPatch(new Vector3(x, 2.25f, 0), new Vector3(-Mathf.Sign(x), 0, 0), 0.30f);
                }
                return Vector3.Lerp(elbows[side].position, hands[side].position, 0.34f);
            }

            var exterior = source - actor.position;
            exterior.y = 0;
            Vector3 center = actor.position + Vector3.up * 1.77f;
            var aimPoint = center;
            if (aim.HasValue)
            {
                var ray = aim.Value;
                float distance = Vector3.Dot(center - ray.origin, ray.direction);
                aimPoint = ray.GetPoint(Mathf.Max(distance, 0f));
            }
            var toward = skin.transform.InverseTransformVector(exterior);
            float sign = toward.z >= 0 ? 1f : -1f;
            var hit = skin.transform.InverseTransformPoint(aimPoint);
            hit.x = Mathf.Clamp(hit.x, -0.40f, 0.40f);
            hit.y = Mathf.Clamp(hit.y, 1.27f, 2.12f);
            hit.z = sign * 0.36f;
            torsoHits++;
            if (damage >= 4 || torsoHits % 2 == 0)
            {
                hit.x = Mathf.Clamp(hit.x + (torsoHits % 2 == 0 ? 0.17f : -0.17f), -0.46f, 0.46f);
                hit.y += (torsoHits % 3 - 1) * 0.17f;
                if (patches.Count < 8) AddPatch(hit, new Vector3(0, 0, -sign), damage >= 4 ? 0.29f : 0.23f);
            }
            return skin.transform.TransformPoint(hit);
        }

        private int FindArm(Vector3 source, Ray? aim)
        {
            if (aim.HasValue)
            {
                float best = float.PositiveInfinity;
                int chosen = -1;
                Vector3 chest = actor.position + Vector3.up * 1.65f;
                if (SphereRay(aim.Value, chest, 0.50f, out var throughTorso)) best = throughTorso + 0.1f;
                float centerEntry = best;
                for (int i = 0; i < 2; i++)
                {
                    if (severed[i]) continue;
                    for (int sample = 0; sample < 4; sample++)
                    {
                        var center = sample < 2 ? Vector3.Lerp(shoulders[i].position, elbows[i].position, (sample + 1) / 2f) :
                            Vector3.Lerp(elbows[i].position, hands[i].position, (sample - 1) / 2f);
                        if (SphereRay(aim.Value, center, 0.285f, out var hit) && hit < best) (chosen, best) = (i, hit);
                    }
                }
                if (chosen >= 0 || centerEntry < float.PositiveInfinity) return chosen;
            }
            var flat = Vector3.ProjectOnPlane(source - actor.position, Vector3.up).normalized;
            float flank = Vector3.Dot(flat, actor.right);
            if (Mathf.Abs(flank) < 0.42f) return -1;
            for (int i = 0; i < 2; i++)
                if (!severed[i] && Vector3.Dot(elbows[i].position - actor.position, actor.right) * flank > 0f) return i;
            return -1;
        }

        private static bool SphereRay(Ray ray, Vector3 center, float radius, out float distance)
        {
            var delta = center - ray.origin;
            float centerDistance = Vector3.Dot(delta, ray.direction);
            float rest = radius * radius - (delta - ray.direction * centerDistance).sqrMagnitude;
            distance = rest >= 0 ? centerDistance - Mathf.Sqrt(rest) : -1f;
            return distance >= 0;
        }

        private void Sever(int side, Vector3 joint)
        {
            // Keep the pose at the instant of impact for the piece that falls away.
            var snapshot = new Mesh();
            skin.BakeMesh(snapshot);
            var shape = new Mesh { name = "Brazo del modelo de Cami", indexFormat = IndexFormat.UInt32 };
            var frameVertices = snapshot.vertices;
            for (int i = 0; i < frameVertices.Length; i++)
                frameVertices[i] = skin.transform.TransformPoint(frameVertices[i]) - joint;
            shape.vertices = frameVertices;
            shape.subMeshCount = materials.Length;
            for (int sub = 0; sub < triangles.Length; sub++)
            {
                var part = new List<int>();
                var source = triangles[sub];
                for (int i = 0; i < source.Length; i += 3)
                {
                    if (arms[sub][i / 3] != side) continue;
                    part.Add(source[i]); part.Add(source[i + 1]); part.Add(source[i + 2]);
                }
                shape.SetTriangles(part, sub, false);
            }
            shape.RecalculateNormals();
            shape.RecalculateBounds();
            Object.Destroy(snapshot);
            severed[side] = true;
            var outward = Vector3.ProjectOnPlane(joint - actor.position, Vector3.up).normalized;
            world.ThrowImportedArm(shape, materials, joint, outward);
        }

        private void AddPatch(Vector3 point, Vector3 inward, float radius)
        {
            if (patches.Count > 0)
            {
                for (int i = 0; i < patches.Count; i++)
                {
                    if ((patches[i].At - point).sqrMagnitude > 0.048f) continue;
                    patches[i] = new Patch(Vector3.Lerp(patches[i].At, point, 0.3f), inward, Mathf.Min(0.40f, patches[i].Radius + 0.055f));
                    Rebuild();
                    return;
                }
            }
            patches.Add(new Patch(point, inward, radius));
            Rebuild();
        }

        private void Rebuild()
        {
            if (wounded == null)
            {
                wounded = new Mesh { name = "Modelo del Yeti con daño", indexFormat = IndexFormat.UInt32 };
                interior = new Material(materials[0]) { color = new Color32(35, 23, 74, 255) };
                var all = new Material[materials.Length + 1];
                materials.CopyTo(all, 0);
                all[materials.Length] = interior;
                skin.sharedMaterials = all;
                var lifetime = skin.gameObject.AddComponent<YetiMeshLifetime>();
                lifetime.Mesh = wounded;
                lifetime.Interior = interior;
            }

            int total = vertices.Length;
            var newVertices = new Vector3[total * 2];
            var newWeights = new BoneWeight[total * 2];
            vertices.CopyTo(newVertices, 0);
            vertices.CopyTo(newVertices, total);
            weights.CopyTo(newWeights, 0);
            weights.CopyTo(newWeights, total);

            var cuts = new List<int>();
            wounded.Clear();
            wounded.vertices = newVertices;
            wounded.boneWeights = newWeights;
            wounded.bindposes = original.bindposes;
            wounded.subMeshCount = triangles.Length + 1;
            var newNormals = new Vector3[total * 2];
            var baseNormals = original.normals;
            baseNormals.CopyTo(newNormals, 0);
            baseNormals.CopyTo(newNormals, total);

            for (int sub = 0; sub < triangles.Length; sub++)
            {
                var kept = new List<int>();
                var source = triangles[sub];
                for (int i = 0; i < source.Length; i += 3)
                {
                    sbyte limb = arms[sub][i / 3];
                    if (limb >= 0 && severed[limb]) continue;
                    int a = source[i], b = source[i + 1], c = source[i + 2];
                    var center = (vertices[a] + vertices[b] + vertices[c]) / 3f;
                    Patch? opening = null;
                    if (limb < 0)
                        foreach (var patch in patches)
                            if ((center - patch.At).sqrMagnitude <= patch.Radius * patch.Radius)
                            { opening = patch; break; }
                    if (opening.HasValue)
                    {
                        var hole = opening.Value;
                        foreach (int vertex in new[] { a, b, c })
                        {
                            float depth = Mathf.Clamp01(1f - Vector3.Distance(vertices[vertex], hole.At) / hole.Radius);
                            newVertices[vertex + total] = vertices[vertex] + hole.Inward * (0.05f + depth * 0.24f);
                            newNormals[vertex + total] = -hole.Inward;
                            cuts.Add(vertex + total);
                        }
                    }
                    else { kept.Add(a); kept.Add(b); kept.Add(c); }
                }
                wounded.SetTriangles(kept, sub, false);
            }
            wounded.vertices = newVertices;
            wounded.normals = newNormals;
            wounded.SetTriangles(cuts, triangles.Length, false);
            wounded.bounds = original.bounds;
            skin.sharedMesh = wounded;
        }
    }

    internal sealed partial class BlockoutWorld
    {
        public void ThrowImportedArm(Mesh mesh, Material[] materials, Vector3 at, Vector3 outward)
        {
            var limb = Group("Brazo desprendido del Yeti", root, at);
            limb.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            limb.gameObject.AddComponent<MeshRenderer>().sharedMaterials = materials;
            limb.gameObject.AddComponent<YetiMeshLifetime>().Mesh = mesh;
            var collider = limb.gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.27f;
            collider.center = new Vector3(0, -0.43f, 0);
            var rigidbody = limb.gameObject.AddComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidbody.AddForce(outward * 4.4f + Vector3.up * 2.3f, ForceMode.VelocityChange);
            rigidbody.AddTorque(new Vector3(-8, 3, 10), ForceMode.VelocityChange);
            Object.Destroy(limb.gameObject, 9f);
            CreamSpray(at, outward, new Color32(144, 198, 249, 255), 26, 4.9f, true);
        }

        public Transform TrophyCrown { get; private set; }

        private bool BuildYetiTrophy(Transform effigy)
        {
            var model = PortableModel.Instantiate("Yeti", effigy, surface);
            if (model == null) return false;
            var body = model.GetComponentInChildren<SkinnedMeshRenderer>();
            if (body == null) { Object.Destroy(model.gameObject); return false; }

            var crown = Group("Corona sobre el trofeo", effigy, Vector3.zero);
            bool mounted = TrophyPart(effigy, body, false);
            if (TrophyPart(crown, body, true))
            {
                // Put the attachment origin at the center of the crown's lower rim.
                var mesh = crown.GetComponent<MeshFilter>().sharedMesh;
                var bounds = mesh.bounds;
                var rim = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
                var vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++) vertices[i] -= rim;
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
                crown.localPosition = rim;
                TrophyCrown = crown;
            }
            else Object.Destroy(crown.gameObject);
            model.gameObject.SetActive(false);
            Object.Destroy(model.gameObject);
            return mounted;
        }

        private static bool TrophyPart(Transform target, SkinnedMeshRenderer body, bool crown)
        {
            var source = body.sharedMesh;
            var vertices = source.vertices;
            var normals = source.normals;
            var materials = body.sharedMaterials;
            var mesh = new Mesh { name = crown ? "Corona del Yeti" : "Trofeo del Yeti de Cami", indexFormat = IndexFormat.UInt32 };
            var indices = new List<int>[source.subMeshCount];
            var compact = new List<Vector3>();
            var compactNormals = new List<Vector3>();
            var originalIds = new Dictionary<int, int>();
            for (int part = 0; part < indices.Length; part++)
            {
                var selected = indices[part] = new List<int>();
                if (crown != IsCrownColor(materials[part].color)) continue;
                var triangles = source.GetTriangles(part);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    if (!crown && (vertices[triangles[i]].y < 2.44f || vertices[triangles[i + 1]].y < 2.44f ||
                        vertices[triangles[i + 2]].y < 2.44f)) continue;
                    for (int c = 0; c < 3; c++)
                    {
                        int id = triangles[i + c];
                        if (!originalIds.TryGetValue(id, out int index))
                        {
                            index = compact.Count;
                            originalIds.Add(id, index);
                            compact.Add((vertices[id] - Vector3.up * 2.67f) * 0.59f);
                            compactNormals.Add(normals[id]);
                        }
                        selected.Add(index);
                    }
                }
            }
            if (compact.Count == 0) { Object.Destroy(mesh); return false; }
            mesh.SetVertices(compact);
            mesh.SetNormals(compactNormals);
            mesh.subMeshCount = indices.Length;
            for (int i = 0; i < indices.Length; i++) mesh.SetTriangles(indices[i], i, false);
            mesh.RecalculateBounds();
            target.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            target.gameObject.AddComponent<MeshRenderer>().sharedMaterials = materials;
            target.gameObject.AddComponent<YetiMeshLifetime>().Mesh = mesh;
            return true;
        }

        private static bool IsCrownColor(Color color) =>
            Near(color, new Color32(99, 212, 238, 255)) || Near(color, new Color32(214, 166, 79, 255)) ||
            Near(color, new Color32(244, 202, 115, 255));

        private static bool Near(Color a, Color b) =>
            Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) < 0.038f;

        public void YetiEruption(Vector3 at, Quaternion rotation)
        {
            var origin = Group("Erupción del Yeti", root, at);
            origin.rotation = rotation;
            var model = PortableModel.Instantiate("YetiSpikes", origin, surface);
            if (model != null) model.GetComponent<PortableRig>()?.Play("Eruption", 1f);
            Object.Destroy(origin.gameObject, 2.3f);
        }
    }
}
