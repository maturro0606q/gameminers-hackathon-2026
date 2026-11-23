using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameMiners.Gameplay.Editor
{
    internal static class M1Build
    {
        private static readonly string[] Scenes =
        {
            "Assets/Gameplay/Scenes/00_Menu.unity",
            "Assets/Gameplay/Scenes/01_Shop.unity",
            "Assets/Gameplay/Scenes/02_IceWorld.unity",
            "Assets/Gameplay/Scenes/03_JetyRoom.unity"
        };

        [MenuItem("GameMiners/Compilar Heladería Maldita para Windows")]
        public static void Windows()
        {
            const string path = "Builds/Heladeria-Maldita/GameMiners.exe";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = path,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.StrictMode
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"M1: {report.summary.result}, {report.summary.totalErrors} errores.");

            Debug.Log($"GAMEMINERS_WINDOWS_BUILD_OK {Path.GetFullPath(path)} ({Environment.Version})");
        }
    }
}
