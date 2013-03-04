using UnityEngine;
using System.Collections;

public class DemoWalker : MonoBehaviour
{
    int index = 0;

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    Vector3[] positions = new Vector3[2];

    void Start()
    {
        transform.position = positions[0];
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, positions[index]) < 0.1f)
        {
            index = (index + 1) % positions.Length;
        }

        Vector3 v = (positions[index] - transform.position).normalized;
        transform.position += v * speed * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v), Time.deltaTime / 0.1f);
    }
}