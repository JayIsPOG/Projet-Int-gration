using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float delayBetweenSpawns = 1.5f;
    [SerializeField] float delayBetweenWaves = 3f;

    [Header("UI")]
    [SerializeField] UIDocument uiDocument;

    int currentWave = 0;
    int totalWaves = 5;
    List<EnemyHealth> aliveEnemies = new List<EnemyHealth>();
    Label waveLabel;
    bool waveInProgress = false;

    // nombre d'ennemis par vague
    int[] enemiesPerWave = { 2, 3, 4, 5, 7 };

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        waveLabel = root.Q<Label>("WaveLabel");

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (!waveInProgress) return;

        // enleve les ennemis morts de la liste
        aliveEnemies.RemoveAll(e => e == null);

        // tous morts = prochaine vague
        if (aliveEnemies.Count == 0)
        {
            waveInProgress = false;

            if (currentWave < totalWaves)
                StartCoroutine(StartNextWave());
            else
                waveLabel.text = "Victoire!";
        }
    }

    IEnumerator StartNextWave()
    {
        currentWave++;
        waveLabel.text = "Wave " + currentWave + "/" + totalWaves;

        yield return new WaitForSeconds(delayBetweenWaves);

        int enemiesToSpawn = enemiesPerWave[currentWave - 1];

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // alterne entre les spawn points
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                aliveEnemies.Add(health);

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        waveInProgress = true;
    }
}
