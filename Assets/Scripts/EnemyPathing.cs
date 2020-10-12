using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    // Tunables
    [SerializeField] bool randomizePathing = false;
    [SerializeField] float randomizePathingJitter = 1.5f;
    [SerializeField] float shipTimeout = 15.0f;

    //Positioning
    [SerializeField] float shipSafeBottomZone = 3.0f;
    float shipPadding = 1.0f;
    float xMin = 0.0f;
    float xMax = 0.0f;
    float yMin = 0.0f;
    float yMax = 0.0f;

    // State
    int waypointIndex = 0;
    bool waveConfigSet = false;
    float startTime = 0.0f; // used to log lifetime of ship
    [SerializeField] float xScreenUnits = 5.625f;

    // Cached references
    WaveConfig waveConfig = null;
    List<Transform> waypoints = null;
    List<GameObject> pathingUpdates = null;
    Enemy enemy = null;

    // Update is called once per frame
    void Update()
    {
        if (waveConfigSet)
        {
            Move();
            CheckAndKillStuckShips();
        }
    }

    private void CheckAndKillStuckShips()
    {
        float timeElapsedSinceCreated = Time.time - startTime;
        if (timeElapsedSinceCreated > shipTimeout)
        {
            KillPathingUpdates();
            enemy.KillShip();
        }
    }

    public void InitializePathing(WaveConfig waveConfig)
    {
        // Pull relevant ship information
        enemy = gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            shipPadding = enemy.GetShipPadding();
        }
        // Set initialization time to despawn if colliding
        startTime = Time.time;

        // Set wave config, initialize pathing
        this.waveConfig = waveConfig;
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].position;
        waveConfigSet = true;
        if (randomizePathing)
        {
            RandomizePathing();
        }
    }

    private void RandomizePathing()
    {
        SetMoveBoundaries(); // setting clamping boundaries for randomized positions
        pathingUpdates = new List<GameObject>(); // define list for updated pathing objects 

        for (int waypointModIndex = 1; waypointModIndex < (waypoints.Count - 1); waypointModIndex++) // Skip the first and last entries, avoid pushing weird directions off-screen
        {
            Transform waypointBasis = waypoints[waypointModIndex];
            // Once spawned, stay in bounds -- clamp to game window
            float randomXPosition = Mathf.Clamp(waypointBasis.position.x + UnityEngine.Random.Range(-randomizePathingJitter, randomizePathingJitter), xMin, xMax);
            float randomYPosition = Mathf.Clamp(waypointBasis.position.y + UnityEngine.Random.Range(-randomizePathingJitter, randomizePathingJitter), yMin, yMax);
            Vector2 randomizedPosition = new Vector3(randomXPosition, randomYPosition, waypointBasis.position.z);

            // Copy into new game object and replace to avoid over-write original pathing
            GameObject randomizeWaypointObject = new GameObject("PathingUpdate");
            randomizeWaypointObject.transform.position = randomizedPosition;
            waypoints[waypointModIndex] = randomizeWaypointObject.transform;
            pathingUpdates.Add(randomizeWaypointObject);
        }
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            // Move to next waypoint
            Vector3 targetPosition = waypoints[waypointIndex].transform.position;
            float movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

            // Check if reached waypoint
            float distanceToTarget = (transform.position - targetPosition).magnitude;
            if (Mathf.Approximately(distanceToTarget, 0.0f))
            {
                waypointIndex++;
            }
        }
        else
        {
            KillPathingUpdates();
            Destroy(gameObject); // simple destroy over-ride here -- no fanfare since off screen
        }
    }

    public void KillPathingUpdates()
    {
        if (pathingUpdates != null)
        {
            // Destroy any pathing update objects (if randomizing)
            foreach (GameObject pathingUpdate in pathingUpdates)
            {
                Destroy(pathingUpdate);
            }
        }
    }

    private void SetMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        Vector3 cornerBottomLeft = new Vector3(0, 0, 0);
        Vector3 cornerTopRight = new Vector3(1, 1, 0);
        //xMin = gameCamera.ViewportToWorldPoint(cornerBottomLeft).x + shipPadding;
        xMin = -xScreenUnits + shipPadding; // full screen breaks bounding, so temporary hack
        yMin = gameCamera.ViewportToWorldPoint(cornerBottomLeft).y + shipPadding + shipSafeBottomZone;
        //xMax = gameCamera.ViewportToWorldPoint(cornerTopRight).x - shipPadding;
        xMax = xScreenUnits - shipPadding; // full screen breaks bounding, so temporary hack
        yMax = gameCamera.ViewportToWorldPoint(cornerTopRight).y - shipPadding;
    }
}
