///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnerInspector.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 22 Nov 2012
//
// Copyright (c) 2012 Garth de Wet
// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_4_0
namespace CorruptedSmileStudio.Spawner
{
#else
using CorruptedSmileStudio.Spawner;
#endif
using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom editor for the spawner class. Makes it easier to see how things interact.
/// </summary>
[CustomEditor(typeof(Spawner))]
public class SpawnerInspector : Editor
{
    /// <summary>
    /// The spawner being edited
    /// </summary>
    Spawner spawn;

    /// <summary>
    /// Performs the custom Inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        spawn = (Spawner)target;

        if (GUILayout.Button("Unit Editor"))
        {
            SpawnerUnitEditor.Initialise(spawn);
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Wave Editor"))
        {
            SpawnerWaveEditor.Initialise(spawn);
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Spawner ID");
            spawn.spawnID = EditorGUILayout.IntField(spawn.spawnID);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Spawn");
            spawn.spawn = EditorGUILayout.Toggle(spawn.spawn);
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
#if UNITY_4_0
}
#endif