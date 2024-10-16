using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElectronicType { Roomba, Camera, Humanoid };
[RequireComponent(typeof(PlayerTPPDirection))]
public class ControllableElectronic : MonoBehaviour
{
    [Header("Public variables")]
    public CinemachineFreeLook objCamera;
    public ElectronicType thisElectronicType;
    public Animator animator;
    public Outline outline;
    public GameObject canvas;
    //public PlayerMovement playerMovement;

    public bool isOnline = true;
}
