using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ArcherMovement : MonoBehaviour
{
    public float speed = 3f;
    public Transform player;

    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public float arrowSpeed = 20f;
    public int arrowDamage = 10;
    [SerializeField] GameObject arrowPrefab;

    private float lastAttackTime = -999f;
    PlayerHealth playerHealth;
    Player_attack playerAttack;

    public enum EnemyState
    {
        Chasing,
        Attacking
    }

    public EnemyState state;

    Rigidbody2D rb;
    GridManager grid;
    Animator animator;

    List<Node> path;
    int pathIndex;

    Vector3 lastPlayerPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        Tilemap tilemap = GameObject.Find("Tilemap Fountain").GetComponent<Tilemap>();
        grid = new GridManager(tilemap);

        player = GameObject.FindWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        playerAttack = player.GetComponent<Player_attack>();

        if (arrowPrefab == null)
            arrowPrefab = Resources.Load<GameObject>("Arrow");

        lastPlayerPos = player.position;
        state = EnemyState.Chasing;

        CalculatePath();
    }

    void FixedUpdate()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case EnemyState.Chasing:

                if (distToPlayer <= attackRange)
                {
                    rb.velocity = Vector2.zero;
                    path = null;
                    state = EnemyState.Attacking;
                    animator.SetBool("IsChasing", false);
                    break;
                }

                if (Vector2.Distance(lastPlayerPos, player.position) > 0.5f)
                {
                    lastPlayerPos = player.position;
                    CalculatePath();
                }

                MoveAlongPath();
                break;

            case EnemyState.Attacking:

                if (distToPlayer > attackRange)
                {
                    lastPlayerPos = player.position;
                    state = EnemyState.Chasing;
                    animator.SetBool("IsChasing", true);
                    CalculatePath();
                    break;
                }

                rb.velocity = Vector2.zero;
                Attack();
                break;
        }
    }

    void Attack()
    {
        float dir = player.position.x - transform.position.x;
        Flip(dir);

        if (Time.time - lastAttackTime >= attackCooldown && playerHealth.currentHealth > 0)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
        }
    }

    public void DealDamage()
    {
if (arrowPrefab == null) return;

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        ArrowProjectile proj = arrow.GetComponent<ArrowProjectile>();
        if (proj == null)
            proj = arrow.AddComponent<ArrowProjectile>();

        proj.Init(player.position, arrowSpeed, arrowDamage, playerHealth, playerAttack);
    }

    void CalculatePath()
    {
        Vector3 target = player.position;

        if (Vector2.Distance(transform.position, target) <= attackRange)
        {
            rb.velocity = Vector2.zero;
            path = null;
            return;
        }

        Node start = grid.NodeFromWorld(transform.position);
        Node end = grid.NodeFromWorld(target);

        if (start == null || end == null || !end.walkable)
            return;

        path = grid.FindPath(start, end);
        pathIndex = 0;
    }

    void MoveAlongPath()
    {
        if (path == null || pathIndex >= path.Count)
        {
            rb.velocity = Vector2.zero;
            path = null;

            float distToPlayer = Vector2.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange)
            {
                state = EnemyState.Attacking;
                animator.SetBool("IsChasing", false);
            }
            else
            {
                CalculatePath();
            }

            return;
        }

        Vector2 target = grid.WorldFromNode(path[pathIndex]);

        Flip(target.x - rb.position.x);

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, target) < 0.05f)
            pathIndex++;
    }

    void Flip(float dir)
    {
        if (dir > 0)
            transform.localScale = Vector3.one;
        else if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
