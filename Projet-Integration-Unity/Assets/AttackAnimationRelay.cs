using UnityEngine;

public class AttackAnimationRelay : MonoBehaviour
{
    private Player_attack playerAttack;

    void Start()
    {
        playerAttack = GetComponentInParent<Player_attack>();
    }

    public void ActivateHitbox()
    {
        playerAttack.ActivateHitbox();
    }

    public void DeactivateHitbox()
    {
        playerAttack.DeactivateHitbox();
    }
}
