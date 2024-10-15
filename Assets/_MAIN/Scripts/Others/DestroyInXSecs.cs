using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInXSecs : MonoBehaviour
{
    [SerializeField] float timeUntilDestroyed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(timeUntilDestroyed);
        Destroy(gameObject);
    }
}
