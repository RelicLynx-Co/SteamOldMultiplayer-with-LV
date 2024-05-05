using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyAI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private GameObject projectile;

    [Header("Parameters")]
    [SerializeField] private float health;
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;
    [Header("Fall stuff")]
    [SerializeField] private float respawnDelay = 3f;
    private Vector3 respawnPosition;
    private bool isRespawning = false;

    // Patroling
    private Vector3 walkPoint;
    private bool walkPointSet;

    // Attacking
    private bool alreadyAttacked;

    private void Start()
    {
        respawnPosition = transform.position;
    }
    private void Awake()
    {
        StartCoroutine(FindPlayerCoroutine());
    }

    private IEnumerator FindPlayerCoroutine()
    {
        while (player == null)
        {
            player = GameObject.Find("Player")?.transform;
            yield return null; // Wait for the next frame before checking again
        }
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
 if (transform.position.y <= -200)
    {
            Destroy(gameObject);
        }

        bool playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        bool playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange || playerInAttackRange)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, whatIsPlayer);

            foreach (Collider collider in colliders)
            {
                Vector3 playerPosition = collider.transform.position;
                player = collider.transform;

            }
        }

        if (!playerInSightRange && !playerInAttackRange)
        {Patroling();}
        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        if (playerInAttackRange && playerInSightRange)
            AttackPlayer();


    }
   
    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet && agent.isActiveAndEnabled)
        {
            // Set destination and draw debug line
            agent.SetDestination(walkPoint);
            Debug.DrawLine(transform.position, walkPoint, Color.blue);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            // If near the walk point, mark it as reached
            walkPointSet = false;
            //Debug.Log("Reached walk point");
        }
    }


    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        // Check if the walkPoint is on the ground
        RaycastHit hit;
        if (Physics.Raycast(walkPoint, -transform.up, out hit, 2f, whatIsGround))
        {
            walkPointSet = true;
            walkPoint = hit.point; // Set walkPoint to the point where the raycast hit the ground
        }
        else
        {
            walkPointSet = false;
        }
    }

private void ChasePlayer()
{
    UnityEngine.AI.NavMeshAgent navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
    {
        navMeshAgent.SetDestination(player.position);
    }
}
        private void AttackPlayer()
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            if (!alreadyAttacked)
            {
                SpwanBulletServerRPC(transform.position, transform.rotation);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    [ServerRpc]
    private void SpwanBulletServerRPC(Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
    {
        float enemyHeight = GetComponent<Collider>().bounds.size.y;
        Vector3 spawnPosition = position + rotation * Vector3.forward*2 + Vector3.up * (enemyHeight / 2f);
        // Instantiate the bullet
        GameObject inst_Bullet = Instantiate(projectile, spawnPosition, rotation);

        // Ignore collisions between the bullet and the spawner object
        int spawnerLayer = gameObject.layer; // Assuming the spawner object's layer is used
        Physics.IgnoreCollision(inst_Bullet.GetComponent<Collider>(), GetComponent<Collider>());

        // Spawn the bullet with ownership
        inst_Bullet.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
