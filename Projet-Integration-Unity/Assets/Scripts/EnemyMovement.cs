using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public Transform player;

    public float sideOffsetX = 1.5f;
    public float offsetY = -0.2f;
    public float stopDistance = 0.7f;

    public float attackCooldown = 1f;

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

        lastPlayerPos = player.position;
        state = EnemyState.Chasing;

        CalculatePath();
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case EnemyState.Chasing:

                // Recalcule le path si le joueur bouge
                if (Vector2.Distance(lastPlayerPos, player.position) > 0.5f)
                {
                    lastPlayerPos = player.position;
                    CalculatePath();
                }

                MoveAlongPath();
                break;

            case EnemyState.Attacking:

                if (PlayerMoved())
                {
                    Debug.Log("player moved");

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

    bool PlayerMoved()
    {
        return Vector2.Distance(lastPlayerPos, player.position) > 0.5f;
    }

    void CalculatePath()
    {
        Vector3 target = GetSidePosition();

        if (Vector2.Distance(transform.position, target) <= stopDistance)
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

    Vector3 GetSidePosition()
    {
        float side = transform.position.x < player.position.x ? -sideOffsetX : sideOffsetX;
        return player.position + new Vector3(side, offsetY, 0);
    }
    public void DealDamage()
    {
        if (playerHealth != null && !(playerAttack != null && playerAttack.IsBlocking))
        {
             playerHealth.TakeDamage(10);
             Debug.Log("damage "+ playerHealth.currentHealth);
        }
       
    }
    void MoveAlongPath()
    {
        if (path == null || pathIndex >= path.Count)
        {
            rb.velocity = Vector2.zero;
            path = null;

            state = EnemyState.Attacking;
            animator.SetBool("IsChasing", false);

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