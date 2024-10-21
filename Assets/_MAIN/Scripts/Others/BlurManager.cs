using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurManager : MonoBehaviour
{
    VolumeProfile volumeProfile;
    DepthOfField dof;

    public static BlurManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        volumeProfile = GetComponent<Volume>().profile;
        if (!volumeProfile.TryGet(out dof)) throw new System.NullReferenceException(nameof(dof));
        
        dof.focusDistance.Override(20f);
    }

    public void BlurCamera()
    {
        dof.focusDistance.Override(0.01f);
    }

    public void UnblurCamera()
    {
        dof.focusDistance.Override(20f);
    }
}
