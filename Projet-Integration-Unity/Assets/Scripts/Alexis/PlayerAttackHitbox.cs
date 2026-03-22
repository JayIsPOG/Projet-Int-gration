using UnityEngine;
using System.Collections.Generic;

public class PlayerAttackHitbox : MonoBehaviour
{
    HashSet<EnemyHealth> alreadyHit = new HashSet<EnemyHealth>();
    BoxCollider2D col;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        alreadyHit.Clear();
    }

    void Update()
    {
        Physics2D.SyncTransforms();

        Vector2 center = (Vector2)transform.position + col.offset;
        Vector2 size = col.size * (Vector2)transform.lossyScale;

        // dessine la hitbox en vert dans la scene view
        Debug.DrawLine(center + new Vector2(-size.x/2, -size.y/2), center + new Vector2(size.x/2, -size.y/2), Color.green);
        Debug.DrawLine(center + new Vector2(size.x/2, -size.y/2), center + new Vector2(size.x/2, size.y/2), Color.green);
        Debug.DrawLine(center + new Vector2(size.x/2, size.y/2), center + new Vector2(-size.x/2, size.y/2), Color.green);
        Debug.DrawLine(center + new Vector2(-size.x/2, size.y/2), center + new Vector2(-size.x/2, -size.y/2), Color.green);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0);
        Debug.Log("[HITBOX] center:" + center + " size:" + size + " found:" + hits.Length + " colliders");

        foreach (var hit in hits)
        {
            Debug.Log("[HITBOX] collider found: " + hit.gameObject.name + " layer:" + hit.gameObject.layer);
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null && !alreadyHit.Contains(enemyHealth))
            {
                alreadyHit.Add(enemyHealth);
                Debug.Log("[HIT] " + enemyHealth.gameObject.name + " HP: " + enemyHealth.currentHealth);
                enemyHealth.TakeDamage(10);
            }
        }
    }
}
