using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    public bool isDead = false;

    [Header("Health Bar")]
    [SerializeField] float barWidth = 1f;
    [SerializeField] float barHeight = 0.12f;
    [SerializeField] float barOffsetY = 1.7f;

    [Header("SFX")]
    [SerializeField] AudioClip hitSfx;
    [SerializeField] AudioClip deathSfx;
    [SerializeField] float sfxVolume = 1f;

    Animator animator;
    SpriteRenderer[] spriteRenderers;
    Transform healthBarParent;
    Transform greenBar;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        // parent qui suit l'ennemi
        healthBarParent = new GameObject("HealthBar").transform;
        healthBarParent.SetParent(transform);
        healthBarParent.localPosition = new Vector3(0, barOffsetY, 0);
        healthBarParent.localScale = Vector3.one;

        // fond rouge
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(healthBarParent);
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localScale = new Vector3(barWidth, barHeight, 1);
        SpriteRenderer bgRenderer = bg.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = CreatePixelSprite();
        bgRenderer.color = Color.red;
        bgRenderer.sortingLayerName = "UI";
        bgRenderer.sortingOrder = 100;

        // barre verte
        GameObject fg = new GameObject("Fill");
        fg.transform.SetParent(healthBarParent);
        fg.transform.localPosition = Vector3.zero;
        fg.transform.localScale = new Vector3(barWidth, barHeight, 1);
        SpriteRenderer fgRenderer = fg.AddComponent<SpriteRenderer>();
        fgRenderer.sprite = CreatePixelSprite();
        fgRenderer.color = Color.green;
        fgRenderer.sortingLayerName = "UI";
        fgRenderer.sortingOrder = 101;

        greenBar = fg.transform;
    }

    Sprite CreatePixelSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }

    void UpdateHealthBar()
    {
        if (greenBar == null) return;

        float pct = (float)currentHealth / maxHealth;
        greenBar.localScale = new Vector3(barWidth * pct, barHeight, 1);

        // decale la barre pour qu'elle se vide de droite a gauche
        float offset = (barWidth - barWidth * pct) / 2f;

        // compense le flip de l'ennemi
        float flipDir = transform.localScale.x >= 0 ? 1f : -1f;
        greenBar.localPosition = new Vector3(-offset * flipDir, 0, 0);
    }

    void LateUpdate()
    {
        // empeche la barre de flipper avec l'ennemi
        if (healthBarParent != null)
        {
            healthBarParent.localScale = new Vector3(
                transform.localScale.x >= 0 ? 1 : -1, 1, 1
            );
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        if (hitSfx != null) AudioSource.PlayClipAtPoint(hitSfx, transform.position, sfxVolume);

        UpdateHealthBar();
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
        EnemyMovement movement = GetComponent<EnemyMovement>();
        if (movement != null)
            movement.enabled = false;

        ArcherMovement archerMovement = GetComponent<ArcherMovement>();
        if (archerMovement != null)
            archerMovement.enabled = false;

        Collider2D col = GetComponentInChildren<Collider2D>();
        if (col != null)
            col.enabled = false;

        // cache la barre de vie
        if (healthBarParent != null)
            healthBarParent.gameObject.SetActive(false);

        isDead = true;
        if (deathSfx != null) AudioSource.PlayClipAtPoint(deathSfx, transform.position, sfxVolume);
        animator.SetTrigger("Dead");
    }
}
