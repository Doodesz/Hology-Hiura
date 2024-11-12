using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject contentObj;

    [Header("Variables")]
    [SerializeField] float speed;
    [SerializeField] string mainMenuSceneName;

    [Header("Debugging")]
    [SerializeField] bool pauseAutoScroll;
    [SerializeField] float pauseTimeout;

    private void Start()
    {
        contentObj.transform.position = new Vector3(contentObj.transform.position.x, 0, transform.position.z);
    }

    private void Update()
    {
        if (!pauseAutoScroll)
            contentObj.transform.Translate(Vector3.up * Time.deltaTime * speed);
        else if (pauseTimeout > 1.5f)
            pauseAutoScroll = false;

        if (Input.GetMouseButtonDown(0))
        {
            pauseAutoScroll = true;
            pauseTimeout = 0;
        }

        pauseTimeout += Time.deltaTime;
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
