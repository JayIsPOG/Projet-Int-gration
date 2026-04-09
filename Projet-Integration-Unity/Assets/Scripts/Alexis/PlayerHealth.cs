using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 30;
    public int currentHealth;
    Animator animator;
    SpriteRenderer[] spriteRenderers;
    public PlayerMovement playerMovement;
    [SerializeField] UIDocument uiDocument;

    VisualElement defeatPanel;
    Button defeatReplayButton;
    bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        defeatPanel = root.Q("DefeatPanel");
        defeatReplayButton = root.Q<Button>("DefeatReplayButton");
    }

    void Update()
    {
        if (isDead && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = RuntimePanelUtils.ScreenToPanel(
                defeatPanel.panel, new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            if (defeatReplayButton.worldBound.Contains(mousePos))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if(currentHealth - damage >= 0)
            currentHealth -= damage;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        foreach (var sr in spriteRenderers)
            sr.color = Color.red;
        yield return new WaitForSeconds(0.08f);
        foreach (var sr in spriteRenderers)
            sr.color = Color.white;
    }

    void Die()
    {
        isDead = true;
        playerMovement.enabled = false;
        var attack = GetComponent<Player_attack>();
        if (attack != null) attack.enabled = false;
        animator.SetTrigger("Die");

        defeatPanel.style.display = DisplayStyle.Flex;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}