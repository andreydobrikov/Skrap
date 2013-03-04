#define REWINDER_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class DemoDraw : MonoBehaviour
{
    int selected = 0;
    float time = 0;
    float behind = 0;
    float accuracy = 0f;
    Vector3 drawPosition;
    Dictionary<RewinderHitbox, GameObject> debugDict = new Dictionary<RewinderHitbox, GameObject>();
    GameObject sphereExample;
    RewinderHitboxHit hit;

    void Start()
    {
        sphereExample = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereExample.active = false;
    }

    void OnGUI()
    {
        behind = (float)System.Math.Round(GUI.HorizontalSlider(new Rect(10, 10, Screen.width - 20, 50), behind, 0, 4), 2);
        string behind_str = "Rewinding: " + behind.ToString() + "s";
        Vector2 valueSize = new GUIStyle("label").CalcSize(new GUIContent(behind_str));

        GUI.Label(new Rect(10, 30, 100, 50), "0");
        GUI.Label(new Rect(Screen.width - 17, 30, 10, 50), "4");
        GUI.Label(new Rect(Screen.width / 2 - valueSize.x / 2, 30, valueSize.x, 50), behind_str);

        int width = (Screen.width - 30) / 2;
        int height = Screen.height - 35;

        GUI.backgroundColor = selected == 0 ? Color.green : Color.gray;
        if (GUI.Button(new Rect(10, height, width, 25), "Raycast"))
        {
            selected = 0;
        }

        GUI.backgroundColor = selected == 1 ? Color.green : Color.gray;
        if (GUI.Button(new Rect(20 + width, height, width, 25), "Overlap Sphere"))
        {
            selected = 1;
        }

        if (hit.Group != null)
        {
            GUI.Label(new Rect(10, Screen.height / 2 - 10, 500, 20), "Hit: " + hit.Hitbox.Type + " (took: " + ToLongString(time) + "s)");
        }
        else
        {
            GUI.Label(new Rect(10, Screen.height / 2 - 10, 500, 20), "Miss");
        }

        GUI.Label(new Rect(10, Screen.height / 2 + 10, 500, 20), "Accuracy: " + accuracy + "s");
    }

    void Update()
    {
        if (Application.isPlaying && RewinderSnapshot.Count > 0)
        {
            RewinderSnapshot snapshot = RewinderSnapshot.Find(Time.time - behind);

            if (snapshot != null)
            {
                accuracy = (float)System.Math.Round(Mathf.Abs((Time.time - behind) - snapshot.Time), 3);
                RewinderHitboxGroupSnapshot group = snapshot.Groups.First();

                // On first time, setup debug display

#if REWINDER_DEBUG
                if (debugDict.Count == 0)
                {
                    foreach (RewinderHitbox hitbox in group.Group.BodyHitboxes)
                    {
                        switch (hitbox.ColliderType)
                        {
                            case RewinderColliderType.Sphere:
                                {
                                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                    go.renderer.material = Resources.Load("RewinderDebug", typeof(Material)) as Material;
                                    go.transform.localScale = hitbox.Collider.bounds.size;
                                    debugDict.Add(hitbox, go);
                                }
                                break;

                            case RewinderColliderType.Box:
                                {
                                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    go.transform.localScale = hitbox.Bounds.size;
                                    go.renderer.material = Resources.Load("RewinderDebug", typeof(Material)) as Material;
                                    debugDict.Add(hitbox, go);
                                }
                                break;
                        }
                    }
                }

                // Move debug display
                
                for (int i = 0; i < group.HitboxMatrices_Debug.Length; ++i)
                {
                    Matrix4x4 m = group.HitboxMatrices_Debug[i];
                    RewinderHitbox hitbox = group.Group.BodyHitboxes[i];
                    GameObject go = debugDict[hitbox];
                    go.transform.position = m.MultiplyPoint(Vector3.zero);
                    go.transform.rotation = group.HitboxRotations_Debug[i];
                }
#endif

                // Do collision detection (if any)

                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit rHit;
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    System.Diagnostics.Stopwatch sw = null;

                    switch (selected)
                    {
                        case 0:
                            {
                                List<RewinderHitboxHit> hits;

                                // Time how long the raycast takes
                                sw = System.Diagnostics.Stopwatch.StartNew();
                                RewinderSnapshot.Raycast(behind, r.origin, r.direction, out hits);
                                sw.Stop();

                                hit = hits.FirstOrDefault();
                                RewinderSnapshot.Recycle(hits);
                                sphereExample.active = false;
                            }
                            break;

                        case 1:
                            if (Physics.Raycast(r, out rHit, 1024f, 1 << 8))
                            {
                                drawPosition = rHit.point;
                                List<RewinderHitboxHit> hits;

                                // Time how long the overlap takes
                                sw = System.Diagnostics.Stopwatch.StartNew();
                                RewinderSnapshot.OverlapSphere(behind, drawPosition, 0.5f, out hits);
                                sw.Stop();

                                hit = hits.FirstOrDefault();
                                RewinderSnapshot.Recycle(hits);
                                sphereExample.transform.position = drawPosition;
                                sphereExample.active = true;
                            }
                            break;
                    }

                    sw.Stop();
                    time = (float)((double)sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency);

                }
            }
        }
    }

    static string ToLongString(double input)
    {
        input = System.Math.Round(input, 8);
        string str = input.ToString().ToUpper();

        // if string representation was collapsed from scientific notation, just return it:
        if (!str.Contains("E")) return str;

        bool negativeNumber = false;

        if (str[0] == '-')
        {
            str = str.Remove(0, 1);
            negativeNumber = true;
        }

        string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        char decSeparator = sep.ToCharArray()[0];

        string[] exponentParts = str.Split('E');
        string[] decimalParts = exponentParts[0].Split(decSeparator);

        // fix missing decimal point:
        if (decimalParts.Length == 1) decimalParts = new string[] { exponentParts[0], "0" };

        int exponentValue = int.Parse(exponentParts[1]);

        string newNumber = decimalParts[0] + decimalParts[1];

        string result;

        if (exponentValue > 0)
        {
            result =
                newNumber +
                GetZeros(exponentValue - decimalParts[1].Length);
        }
        else // negative exponent
        {
            result =
                "0" +
                decSeparator +
                GetZeros(exponentValue + decimalParts[0].Length) +
                newNumber;

            result = result.TrimEnd('0');
        }

        if (negativeNumber)
            result = "-" + result;

        return result;
    }

    static string GetZeros(int zeroCount)
    {
        if (zeroCount < 0)
            zeroCount = Math.Abs(zeroCount);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < zeroCount; i++) sb.Append("0");

        return sb.ToString();
    }
}