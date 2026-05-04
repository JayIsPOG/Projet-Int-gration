using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float hitRadius = 0.6f;

    float speed;
    int damage;
    Vector3 targetPos;
    PlayerHealth playerHealth;
    Player_attack playerAttack;

    public void Init(Vector3 target, float arrowSpeed, int arrowDamage, PlayerHealth health, Player_attack attack)
    {
        targetPos = target;
        speed = arrowSpeed;
        damage = arrowDamage;
        playerHealth = health;
        playerAttack = attack;

        // rotation vers la cible
        Vector2 dir = (targetPos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            bool playerClose = playerHealth != null
                && Vector2.Distance(transform.position, playerHealth.transform.position) <= hitRadius;

            if (playerClose && playerHealth.currentHealth > 0
                && !(playerAttack != null && playerAttack.IsBlocking))
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
