using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    EnemyMovement movement;
    ArcherMovement archerMovement;

    void Awake()
    {
        movement = GetComponentInParent<EnemyMovement>();
        archerMovement = GetComponentInParent<ArcherMovement>();
    }

    public void DealDamage()
    {
if (movement != null)
            movement.DealDamage();
        else if (archerMovement != null)
            archerMovement.DealDamage();
    }
}