using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack };
    public EnemyState currentState;
    public GameObject player;
    private Transform playerTrans;
    public float recoverTime = 3f;
    public float chaseDistance = 10f;
    public float attackDistance = 2f;
    public GameObject hitbox;

    private NavMeshAgent agent;
    private Billboard billboard;
    private float patrolDistance = 5f;
    private Animator anim;
    private int health = 100;
    private UnityAction<int,GameObject> DamageListener;

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
        DamageListener = new UnityAction<int,GameObject>(ApplyDamage);// Delegate points to function that handles event
        EventManager.StartListening("Damager", DamageListener); //Like and subscribe to an event
    }

    private void OnDisable()
    {
        EventManager.StopListening("Damager", DamageListener);
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        billboard = GetComponent<Billboard>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("player");

        ChangeState(EnemyState.Patrol);
    }


    //Triggers when the player changes state
    void ChangeState(EnemyState newState)
    {
        //Debug.Log("New State: " + newState);
        currentState = newState;
        StopAllCoroutines();
        playerTrans = player.transform;

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

    //Patroling when player is not yet spotted
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

    //Chase the player when player is in range
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

    //Attack state
    IEnumerator AI_Attack()
    {
        float elapsedTime = recoverTime + 1;
        billboard.enabled = true;
        anim.SetBool("IsAttacking", true);

        while (true)
        {
            if (elapsedTime > recoverTime)
            {
                anim.SetBool("IsAttacking", true);
                elapsedTime = 0f; //Just attacked, reset count

                Invoke("EnableHitbox",0.5f);
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

    void EnableHitbox()
    {
        hitbox.SetActive(true);
        Invoke("DisableHitbox", 0.5f);
    }

    void DisableHitbox()
    {
        hitbox.SetActive(false);
    }

    //Applying damage to self
    public void ApplyDamage(int amt, GameObject obj)
    {
        if (obj == gameObject)
        {
            Health = amt; //Subtracting from health due to setter
            Debug.Log("Enemy Damaged, Health" + health);
        }
    }
}
