using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    [Header("References")]
    public ChaosBot chaosBotScript;
    public FieldOfView fov;
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent ai;

    [Header("Variables")]
    public Transform[] patrolPoints;
    public int targetPoint;
    [SerializeField] float turnSpeed;
    [SerializeField] float stationaryDuration;

    [Header("Debugging")]
    [SerializeField] float speed;
    [SerializeField] bool stopped = false;
    public bool patrolInterrupted = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = chaosBotScript.speed;
        turnSpeed = chaosBotScript.turnSpeed;
        targetPoint = 0;

        anim.SetBool("isMoving", true);
    }

    // Update is called once per frame
    void Update()
    {
        // When patrolling
        if (!patrolInterrupted)
        {
            if (ai.isStopped)
                ai.isStopped = false;

            // If reached target patrol point, change target patrol point
            Vector3 targetPos = new Vector3(patrolPoints[targetPoint].position.x, transform.position.y,
                patrolPoints[targetPoint].position.z);
            if (transform.position == targetPos && !stopped)
            {
                StartCoroutine(Wait());
            }

            // Move towards target patrol point
            /*Vector3 moveTowardsPos = new Vector3(patrolPoints[targetPoint].position.x,
                transform.position.y, patrolPoints[targetPoint].position.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTowardsPos,
                speed * Time.deltaTime);*/
            ai.SetDestination(targetPos);

            #region rotation 
            // Looks at target transform point
            /*if (!stopped & transform.position != moveTowardsPos)
            {
                var targetRotation = Quaternion.LookRotation(moveTowardsPos - transform.position);

                // Smoothly rotate towards the target point
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    turnSpeed * Time.deltaTime);
            }*/

            if (stopped & transform.position == targetPos)
            {
                // Smoothly rotate towards the target point
                transform.rotation = Quaternion.Slerp(transform.rotation, patrolPoints[targetPoint].rotation,
                    turnSpeed * Time.deltaTime);
            }
            #endregion
        }
        else
        {
            if (!ai.isStopped)
                ai.isStopped = true;
        }
    }

    IEnumerator Wait()
    {
        stopped = true;
        anim.SetBool("isMoving", false);

        yield return new WaitForSeconds(stationaryDuration);

        stopped = false;
        ChangeTargetPatrolPoint();
        
        if (!ai.isStopped)
        anim.SetBool("isMoving", true);
    }

    void ChangeTargetPatrolPoint()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    public void InterruptPatrol()
    {
        patrolInterrupted = true;
        stopped = true;
        anim.SetBool("isMoving", false);
        StopAllCoroutines();
    }

    public void ResumePatrol()
    {
        patrolInterrupted = false;
        stopped = false;
        StartCoroutine(Wait());
    }
}
