using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;


[CustomEditor(typeof(RewinderHitboxGroup))]
public class RewinderHitboxGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RewinderHitboxGroup group = target as RewinderHitboxGroup;

        if (group != null)
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Find Colliders"))
            {
                group.ProximityHitbox =
                    new RewinderHitbox
                    {
                        Collider = group.GetComponent<Collider>(),
                        Type = RewinderHitboxType.Proximity
                    };

                if (group.ProximityHitbox.Collider == null)
                {
                    Debug.LogWarning("Could not find any proximity collider, hit detection (but not proximity detection) will still work but be slower.");
                }

                if (!(group.ProximityHitbox.Collider is BoxCollider) && !(group.ProximityHitbox.Collider is SphereCollider))
                {
                    Debug.LogWarning("The found proximity collider is not a Box or Sphere collider, hit detection (but not proximity detection) will still work but be slower.");
                    group.ProximityHitbox.Collider = null;
                }

                group.BodyHitboxes =
                    group
                        .GetComponentsInChildren<Collider>()
                        .Where(c => c.GetComponent<RewinderHitboxGroup>() == null)
                        .Where(c => c is BoxCollider || c is SphereCollider)
                        .Select(c => new RewinderHitbox { Collider = c, Type = RewinderHitboxType.Chest })
                        .ToArray();

                if (group.BodyHitboxes.Length == 0)
                {
                    Debug.LogWarning("Didn't find any hitboxes on '" + group.transform.name + "', hit detection will not work");
                }
                else
                {
                    for (int i = 0; i < group.BodyHitboxes.Length; ++i)
                    {
                        string n = group.BodyHitboxes[i].Collider.transform.name.ToLower();

                        if (n.Contains("head") || n.Contains("neck"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Head;
                        }

                        if (n.Contains("chest") || n.Contains("pelvis") || n.Contains("body") || n.Contains("torso") || n.Contains("spine"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Chest;
                        }

                        if (n.Contains("upper arm") || n.Contains("upperarm") || n.Contains("arm") || n.Contains("collarbone") || n.Contains("bicep"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Arm;
                        }

                        if (n.Contains("fore arm") || n.Contains("forearm") || n.Contains("lower arm"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Forearm;
                        }

                        if (n.Contains("hand") || n.Contains("fingers") || n.Contains("finger") || n.Contains("knuckle"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Hand;
                        }

                        if (n.Contains("thigh") || n.Contains("leg") || n.Contains("upper leg") || n.Contains("legupper") || n.Contains("leg upper"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Thigh;
                        }

                        if (n.Contains("calf") || n.Contains("lower leg") || n.Contains("leg calf") || n.Contains("legcalf"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Calf;
                        }

                        if (n.Contains("feet") || n.Contains("foot") || n.Contains("toe"))
                        {
                            group.BodyHitboxes[i].Type = RewinderHitboxType.Foot;
                        }
                    }
                }

                EditorUtility.SetDirty(group);
            }
        }
    }
}
