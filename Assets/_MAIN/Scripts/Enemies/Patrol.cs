using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Header("References")]
    public ChaosBot chaosBotScript;
    public FieldOfView fov;
    [SerializeField] Animator anim;

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

        anim.SetBool("isMoving", true);
    }

    // Update is called once per frame
    void Update()
    {
        // If reached target patrol point, change target patrol point
        Vector3 targetPos = new Vector3(patrolPoints[targetPoint].position.x, transform.position.y, 
            patrolPoints[targetPoint].position.z);
        if (transform.position == targetPos && !stopped)
        {
            StartCoroutine(IncreaseTargetInt());
        }

        // Move towards target patrol point
        Vector3 moveTowardsPos = new Vector3(patrolPoints[targetPoint].position.x,
            transform.position.y, patrolPoints[targetPoint].position.z);
        transform.position = Vector3.MoveTowards(transform.position, moveTowardsPos,
            speed * Time.deltaTime);

        #region rotation 
        // Looks at target transform point
        if (!stopped & transform.position != moveTowardsPos)
        {
            var targetRotation = Quaternion.LookRotation(moveTowardsPos - transform.position);

            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                rotationSpeed * Time.deltaTime);
        }

        else
        {
            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, patrolPoints[targetPoint].rotation,
                rotationSpeed * Time.deltaTime);
        }
        #endregion
    }

    IEnumerator IncreaseTargetInt()
    {
        stopped = true;
        anim.SetBool("isMoving", false);

        yield return new WaitForSeconds(3f);

        stopped = false;
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
        anim.SetBool("isMoving", true);
    }
}
