using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 60f;

    private void Start()
    {
        transform.localEulerAngles = new Vector3(-90, 0, 0);

    }

    private void Update()
    {
        transform.Rotate(Vector3.forward *rotationSpeed * Time.deltaTime);
    }
}
