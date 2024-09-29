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
    [SerializeField] float turnSpeed;

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
            // If reached target patrol point, change target patrol point
            Vector3 targetPos = new Vector3(patrolPoints[targetPoint].position.x, transform.position.y, 
                patrolPoints[targetPoint].position.z);
            if (transform.position == targetPos && !stopped)
            {
                StartCoroutine(Wait());
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
                    turnSpeed * Time.deltaTime);
            }

            else
            {
                // Smoothly rotate towards the target point
                transform.rotation = Quaternion.Slerp(transform.rotation, patrolPoints[targetPoint].rotation,
                    turnSpeed * Time.deltaTime);
            }
            #endregion
        }
    }

    IEnumerator Wait()
    {
        stopped = true;
        anim.SetBool("isMoving", false);

        yield return new WaitForSeconds(3f);

        stopped = false;
        ChangeTargetPatrolPoint();
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
    }

    public void ResumePatrol()
    {
        patrolInterrupted = false;
        stopped = false;
        anim.SetBool("isMoving", true);
    }
}
