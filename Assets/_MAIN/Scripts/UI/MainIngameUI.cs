using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainIngameUI : MonoBehaviour
{
    public Animator anim;

    public static MainIngameUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }
}
