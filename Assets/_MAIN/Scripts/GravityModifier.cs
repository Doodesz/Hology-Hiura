using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour
{
    [Header("Gravity")]
    public ConstantForce gravity;
    public float gravityMultiplier = 1;
    [Range(0f,1f)] public float decreaseGravMultiplier;
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
            gravityMultiplier = initGravMult * decreaseGravMultiplier;
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

    private void OnDisable()
    {
        gravity.enabled = false;
    }

    private void OnEnable()
    {
        if (gravity != null)
            gravity.enabled = true;
    }
}
