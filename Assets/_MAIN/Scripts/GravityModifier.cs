using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour
{
    [Header("Gravity")]
    public ConstantForce gravity;
    [SerializeField] float gravityMultiplier = 1;
    [SerializeField] float decreaseGravMultiplier = 2;
    float initGravMult;

    [Header("Layer Reference")]
    [SerializeField] int layerToReduceGrav = 10;

    // Start is called before the first frame update
    void Start()
    {
        gravity = gameObject.AddComponent<ConstantForce>();
        initGravMult = gravityMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        //gravity.force = new Vector3(0.0f, -9.87f * gravityMultiplier, 0.0f);
        gravity.force = new Vector3(0.0f, -9.87f * gravityMultiplier, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(collision.gameObject.layer);
        if (other.gameObject.layer == layerToReduceGrav)
        {
            gravityMultiplier = initGravMult / decreaseGravMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(collision.gameObject.layer);
        if (other.gameObject.layer == layerToReduceGrav)
        {
            gravityMultiplier = initGravMult;
        }        
    }
}