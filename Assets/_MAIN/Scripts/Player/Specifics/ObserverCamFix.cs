using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverCamFix : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook cam;
    public float xValueModifier;

    private void Start()
    {
        xValueModifier = transform.eulerAngles.y;

        cam.m_XAxis.Value = xValueModifier + 90;
        cam.m_XAxis.m_MaxValue = xValueModifier + 200;
        cam.m_XAxis.m_MinValue = xValueModifier - 20;

        enabled = false;
    }
}
