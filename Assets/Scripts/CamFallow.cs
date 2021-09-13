using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFallow : MonoBehaviour
{
    public Transform sphere;
    private Vector3 offset;

    private void Start()
    {
        offset = sphere.position + transform.position;
    }

    void LateUpdate()
    {
        transform.position = sphere.position + offset;
    }
}