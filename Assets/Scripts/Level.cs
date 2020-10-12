using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Level : MonoBehaviour
{
    [SerializeField] float currentScore = 0.0f; // Serialize for debug
    [SerializeField] int currentWave = 0; // Serialize for debug
    [SerializeField] int currentDifficulty = 1;
    int finalWave = 0;
    int finalDifficulty = 0;

    private void Awake()
    {
        int levelCount = FindObjectsOfType<Level>().Length;
        if (levelCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void ResetScores()
    {
        currentScore = 0.0f;
        currentWave = 0;
        finalWave = 0;
        currentDifficulty = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddToScore(float scoreAdder)
    {
        currentScore += scoreAdder;
    }

    public void IncrementWave()
    {
        currentWave++;
    }

    public void IncrementDifficulty()
    {
        currentDifficulty++;
    }

    public void SetFinalWave(int finalWave)
    {
        this.finalWave = finalWave;
        finalDifficulty = currentDifficulty;
    }

    public float GetScore()
    {
        return currentScore;
    }

    public int GetWave()
    {
        return currentWave;
    }

    public int GetFinalWave()
    {
        return finalWave;
    }

    public int GetFinalDifficulty()
    {
        return finalDifficulty;
    }
}
