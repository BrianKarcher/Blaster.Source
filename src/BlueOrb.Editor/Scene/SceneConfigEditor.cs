using BlueOrb.Base.Config;
using BlueOrb.Controller.Manager;
using BlueOrb.Controller.Scene;
using BlueOrb.Editor.GUIStyles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlueOrb.Editor.Skill
{
    [CustomEditor(typeof(SceneConfig), true)]
    public class SceneConfigEditor : ConfigEditorBase<SceneConfig>
    {
        private bool _showSpawnPoints = false;

        [MenuItem("Assets/Create/Blue Orb/Scene Config")]
        public static void CreateNewAsset()
        {
            var newAsset = CreateAsset("NewScene.asset");
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(agent.SceneName);
            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (GUILayout.Button("Add/Enable in Build Settings"))
            {
                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                var editorBuildScene = editorBuildSettingsScenes.FirstOrDefault(i => i.path == agent.SceneName);
                if (editorBuildScene == null)
                {
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(agent.SceneName, true));

                }
                else
                {
                    editorBuildScene.enabled = true;
                }
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            }

            if (GUILayout.Button("Disable in Build Settings"))
            {
                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                var editorBuildScene = editorBuildSettingsScenes.FirstOrDefault(i => i.path == agent.SceneName);
                if (editorBuildScene != null)
                {
                    editorBuildScene.enabled = false;
                }
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            }

            agent.SceneName = EditorGUILayout.TextField("Scene Name", agent.SceneName ?? string.Empty);

            _showSpawnPoints = GUILayout.Toggle(_showSpawnPoints, "Spawn Points", "Button");

            var spawnPoints = agent.SpawnPoints;

            if (_showSpawnPoints)
            {
                if (GUILayout.Button("Add Spawn Point", GUILayout.Width(130)))
                {
                    var newSpawnPoint = new SpawnPointConfig();
                    newSpawnPoint.UniqueId = Guid.NewGuid().ToString();
                    spawnPoints.Add(newSpawnPoint);
                }

                int removeSpawnpointIndex = -1;

                for (int i = 0; i < spawnPoints.Count; i++)
                {
                    var spawnPoint = spawnPoints[i];
                    GUILayout.Space(4.0f);
                    GUILayout.BeginVertical(Styles.CreateInspectorStyle());
                    EditorGUILayout.BeginHorizontal();
                    spawnPoint.Name = EditorGUILayout.TextField("Name", spawnPoint.Name);
                    if (GUILayout.Button("", Styles.CreateTilemapDeleteItemStyle()))
                    {
                        if (EditorUtility.DisplayDialog("Remove spawn point?", "Remove spawn point " + spawnPoint.Name + "?", "Yes", "No"))
                        {
                            removeSpawnpointIndex = i;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("Unique Id", spawnPoint.UniqueId);
                    spawnPoint.IsInitialSpawnPoint = EditorGUILayout.Toggle("Initial Spawn Point", spawnPoint.IsInitialSpawnPoint);
                    spawnPoint.ExtraInfo = EditorGUILayout.TextField("Extra Info", spawnPoint.ExtraInfo);

                    GUILayout.EndVertical();
                }

                if (removeSpawnpointIndex != -1)
                    spawnPoints.RemoveAt(removeSpawnpointIndex);
            }

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                agent.SceneName = newPath;
            }

            if (GUI.changed)
            {
                Dirty(false);
            }
        }
    }
}
