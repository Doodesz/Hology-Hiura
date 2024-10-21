using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurManager : MonoBehaviour
{
    //[SerializeField] Volume volume;
    [SerializeField] DepthOfField dof;

    public static BlurManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //volume = GetComponent<Volume>();
        dof = GetComponent<DepthOfField>();

        dof.focusDistance = new MinFloatParameter(20f, 0.1f, false);
    }

    public void BlurCamera()
    {
        dof.focusDistance = new MinFloatParameter(20f, 0.1f, false);
    }

    public void UnblurCamera()
    {
        dof.focusDistance = new MinFloatParameter(0.01f, 0.1f, false);
    }
}
