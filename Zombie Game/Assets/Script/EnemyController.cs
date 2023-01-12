using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Move Settings")]    
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float patrolRadius = 6f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float searchSpeed = 3.5f;

    [Header("Attack Settings")]
    [SerializeField] int damage = 2;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float attackRange = 2f;

    private bool isSearched = false;
    private bool isAttacking = false;

    private Animator anim;
    private NavMeshAgent agent;
    private Transform player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    enum State
    {
        Idle,
        Search,
        Chase,
        Attack
    }

    [SerializeField] private State currentState = State.Idle;

    void Start()
    {
        
    }

    
    void Update()
    {
        StateCheck();
        StateExecute();
    }

    private void OnDrawGizmos()   //Bu editör çalýþtýðý vakit sürekli çalýþýr.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        switch (currentState)
        {           
            case State.Search:
                Gizmos.color = Color.blue;
                Vector3 targetPos = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                Gizmos.DrawLine(transform.position, targetPos);
                break;

            case State.Chase:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, player.position);
                break;

            case State.Attack:
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
                break;        
        }
    } 


    private void StateCheck()
    {
        float distanceToTarget = Vector3.Distance(player.position, transform.position);

        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            currentState = State.Chase;
        }

        else if (distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
        }

        else
        {
            currentState = State.Search;
        }
    }

    private void StateExecute()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Search:
                if (!isSearched && agent.remainingDistance <= 0.1f || !agent.hasPath && !isSearched)   //remainingdistance gittiði nokta ile þuanki nokta arasýndaki mesafeyi dönen bir komuttur. 0.1 in altýna düþünce fark çalýþacak.
                {                                                                                      //Veya gidecek bir yerimiz yoksa çalýþacak.!agent.hasPath.
                    Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                    agent.enabled = false;
                    transform.position = agentTarget;   //Aralarda kaçaklar olmasýn diye ulaþmak istediði yere çok az kala ulaþmak istediði yerin oraya ýþýnlanýr(gözle görülmez). Ve tekrar arama yapýlýr.
                    agent.enabled = true;

                    Invoke("Search", patrolWaitTime);   //Search metodunu çaðýr, bir yer seç ve git. Arada geçiþlerde beklesin diye ýnvoke yazdýk.
                    anim.SetBool("Walk", false);

                    isSearched = true;
                }           
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;          
        }
    }

    private void Search()
    {
        agent.speed = searchSpeed;
        agent.isStopped = false;
        isSearched = false;
        anim.SetBool("Walk", true);
        agent.SetDestination(GetRandomPosition());
    }

    private void Attack()
    {
        if (player == null)
        {
            return;
        }

        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        anim.SetBool("Walk", false);
        //agent.velocity = Vector3.zero;
        agent.isStopped = true;   //Ajan saldýrýrken duracak.
        LookTheTarget(player.position);
    }


    private void Chase()
    {
        if (player == null)
        {
            return;
        }
        agent.isStopped = false;   //Ajan durmadan bize gelecek.
        agent.speed = chaseSpeed;
        anim.SetBool("Walk", true);
        agent.SetDestination(player.position);
    } 

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackRate);
        anim.SetTrigger("Attack");
        yield return new WaitUntil(IsAttackAnimationFinished);   //WaitUntil'e gelen fonksiyon true olunca tutmayý býrakýr ve devam ettirir, false ise devam ettirmez.
        isAttacking = false;
    }

    private bool IsAttackAnimationFinished()
    {
        //Animasyona ulaþýp animasyonun bitip bitmediðini kontrol ediyoruz.
        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private void LookTheTarget(Vector3 target)
    {
        Vector3 lookPos = new Vector3(target.x, transform.position.y, target.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), turnSpeed * Time.deltaTime);
    }

    private Vector3 GetRandomPosition()
    {
        //insideUnitSphere = Bir daire oluþturur ve dairenin içinde rastgele bir konumu seçer ve o konumu bize vector3 olarak döner. Onu float yani patrolradiusla çarpýnca çapý büyür.
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;   //Navmeshde bir noktayý atamak için kullanýcaz.
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);    //O noktanýn navmesh üzerinde gidlebilir olup olmadýðýný kontrol ederiz. 1 bu kodun hangi bölgelerde çalýþacaðýný belirtir.
        return hit.position;
    }

    public int GetDamage()
    {
        return damage;
    }
}
