using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score = null;
    [SerializeField] TextMeshProUGUI wave = null;
    [SerializeField] TextMeshProUGUI difficulty = null;
    Level level = null;

    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<Level>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = level.GetScore().ToString();
        wave.text = level.GetFinalWave().ToString();
        difficulty.text = level.GetFinalDifficulty().ToString();
    }
}
