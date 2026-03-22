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
        Debug.Log("damageRealy");
        if (movement != null)
            movement.DealDamage();
    }
}