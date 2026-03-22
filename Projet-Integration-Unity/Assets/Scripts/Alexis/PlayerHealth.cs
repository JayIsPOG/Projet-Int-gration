using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 30;
    public int currentHealth;
    Animator animator;
    public PlayerMovement playerMovement;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(int damage)
    {
        if(currentHealth - damage >= 0)
            currentHealth -= damage;

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerMovement.enabled = false;
        animator.SetTrigger("Die");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}