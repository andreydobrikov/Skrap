using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelCreator : EditorWindow
{
	GameObject objectToBePlaced;

	[MenuItem("Window/LevelCreator")]
	static void Init()
	{
		LevelCreator window = (LevelCreator)EditorWindow.GetWindow(typeof(LevelCreator));
		SceneView.onSceneGUIDelegate += window.OnScene;
	}

	void OnGUI()
	{
		if (objectToBePlaced != null)
		{
			GUILayout.Label("Place your object in the scene view."
				+ "\n Your object will be place at your mouse click. The mouse click has to hit the floor/terrain of the level (Or another object)"
				+ "\n Right click in the Scene view to cancel.");
		}
		else
		{
			if (GUILayout.Button("Place Spawn Area"))
			{
				objectToBePlaced = (GameObject)Resources.Load("LevelCreation/SpawnArea");
			}
		}
	}

	void OnScene(SceneView sceneView)
	{
		if (objectToBePlaced != null)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				if (Event.current.button == 0)
				{
					Event.current.Use();
					RaycastHit hit;
					if (Physics.Raycast(sceneView.camera.ScreenPointToRay(Event.current.mousePosition), out hit))
					{
						GameObject clone = (GameObject)Instantiate(objectToBePlaced, hit.transform.position, Quaternion.identity);
						Selection.activeObject = clone;
						EditorGUIUtility.PingObject(clone);
						objectToBePlaced = null;
					}
				}
				else if (Event.current.button == 1)
				{
					Event.current.Use();
					objectToBePlaced = null;
				}
			}
		}
	}

	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= this.OnScene;
	}
}
