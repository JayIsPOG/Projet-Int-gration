using System.Collections;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [Header("Pickup")]
    public float pickupRange = 0.5f;

    [Header("Idle anim")]
    public float bobAmplitude = 0.1f;
    public float bobSpeed = 3f;
    public float pulseAmplitude = 0.08f;
    public float pulseSpeed = 4f;

    [Header("Pickup anim")]
    public float disappearDuration = 0.3f;

    [Header("SFX")]
    [SerializeField] AudioClip pickupSfx;
    [SerializeField] float pickupSfxVolume = 1f;

    protected Transform player;
    protected PlayerHealth playerHealth;
    protected SpriteRenderer sr;

    Vector3 basePos;
    Vector3 baseScale;
    bool picked = false;

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        baseScale = transform.localScale;
        basePos = transform.position;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerHealth = p.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (picked) return;

        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = basePos + new Vector3(0f, bob, 0f);

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        transform.localScale = baseScale * pulse;

        if (player != null && Vector2.Distance(transform.position, player.position) <= pickupRange)
        {
            picked = true;
            if (pickupSfx != null) AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupSfxVolume);
            OnPickup();
            StartCoroutine(DisappearAnim());
        }
    }

    protected abstract void OnPickup();

    IEnumerator DisappearAnim()
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = sr != null ? sr.color : Color.white;

        while (t < disappearDuration)
        {
            t += Time.deltaTime;
            float k = t / disappearDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, k);
            transform.position += new Vector3(0f, Time.deltaTime * 1.5f, 0f);

            if (sr != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, k);
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
