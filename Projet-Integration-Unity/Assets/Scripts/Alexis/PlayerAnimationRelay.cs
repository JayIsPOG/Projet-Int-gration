using UnityEngine;

public class PlayerAnimationRelay : MonoBehaviour
{
    Player_attack playerAttack;

    void Awake()
    {
        playerAttack = GetComponentInParent<Player_attack>();
    }

    public void ActivateHitbox()
    {
        playerAttack?.EnableAttackHitbox();
    }

    public void EnableAttackHitbox()
    {
        playerAttack?.EnableAttackHitbox();
    }

    public void DeactivateHitbox()
    {
        playerAttack?.DisableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        playerAttack?.DisableAttackHitbox();
    }
}
