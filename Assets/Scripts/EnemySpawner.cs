using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Difficulty Tunables")]
    [SerializeField] bool gameLooping = true;
    [SerializeField] List<WaveConfig> waveConfigs = null;
    [SerializeField] float timeBetweenWaves = 5.0f;
    [SerializeField] float difficultyNumberOfWavesPerDecrement = 8;
    [SerializeField] float difficultyWaveDelayDecrementer = 1.0f;
    [SerializeField] float difficultyMinimumTimeBetweenWaves = 2.0f;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI score = null;
    [SerializeField] TextMeshProUGUI health = null;

    // State
    int numberOfWavesSurvived = 0;
    int currentWaveIndex = 0;

    // Cached References
    Level level = null;
    GameObject continueText;
    Player player = null;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Port over level for scoring, initialize scoring + continue text for use later
        level = FindObjectOfType<Level>();
        level.ResetScores();
        continueText = GameObject.Find("ContinueText");
        continueText.SetActive(false);

        // Grab player for player status
        player = FindObjectOfType<Player>();

        // Main game wave generation loop
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (gameLooping);
    }

    private void Update()
    {
        // Update scoreboard UI
        score.text = level.GetScore().ToString();
        health.text = player.GetHealth().ToString();

        // Handling for player death
        if (player.IsShipDead())
        {
            continueText.SetActive(true);
            if (Input.GetButtonDown("Fire1"))
            {
                level.LoadGameOver();
            }
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for (int enemyIndex = 0; enemyIndex < waveConfig.GetNumberOfEnemies(); enemyIndex++)
        {
            GameObject newEnemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetWaypoints()[0].transform.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyPathing>().InitializePathing(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns() + UnityEngine.Random.Range(0.0f, waveConfig.GetJitterBetweenSpawns()));
        }
    }

    private IEnumerator SpawnAllWaves()
    {
        /* Loop through waves approach
        foreach (WaveConfig waveConfig in waveConfigs)
        {
            StartCoroutine(SpawnAllEnemiesInWave(waveConfig));
            yield return new WaitForSeconds(timeBetweenSpawns);
        } */

        // Randomize wave approach
        int waveIndex = (int)UnityEngine.Random.Range(0.0f, waveConfigs.Count);
        while (currentWaveIndex == waveIndex) // prevent repeat same wave twice
        {
            waveIndex = (int)UnityEngine.Random.Range(0.0f, waveConfigs.Count);
        }
        currentWaveIndex = waveIndex;

        // Spawn this wave
        StartCoroutine(SpawnAllEnemiesInWave(waveConfigs[waveIndex]));
        numberOfWavesSurvived++;
        level.IncrementWave();

        // Increase difficulty
        IncreaseDifficulty();
        yield return new WaitForSeconds(timeBetweenWaves);
    }

    private void IncreaseDifficulty()
    {
        if (numberOfWavesSurvived % difficultyNumberOfWavesPerDecrement == 0)
        {
            timeBetweenWaves = Mathf.Clamp(timeBetweenWaves - difficultyWaveDelayDecrementer, difficultyMinimumTimeBetweenWaves, timeBetweenWaves);
            level.IncrementDifficulty();
        }
    }
}
