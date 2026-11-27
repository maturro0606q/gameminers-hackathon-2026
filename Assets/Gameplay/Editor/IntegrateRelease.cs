using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameMiners.Gameplay.Editor
{
    internal static class IntegrateRelease
    {
        private const string Art = "Assets/Art/Characters/";
        private const string Gameplay = "Assets/Gameplay/";
        private static readonly string[] SceneNames = { "00_Menu", "01_Shop", "02_IceWorld", "03_JetyRoom" };
        private static readonly string[] UiNames = { "MainMenu", "PlayerHud", "InteractionPrompt", "BossHealthBar", "DefeatPanel", "EndingPanel" };

        public static void Build()
        {
            AssetDatabase.Refresh();
            const string heroPath = Art + "Protagonista/Protagonista.fbx";
            const string icePath = Art + "HeladoChico/HeladoChico.fbx";
            var hero = Controller("Protagonista", heroPath);
            var ice = Controller("HeladoChico", icePath);
            for (int i = 0; i < SceneNames.Length; i++)
            {
                var scene = EditorSceneManager.OpenScene(Gameplay + "Scenes/" + SceneNames[i] + ".unity");
                var state = new SerializedObject(UnityEngine.Object.FindFirstObjectByType<GameFlow>());
                Assign(state, "protagonistModel", heroPath);
                Assign(state, "smallIceCreamModel", icePath);
                Assign(state, "largeIceCreamModel", icePath);
                Assign(state, "iceCreamBossModel", icePath);
                Assign(state, "rabbitModel", Art + "ConejoDialogo/ConejoDialogo.prefab");
                Assign(state, "truckModel", Gameplay + "Props/Camion.fbx");
                state.FindProperty("heroMotion").objectReferenceValue = hero;
                state.FindProperty("iceMotion").objectReferenceValue = ice;
                state.FindProperty("sceneMusic").objectReferenceValue = null;
                var prefabs = state.FindProperty("uiPrefabs");
                prefabs.arraySize = UiNames.Length;
                for (int j = 0; j < UiNames.Length; j++)
                    prefabs.GetArrayElementAtIndex(j).objectReferenceValue = Require("Assets/UI/Prefabs/" + UiNames[j] + ".prefab");
                state.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();
            PlayerSettings.productName = "La Heladería Maldita";
            M1Build.Windows();
        }

        private static RuntimeAnimatorController Controller(string name, string path)
        {
            string destination = Gameplay + "Props/" + name + ".controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(destination);
            if (controller == null)
                controller = AnimatorController.CreateAnimatorControllerAtPath(destination);
            var machine = controller.layers[0].stateMachine;
            int count = 0;
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (asset is not AnimationClip clip || clip.name.StartsWith("__preview__")) continue;
                AnimatorState state = null;
                foreach (var entry in machine.states)
                    if (entry.state.name == clip.name) state = entry.state;
                if (state == null) state = machine.AddState(clip.name, new Vector3(280 + count % 3 * 225, 50 + count / 3 * 72));
                state.motion = clip;
                if (clip.name == "Idle") machine.defaultState = state;
                count++;
            }
            if (count == 0 || machine.defaultState == null) throw new InvalidDataException("No animation clips in " + path);
            Debug.Log($"GAMEMINERS_ASSET {name} clips={count}");
            return controller;
        }

        private static void Assign(SerializedObject state, string field, string path) =>
            state.FindProperty(field).objectReferenceValue = Require(path);

        private static UnityEngine.Object Require(string path)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            return asset != null ? asset : throw new FileNotFoundException("Missing integration asset", path);
        }
    }
}
