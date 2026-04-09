using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.XR;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack };
    public EnemyState currentState;
    public Transform playerTrans;
    public float recoverTime = 3f;
    public float chaseDistance = 10f;
    public float attackDistance = 2f;

    private NavMeshAgent agent;
    private Billboard billboard;
    private float patrolDistance = 5f;
    private Animator anim;
    private int health = 100;
    private int damageAmount = 20;
    private UnityAction<int> DamageListener;

    public int Health
    {
        get { return health; }
        set
        {
            health -= value;
            Debug.Log("Health: " + health);
            if (health <= 0)
            {
                anim.SetBool("IsDead", true);
                billboard.enabled = false;
                StopAllCoroutines();
                
            }
        }
    }

    private void OnEnable()
    {
        DamageListener = new UnityAction<int>(ApplyDamage);// Delegate points to function that handles event
        EventManager.StartListening("EnemyDamager", DamageListener); //Like and subscribe to an event
    }

    private void OnDisable()
    {
        EventManager.StopListening("EnemyDamager", DamageListener);
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        billboard = GetComponent<Billboard>();
        anim = GetComponent<Animator>();

        ChangeState(EnemyState.Patrol);
    }

    void ChangeState(EnemyState newState)
    {
        Debug.Log("New State: " + newState);
        currentState = newState;
        StopAllCoroutines();

        switch (currentState)
        {
            case EnemyState.Patrol:
                StartCoroutine("AI_Patrol");
                break;
            case EnemyState.Chase:
                StartCoroutine("AI_Chase");
                break;
            case EnemyState.Attack:
                StartCoroutine("AI_Attack");
                break;
            default:
                StartCoroutine("AI_Patrol");
                break;
        }
    }

    IEnumerator AI_Patrol()
    {
        billboard.enabled = false;
        anim.SetBool("IsPatrolling", true);
        while (true)
        {
            Vector3 randomPosition = patrolDistance * Random.insideUnitSphere;
            NavMeshHit hit;
            //Finding on our map
            NavMesh.SamplePosition(transform.position + randomPosition, out hit, 10f, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            //Debug.Log(hit.position);

            //Is the player close enough to chase?
            if(Vector3.Distance(playerTrans.position, transform.position) < chaseDistance)
            {
                ChangeState(EnemyState.Chase);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator AI_Chase()
    {
        billboard.enabled = true;
        //anim.SetBool("IsPatrolling", true);
        anim.SetBool("IsChasing", true);
        while (true)
        {
            agent.SetDestination(playerTrans.position);

            if (Vector3.Distance(playerTrans.position, transform.position) < attackDistance)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (Vector3.Distance(playerTrans.position, transform.position) > chaseDistance)
            {
                anim.SetBool("IsChasing", false);
                ChangeState(EnemyState.Patrol);
            }
                yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator AI_Attack()
    {
        float elapsedTime = recoverTime + 1;
        billboard.enabled = true;
        //anim.SetBool("IsPatrolling", true);
        //anim.SetBool("IsChasing", true);
        anim.SetBool("IsAttacking", true);

        while (true)
        {
            if (elapsedTime > recoverTime)
            {
                anim.SetBool("IsAttacking", true);
                elapsedTime = 0f; //Just attacked, reset count

                //Below is spaghetti code
                //playerTrans.gameObject.GetComponent<PlayerController>().PlayerDamage(10);

                EventManager.TriggerEvent("PlayerDamager", damageAmount);
            }
            else
            {
                anim.SetBool("IsAttacking", false);
            }
            elapsedTime++;

            //deal damage

            if (Vector3.Distance(playerTrans.position, transform.position) > attackDistance)
            {
                ChangeState(EnemyState.Chase);
                anim.SetBool("IsAttacking", false);
            }
                yield return new WaitForSeconds(1f);
        }
    }

    public void ApplyDamage(int amt)
    {
        Health = amt; //Subtracting from health due to setter
        Debug.Log("Enemy Damaged");
    }
}
