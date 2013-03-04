using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RewinderSnapshotCreator))]
public class RewinderSnapshotCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RewinderSnapshotCreator creator = target as RewinderSnapshotCreator;

        if (creator != null)
        {
            creator.FrameType = (RewinderSnapshotCreator.FrameTypes)EditorGUILayout.EnumPopup("Snapshot on ", creator.FrameType);
            creator.SnapshotEveryNthFrame = EditorGUILayout.IntField("Snapshot interval", creator.SnapshotEveryNthFrame);
            creator.RaiseEveryNthSnapshot = EditorGUILayout.IntField("Event interval", creator.RaiseEveryNthSnapshot);
            creator.MaxSnapshots = RewinderSnapshot.MaxSnapshots = EditorGUILayout.IntSlider("Max snapshots", creator.MaxSnapshots, 2, 2048);

            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Snapshots in memory", RewinderSnapshot.Count.ToString());
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(creator);
        }
    }
}
