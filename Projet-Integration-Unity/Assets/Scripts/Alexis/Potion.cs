using UnityEngine;

public class Potion : PowerUp
{
    [Header("Heal")]
    public int healAmount = 10;

    protected override void OnPickup()
    {
        if (playerHealth != null)
            playerHealth.Heal(healAmount);
    }
}
