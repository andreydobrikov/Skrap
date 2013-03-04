using UnityEngine;
using System.Collections;

public class ClearSight : MonoBehaviour
{
    public float distanceToClear = 5.0f;

    void Update()
    {
        RaycastHit[] hits;

        hits = Physics.RaycastAll(transform.position, transform.forward, distanceToClear);

        foreach(RaycastHit hit in hits)
        {
            Renderer R = hit.collider.renderer;
            if (R == null)
                continue; // no renderer attached? go to next hit

            AutoTransparent AT = R.GetComponent<AutoTransparent>();
            if (AT == null) // if no script is attached, attach one
            {
                AT = R.gameObject.AddComponent<AutoTransparent>();
            }
            AT.BeTransparent(); // get called every frame to reset the falloff
        }
    }
}