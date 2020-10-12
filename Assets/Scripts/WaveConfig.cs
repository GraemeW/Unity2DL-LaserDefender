using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] GameObject enemyPrefab = null;
    [SerializeField] GameObject pathPrefab = null;
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float jitterBetweenSpawns = 0.3f;
    [SerializeField] int numberOfEnemies = 5;
    [SerializeField] float moveSpeed = 2.0f;

    public GameObject GetEnemyPrefab() { return enemyPrefab; }
    public List<Transform> GetWaypoints() 
    {
        List<Transform> waveWaypoints = new List<Transform>();
        foreach(Transform childPath in pathPrefab.transform)
        {
            waveWaypoints.Add(childPath);
        }
        return waveWaypoints; 
    }
    public float GetTimeBetweenSpawns() { return timeBetweenSpawns; }
    public float GetJitterBetweenSpawns() { return jitterBetweenSpawns; }
    public int GetNumberOfEnemies() { return numberOfEnemies; }
    public float GetMoveSpeed() { return moveSpeed; }
}
