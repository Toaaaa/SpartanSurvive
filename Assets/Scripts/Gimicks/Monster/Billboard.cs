using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;
    private void Start()
    {
        cam = Camera.main.transform;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.rotation*Vector3.forward, cam.rotation*Vector3.up);
        transform.Rotate(0, 180, 0);
    }
}
