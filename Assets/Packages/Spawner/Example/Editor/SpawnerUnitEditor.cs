///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnerUnitEditor.cs
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
/// Handles the Spawner Unit editor window.
/// </summary>
public class SpawnerUnitEditor : EditorWindow
{
    Spawner spawn;
    Vector2 scrollbar = new Vector2();
    UnitLevels level = UnitLevels.Easy;
    int currentLevel = 0;
    /// <summary>
    /// Initialise the Unit Editor
    /// </summary>
    /// <param name="target">The spawner to associate the editor with</param>
    public static void Initialise(Spawner target)
    {
        SpawnerUnitEditor editor = EditorWindow.GetWindow<SpawnerUnitEditor>(true, "Spawner Unit Editor", true);
        editor.spawn = target;
    }

    void OnGUI()
    {
        if (spawn.unitList[currentLevel] == null)
        {
            spawn.unitList[currentLevel] = new SpawnableClass();
            spawn.unitList[currentLevel].level = level;
        }

        GUILayout.Label("Units for: ");
        level = (UnitLevels)EditorGUILayout.EnumPopup(level);
        if (GUILayout.Button("Add Unit"))
        {
            Resize(ref spawn.unitList[currentLevel].units, 1);
        }
        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);
        {
            if (spawn.unitList[currentLevel].units.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Units");
                EditorGUILayout.EndHorizontal();
                for (int x = 0; x < spawn.unitList[currentLevel].units.Length; x++)
                {
                    EditorGUILayout.BeginHorizontal();
                    spawn.unitList[currentLevel].units[x] = (GameObject)EditorGUILayout.ObjectField(spawn.unitList[currentLevel].units[x], typeof(GameObject), false);
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        RemoveAt(ref spawn.unitList[currentLevel].units, x);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("No units set for current unit level.");
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        if (GUI.changed)
        {
            currentLevel = (int)level;
            EditorUtility.SetDirty(spawn);
        }
    }

    private void RemoveAt(ref GameObject[] array, int pos)
    {
        GameObject[] currentArray = array;
        array = new GameObject[currentArray.Length - 1];
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

    private void Resize(ref GameObject[] array, int amount = 1)
    {
        GameObject[] currentArray = array;
        array = new GameObject[currentArray.Length + amount];

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