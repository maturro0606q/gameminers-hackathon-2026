using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameMiners.Gameplay
{
    internal sealed class PortableModel
    {
        private static readonly Dictionary<string, PortableModel> Cache = new();
        private readonly Bone[] bones;
        private readonly Part[] parts;
        private readonly Dictionary<string, Clip> clips = new();

        internal sealed class Rig
        {
            private readonly PortableModel model;
            private readonly Transform[] joints;
            private Clip active;
            private float started, speed;

            public Rig(PortableModel model, Transform[] joints) => (this.model, this.joints) = (model, joints);

            public bool Play(string name, float rate)
            {
                if (!model.clips.TryGetValue(name, out var clip)) return false;
                active = clip;
                started = Time.time;
                speed = rate;
                Sample();
                return true;
            }

            public void Sample()
            {
                if (active == null) return;
                float elapsed = (Time.time - started) * speed;
                elapsed = active.Loop ? elapsed % active.Duration : Mathf.Min(elapsed, active.Duration);
                float frame = elapsed * (active.Frames.Length - 1) / active.Duration;
                int a = Mathf.FloorToInt(frame), b = Mathf.Min(a + 1, active.Frames.Length - 1);
                float blend = frame - a;
                for (int i = 0; i < joints.Length; i++)
                {
                    var first = active.Frames[a][i];
                    var next = active.Frames[b][i];
                    joints[i].SetLocalPositionAndRotation(Vector3.Lerp(first.Position, next.Position, blend),
                        Quaternion.Slerp(first.Rotation, next.Rotation, blend));
                    joints[i].localScale = Vector3.Lerp(first.Scale, next.Scale, blend);
                }
            }
        }

        private readonly struct Pose
        {
            public readonly Vector3 Position, Scale;
            public readonly Quaternion Rotation;
            public Pose(BinaryReader reader)
            {
                Position = ReadVector(reader);
                Rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Scale = ReadVector(reader);
            }
        }

        private readonly struct Bone
        {
            public readonly string Name;
            public readonly int Parent;
            public readonly Pose Base;
            public Bone(BinaryReader reader) => (Name, Parent, Base) = (ReadString(reader), reader.ReadInt32(), new Pose(reader));
        }

        private sealed class Clip
        {
            public readonly bool Loop;
            public readonly float Duration;
            public readonly Pose[][] Frames;
            public Clip(BinaryReader reader, string name, int bones)
            {
                Loop = name == "Idle" || name == "Walk" || name == "Run" || name == "Talk" || name == "Cheer" || name == "ThroneIdle";
                Duration = reader.ReadSingle();
                Frames = new Pose[reader.ReadInt32()][];
                for (int f = 0; f < Frames.Length; f++)
                {
                    Frames[f] = new Pose[bones];
                    for (int i = 0; i < bones; i++) Frames[f][i] = new Pose(reader);
                }
            }
        }

        private sealed class Part
        {
            public readonly Mesh Mesh;
            public readonly Material[] Materials;
            public Part(BinaryReader reader, Material surface, int bones)
            {
                Mesh = new Mesh { name = ReadString(reader), indexFormat = IndexFormat.UInt32 };
                Materials = new Material[reader.ReadInt32()];
                for (int i = 0; i < Materials.Length; i++)
                {
                    var color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    var material = new Material(surface) { color = color };
                    if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
                    if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0.21f);
                    Materials[i] = material;
                }
                var triangles = new List<int>[Materials.Length];
                for (int i = 0; i < triangles.Length; i++) triangles[i] = new List<int>();
                int length = reader.ReadInt32() * 3;
                var vertices = new Vector3[length];
                var normals = new Vector3[length];
                var weights = bones > 0 ? new BoneWeight[length] : null;
                for (int i = 0; i < length;)
                {
                    var section = triangles[reader.ReadInt32()];
                    for (int corner = 0; corner < 3; corner++, i++)
                    {
                        section.Add(i);
                        vertices[i] = ReadVector(reader);
                        normals[i] = ReadVector(reader);
                        if (weights == null) continue;
                        weights[i] = new BoneWeight
                        {
                            boneIndex0 = reader.ReadInt32(), boneIndex1 = reader.ReadInt32(),
                            boneIndex2 = reader.ReadInt32(), boneIndex3 = reader.ReadInt32(),
                            weight0 = reader.ReadSingle(), weight1 = reader.ReadSingle(),
                            weight2 = reader.ReadSingle(), weight3 = reader.ReadSingle()
                        };
                    }
                }
                Mesh.vertices = vertices;
                Mesh.normals = normals;
                Mesh.subMeshCount = Materials.Length;
                for (int i = 0; i < triangles.Length; i++) Mesh.SetTriangles(triangles[i], i, false);
                if (weights != null) Mesh.boneWeights = weights;
                Mesh.RecalculateBounds();
            }
        }

        private PortableModel(BinaryReader reader, Material surface)
        {
            if (reader.ReadInt32() != 0x31414D47) throw new InvalidDataException("Invalid GameMiners model");
            bones = new Bone[reader.ReadInt32()];
            for (int i = 0; i < bones.Length; i++) bones[i] = new Bone(reader);
            parts = new Part[reader.ReadInt32()];
            for (int i = 0; i < parts.Length; i++) parts[i] = new Part(reader, surface, bones.Length);
            int clipCount = reader.ReadInt32();
            for (int i = 0; i < clipCount; i++)
            {
                string name = ReadString(reader);
                clips[name] = new Clip(reader, name, bones.Length);
            }
        }

        internal static string PathFor(string filename) =>
            Path.Combine(Application.streamingAssetsPath, "GameMiners", filename);

        public static Transform Instantiate(string name, Transform parent, Material surface)
        {
            if (!Cache.TryGetValue(name, out var model))
            {
                string path = PathFor(name + ".gma");
                if (!File.Exists(path)) return null;
                using var file = new BinaryReader(File.OpenRead(path));
                model = new PortableModel(file, surface);
                Cache.Add(name, model);
            }
            return model.Mount(name, parent);
        }

        private Transform Mount(string name, Transform parent)
        {
            var root = new GameObject(name).transform;
            root.SetParent(parent, false);
            var joints = new Transform[bones.Length];
            var bindPoses = new Matrix4x4[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                var bone = bones[i];
                var joint = joints[i] = new GameObject(bone.Name).transform;
                joint.SetParent(bone.Parent >= 0 ? joints[bone.Parent] : root, false);
                joint.SetLocalPositionAndRotation(bone.Base.Position, bone.Base.Rotation);
                joint.localScale = bone.Base.Scale;
                bindPoses[i] = joint.worldToLocalMatrix * root.localToWorldMatrix;
            }
            foreach (var part in parts)
            {
                var obj = new GameObject(part.Mesh.name);
                obj.transform.SetParent(root, false);
                if (joints.Length > 0)
                {
                    var renderer = obj.AddComponent<SkinnedMeshRenderer>();
                    if (part.Mesh.bindposeCount == 0) part.Mesh.bindposes = bindPoses;
                    renderer.sharedMesh = part.Mesh;
                    renderer.sharedMaterials = part.Materials;
                    renderer.bones = joints;
                    renderer.rootBone = root;
                    renderer.localBounds = part.Mesh.bounds;
                    renderer.updateWhenOffscreen = true;
                }
                else
                {
                    obj.AddComponent<MeshFilter>().sharedMesh = part.Mesh;
                    obj.AddComponent<MeshRenderer>().sharedMaterials = part.Materials;
                }
            }
            if (joints.Length > 0) root.gameObject.AddComponent<PortableRig>().Initialize(new Rig(this, joints));
            return root;
        }

        private static Vector3 ReadVector(BinaryReader reader) =>
            new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        private static string ReadString(BinaryReader reader) =>
            System.Text.Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
    }

    internal sealed class PortableRig : MonoBehaviour
    {
        private PortableModel.Rig rig;
        public void Initialize(PortableModel.Rig value) => rig = value;
        public bool Play(string state, float speed) => rig.Play(state, speed);
        private void LateUpdate() => rig.Sample();
    }
}