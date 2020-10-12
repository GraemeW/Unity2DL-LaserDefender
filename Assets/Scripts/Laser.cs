using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Tunables
    [SerializeField] float projectleMultiplier = 100.0f;
    [SerializeField] AudioClip laserSound = null;
    [SerializeField] [Range(0, 1)] float laserVolume = 1.0f;
    [SerializeField] bool isRotating = false;
    [SerializeField] float rotatingSpeed = 20.0f;

    // Cached references
    Rigidbody2D laserRigidbody2D = null;

    // Start is called before the first frame update
    void Awake()
    {
        laserRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Catch rogue projectiles, delete if they travel too far
        if (transform.position.magnitude > 20.0f)
        {
            Destroy(gameObject);
        }
        
        // Rotating property for fancy weapons
        if (isRotating)
        {
            transform.Rotate(0.0f, 0.0f, rotatingSpeed * Time.deltaTime);
        }
    }

    public void Launch(Vector2 direction, float projectileForce)
    {
        laserRigidbody2D.AddForce(direction * projectileForce * projectleMultiplier);
    }

    public AudioClip GetLaserSound()
    {
        return laserSound;
    }

    public float GetLaserVolume()
    {
        return laserVolume;
    }
}
