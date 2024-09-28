using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Header("References")]
    public ChaosBot chaosBotScript;
    public FieldOfView fov;
    [SerializeField] GameObject modelObj;

    [Header("Variables")]
    public Transform[] patrolPoints;
    public int targetPoint;
    [SerializeField] float rotationSpeed = 0.2f;

    [Header("Debugging")]
    [SerializeField] float speed;
    [SerializeField] bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = chaosBotScript.speed;
        targetPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // If reached target patrol point, change target patrol point
        Vector3 targetPos = new Vector3(patrolPoints[targetPoint].position.x, transform.position.y, 
            patrolPoints[targetPoint].position.z);
        if (transform.position == targetPos && !stopped)
            StartCoroutine(IncreaseTargetInt());

        // Move towards target patrol point
        Vector3 moveTowardsPos = new Vector3(patrolPoints[targetPoint].position.x,
            transform.position.y, patrolPoints[targetPoint].position.z);
        transform.position = Vector3.MoveTowards(transform.position, moveTowardsPos,
            speed * Time.deltaTime);


    }

    private void LateUpdate()
    {
        // Looks at target transform point
        if (transform.position != patrolPoints[targetPoint].position && !stopped)
        {
            Vector3 moveTowardsPos = new Vector3(patrolPoints[targetPoint].position.x,
                transform.position.y, patrolPoints[targetPoint].position.z);

            var targetRotation = Quaternion.LookRotation(moveTowardsPos - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                rotationSpeed * Time.deltaTime);
        }        
    }

    IEnumerator IncreaseTargetInt()
    {
        stopped = true;

        yield return new WaitForSeconds(3f);

        stopped = false;
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }
}
