///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnerWaveEditor.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 22 Nov 2012
//
// Copyright (c) 2012 Garth de Wet
// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using CorruptedSmileStudio.Spawner;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Handles the Spawner Wave editor window.
/// </summary>
public class SpawnerWaveEditor : EditorWindow
{
    public Spawner spawn;
    Vector2 scrollbar = new Vector2();
    /// <summary>
    /// Initialises the Wave Editor window
    /// </summary>
    /// <param name="target">The spawner the editor window is associated with</param>
    public static void Initialise(Spawner target)
    {
        SpawnerWaveEditor editor = EditorWindow.GetWindow<SpawnerWaveEditor>(true, "Spawner Wave Editor", true);
        editor.spawn = target;
        if (editor.spawn.spawnLocations == null)
            editor.spawn.spawnLocations = new Transform[0];
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(150));
            {
                GUILayout.Label("Unit Level");
                GUILayout.Label("Number of Units");
                GUILayout.Label("Spawn Type");
                GUILayout.Label("Time Between Spawn");
                switch (spawn.spawnType)
                {
                    case SpawnModes.TimedWave:
                    case SpawnModes.TimeSplitWave:
                        GUILayout.Label("Wave Timer");
                        goto case SpawnModes.Wave;
                    case SpawnModes.Wave:
                        GUILayout.Label("Number of Waves");
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(80));
            {
                spawn.unitLevel = (UnitLevels)EditorGUILayout.EnumPopup(spawn.unitLevel);
                spawn.totalUnitsToSpawn = EditorGUILayout.IntField(spawn.totalUnitsToSpawn);
                spawn.spawnType = (SpawnModes)EditorGUILayout.EnumPopup(spawn.spawnType);
                spawn.timeBetweenSpawns = EditorGUILayout.FloatField(spawn.timeBetweenSpawns);

                switch (spawn.spawnType)
                {
                    case SpawnModes.TimedWave:
                    case SpawnModes.TimeSplitWave:
                        spawn.waveTimer = EditorGUILayout.FloatField(spawn.waveTimer);
                        goto case SpawnModes.Wave;
                    case SpawnModes.Wave:
                        spawn.totalWavesToSpawn = EditorGUILayout.IntField(spawn.totalWavesToSpawn);
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Spawn Locations");
        if (GUILayout.Button("Add Spawn Location"))
        {
            Resize(ref spawn.spawnLocations, 1);
        }
        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);
        {
            if (spawn.spawnLocations.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Spawn Location");
                EditorGUILayout.EndHorizontal();
                for (int x = 0; x < spawn.spawnLocations.Length; x++)
                {
                    EditorGUILayout.BeginHorizontal();
                    spawn.spawnLocations[x] = (Transform)EditorGUILayout.ObjectField(spawn.spawnLocations[x], typeof(Transform), true);
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        RemoveAt(ref spawn.spawnLocations, x);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("No spawn locations have been specified.\nUnits will spawn at Spawner GameObject location.");
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
            EditorUtility.SetDirty(spawn);
    }

    private void RemoveAt(ref Transform[] array, int pos)
    {
        Transform[] currentArray = array;
        array = new Transform[currentArray.Length - 1];
        bool posFound = false;

        for (int i = 0; i < currentArray.Length; i++)
        {
            if (i != pos)
            {
                if (!posFound)
                    array[i] = currentArray[i];
                else
                    array[i - 1] = currentArray[i];
            }
            else
                posFound = true;
        }
    }

    private void Resize(ref Transform[] array, int amount = 1)
    {
        Transform[] currentArray = array;
        array = new Transform[currentArray.Length + amount];

        for (int i = 0; i < currentArray.Length; i++)
        {
            array[i] = currentArray[i];
        }
        for (int i = currentArray.Length; i < currentArray.Length + amount; i++)
        {
            array[i] = null;
        }
    }
}