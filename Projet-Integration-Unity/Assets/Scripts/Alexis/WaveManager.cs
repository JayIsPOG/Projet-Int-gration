using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float delayBetweenSpawns = 1.5f;
    [SerializeField] float delayBetweenWaves = 3f;

    [Header("Power-ups")]
    [SerializeField] GameObject[] powerUpPrefabs;
    [SerializeField] Tilemap walkableTilemap;
    [SerializeField] float minDistanceFromPlayer = 3f;
    [SerializeField] int maxSpawnAttempts = 30;

    GridManager powerUpGrid;

    [Header("UI")]
    [SerializeField] UIDocument uiDocument;

    int currentWave = 0;
    int totalWaves = 5;
    List<EnemyHealth> aliveEnemies = new List<EnemyHealth>();
    Label waveLabel;
    VisualElement victoryPanel;
    Button victoryReplayButton;
    bool waveInProgress = false;
    bool gameWon = false;

    // nombre d'ennemis par vague
    int[] enemiesPerWave = { 2, 3, 4, 5, 7 };

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        waveLabel = root.Q<Label>("WaveLabel");
        victoryPanel = root.Q("VictoryPanel");
        victoryReplayButton = root.Q<Button>("VictoryReplayButton");

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        // rejouer via clic sur le bouton (detection manuelle)
        if (gameWon && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = RuntimePanelUtils.ScreenToPanel(
                victoryPanel.panel, new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            if (victoryReplayButton.worldBound.Contains(mousePos))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
        }

        if (!waveInProgress) return;

        // enleve les ennemis morts de la liste
        aliveEnemies.RemoveAll(e => e == null || e.isDead);

        // tous morts = prochaine vague
        if (aliveEnemies.Count == 0)
        {
            waveInProgress = false;

            if (currentWave < totalWaves)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
                gameWon = true;
                // desactive le joueur
                var player = FindObjectOfType<PlayerMovement>();
                if (player != null)
                {
                    player.enabled = false;
                    var attack = player.GetComponent<Player_attack>();
                    if (attack != null) attack.enabled = false;
                }
                victoryPanel.style.display = DisplayStyle.Flex;
            }
        }
    }

    Transform GetSpawnPointOutsideCamera()
    {
        Camera cam = Camera.main;

        List<Transform> validPoints = new List<Transform>();
        foreach (Transform sp in spawnPoints)
        {
            Vector3 vp = cam.WorldToViewportPoint(sp.position);
            bool inView = vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f && vp.z > 0f;
            if (!inView)
                validPoints.Add(sp);
        }

        if (validPoints.Count == 0)
            validPoints = new List<Transform>(spawnPoints);

        return validPoints[Random.Range(0, validPoints.Count)];
    }

    IEnumerator StartNextWave()
    {
        currentWave++;
        waveLabel.text = "Wave " + currentWave + "/" + totalWaves;

        yield return new WaitForSeconds(delayBetweenWaves);

        // nettoie les cadavres de la vague precedente
        foreach (var corpse in FindObjectsOfType<EnemyHealth>())
            if (corpse.isDead) Destroy(corpse.gameObject);

        SpawnPowerUps();

        int enemiesToSpawn = enemiesPerWave[currentWave - 1];

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = GetSpawnPointOutsideCamera();
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                aliveEnemies.Add(health);

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        waveInProgress = true;
    }

    void SpawnPowerUps()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0 || walkableTilemap == null) return;

        if (powerUpGrid == null)
            powerUpGrid = new GridManager(walkableTilemap);

        Transform player = null;
        var pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) player = pm.transform;

        // un de plus a chaque vague
        int count = currentWave;

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                Node n = powerUpGrid.GetRandomWalkableNode();
                if (n == null) break;

                Vector3 pos = powerUpGrid.WorldFromNode(n);

                if (player != null && Vector2.Distance(pos, player.position) < minDistanceFromPlayer)
                    continue;

                GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity);
                break;
            }
        }
    }
}
