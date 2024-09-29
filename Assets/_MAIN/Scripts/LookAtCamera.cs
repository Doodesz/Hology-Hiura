using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.gameObject;
        canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);
    }
}
