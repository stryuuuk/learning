using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 5f;
    private Transform rotate;
    void Start()
    {
        rotate = GetComponent<Transform>();
    }

    void Update()
    {
        rotate.Rotate(0, speed * Time.deltaTime, 0);
    }
}
