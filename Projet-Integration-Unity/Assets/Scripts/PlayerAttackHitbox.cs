using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    void TryHit(Collider2D other)
    {
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>();
        if (enemy != null)
            Destroy(enemy.gameObject);
    }
}
