using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosbotLaser : MonoBehaviour
{
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        
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
            script.systemsOfflineUI.SetActive(true);
            script.electronicScript.isOnline = false;
            script.anim.SetBool("isOnline", false);
        }

        Destroy(gameObject);
    }
}