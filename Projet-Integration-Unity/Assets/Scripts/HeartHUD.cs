using UnityEngine;
using UnityEngine.UIElements;

public class HeartHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private VisualElement[] hearts = new VisualElement[3];
    private int lastHealth = -1;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = root.Q<VisualElement>("HeartsContainer");

        hearts[0] = container.ElementAt(0);
        hearts[1] = container.ElementAt(1);
        hearts[2] = container.ElementAt(2);

        UpdateHearts(playerHealth.currentHealth);
    }

    void Update()
    {
        if (playerHealth.currentHealth != lastHealth)
        {
            lastHealth = playerHealth.currentHealth;
            UpdateHearts(lastHealth);
        }
    }

    void UpdateHearts(int health)
    {
        // Heart i is full if health > i * 10
        // 30hp -> all full | 20hp -> Heart3 empty | 10hp -> Heart2+3 empty | 0hp -> all empty
        for (int i = 0; i < 3; i++)
        {
            Sprite sprite = health > i * 10 ? fullHeart : emptyHeart;
            hearts[i].style.backgroundImage = new StyleBackground(sprite);
        }
    }
}
