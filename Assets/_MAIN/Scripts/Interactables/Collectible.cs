using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] string thisCollectibeKey;
    [SerializeField] GameObject onCollectedPrefab;

    private void Start()
    {
        if (PlayerPrefs.GetInt(thisCollectibeKey, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer == 7 || other.gameObject.layer == 17)
            && other.TryGetComponent<ControllableElectronic>(out ControllableElectronic otherScript)
            && otherScript.gameObject == PlayerController.Instance.currPlayerObj)
        {
            PlayerPrefs.SetInt(thisCollectibeKey, 1);
            Instantiate(onCollectedPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
