using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosbotLaser : MonoBehaviour
{
    [SerializeField] float lifetime;
    [SerializeField] float speed;
    [SerializeField] GameObject postImpactLaserObj;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyInSeconds());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerMovement>(out PlayerMovement script))
        {
            script.DisableMovement();
            script.gameObject.GetComponent<RepairElectronic>().InitializeDisabledBehaviour();
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Instantiate(postImpactLaserObj, transform.position, transform.rotation);
    }

    IEnumerator DestroyInSeconds()
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
    }
}
