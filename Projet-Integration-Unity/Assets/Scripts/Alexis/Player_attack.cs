using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_attack : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement pm;
    private Rigidbody2D rb;
    private bool leftReleased = true;
    private bool rightReleased = true;
    private bool isAttacking = false;
    [SerializeField] private GameObject blockObject;

    // hitbox settings
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] Vector2 attackBoxSize = new Vector2(2f, 1.5f);
    [SerializeField] int attackDamage = 10;
    [SerializeField] float hitboxDelay = 0.3f;
    [SerializeField] float hitboxDuration = 0.3f;

    public bool IsBlocking { get; private set; }

    // garde pour les animation events dans PlayerAnimationRelay
    public void EnableAttackHitbox() { }
    public void DisableAttackHitbox() { }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!Input.GetMouseButton(0)) leftReleased = true;
        if (!Input.GetMouseButton(1)) rightReleased = true;

        if (Input.GetMouseButton(0) && leftReleased && !IsBlocking && !isAttacking)
        {
            animator.SetTrigger("Attack");
            leftReleased = false;
            StartCoroutine(AttackRoutine());
        }

        if (Input.GetMouseButton(1) && rightReleased)
        {
            IsBlocking = true;
            animator.SetBool("IsBlocking", true);
            blockObject?.SetActive(true);
            rb.velocity = Vector2.zero;
            pm.enabled = false;
            rightReleased = false;
        }
        else if (!Input.GetMouseButton(1))
        {
            IsBlocking = false;
            animator.SetBool("IsBlocking", false);
            blockObject?.SetActive(false);
            pm.enabled = true;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(hitboxDelay);

        // direction du joueur (localScale.x negatif = droite, positif = gauche)
        float dir = transform.localScale.x < 0 ? 1f : -1f;
        Vector2 facing = new Vector2(dir, 0f);

        // trouve tous les ennemis dans le demi-cercle devant le joueur
        HashSet<EnemyHealth> alreadyHit = new HashSet<EnemyHealth>();
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();

        foreach (var enemy in allEnemies)
        {
            if (enemy == null) continue;

            Vector2 toEnemy = (Vector2)enemy.transform.position - (Vector2)transform.position;
            float dist = toEnemy.magnitude;

            // demi-cercle: l'ennemi doit etre dans le rayon ET devant le joueur (dot > 0)
            bool inRange = dist < attackRange;
            bool inFront = Vector2.Dot(facing, toEnemy.normalized) > 0f;

            if (inRange && inFront && !alreadyHit.Contains(enemy))
            {
                alreadyHit.Add(enemy);
                enemy.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
    }

}
