using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPointTransform : MonoBehaviour
{

    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        Camera.main.transform.position = transform.position;
    }
}
