using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    EnemyMovement movement;

    void Awake()
    {
        movement = GetComponentInParent<EnemyMovement>();
    }

    public void DealDamage()
    {
        if (movement != null)
            movement.DealDamage();
    }
}